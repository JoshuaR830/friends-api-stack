using Amazon.Lambda.APIGatewayEvents;
using System.Threading.Tasks;
using System.Collections.Generic;
using Amazon.EC2;
using Amazon.EC2.Model;
using System.Linq;
using Amazon.S3;
using System.IO;
using System;
using Newtonsoft.Json;

namespace BotuaEC2Server
{
    public class Handler
    {
        private IAmazonEC2 _ec2;
        private IAmazonS3 _s3;

        public Handler(IAmazonEC2 ec2, IAmazonS3 s3)
        {
            _ec2 = ec2;
            _s3 = s3;
        }

        public async Task<APIGatewayProxyResponse> Handle(APIGatewayProxyRequest input)
        {
            if(!input.QueryStringParameters.TryGetValue("serverType", out var serverType))
                return new APIGatewayProxyResponse
                {
                    StatusCode = 404,
                    Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } },
                    Body = "Server not found"
                };

            var requestedGameServerInstances = await _ec2.DescribeInstancesAsync(new DescribeInstancesRequest
            {
                Filters = new List<Filter> 
                {
                    new Filter
                    {
                        Name = $"tag:Name",
                        Values = new List<string>
                        {
                            serverType
                        }
                    } 
                }
            });

            string s3Key;
            string instanceId;

            if (serverType == "Minecraft")
            {
                instanceId = "Minecraft";
                s3Key = "minecraft/minecraft-user-data.txt";
            }
            else if (serverType == "Lego-universe")
            {
                instanceId = "Lego Universe";
                s3Key = "darkflame-universe/scripts/lego-universe-user-data.txt";
            }
            else
            {
                return new APIGatewayProxyResponse
                {
                    StatusCode = 404,
                    Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } },
                    Body = "Invalid game choice"
                };
            }

            foreach (var reservations in requestedGameServerInstances.Reservations)
            {
                Console.WriteLine(JsonConvert.SerializeObject(reservations));
                var state = reservations.Instances.Where(x => x.State.Name.Value.ToLower() == "running");
                Console.WriteLine($"Currently running: {state}");
                Console.WriteLine($"Has any: {state.Any()}");
                if (state.Any())
                {
                    return new APIGatewayProxyResponse
                    {
                        StatusCode = 200,
                        Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } },
                        Body = $"A {instanceId} server is already running, please play on that one!"
                    };
                }
            }

            var userDataObject = await _s3.GetObjectAsync("joshua-game-hosting", s3Key);
            var reader = new StreamReader(userDataObject.ResponseStream);
            var userData = reader.ReadToEnd();

            var base64UserData = System.Text.Encoding.UTF8.GetBytes(userData);
            var stringBase64UserData = System.Convert.ToBase64String(base64UserData);

            var instanceRequest = new RunInstancesRequest
            {
                ImageId = "ami-0b22ba4613bff958a",
                MinCount = 1,
                MaxCount = 1,
                InstanceType = "t3.small",
                KeyName = "game-server",
                SecurityGroupIds = new List<string> { "sg-09189d8dd284e475b" },
                IamInstanceProfile = new IamInstanceProfileSpecification
                {
                    Arn = "arn:aws:iam::266804135388:instance-profile/game-hosting"
                },
                UserData = stringBase64UserData,
                TagSpecifications = new List<TagSpecification>
                {
                    new TagSpecification
                    {
                        ResourceType = "instance",
                        Tags = new List<Tag>
                        {
                            new Tag
                            {
                                Key = "Name",
                                Value = serverType,
                            }
                        }
                    }
                }
            };

            var newInstance = await _ec2.RunInstancesAsync(instanceRequest);

            return new APIGatewayProxyResponse
            {
                StatusCode = 200,
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } },
                Body = $"A {instanceId} has been started, it may be a few minutes until it is ready to play!"
            };
        }
    }
}