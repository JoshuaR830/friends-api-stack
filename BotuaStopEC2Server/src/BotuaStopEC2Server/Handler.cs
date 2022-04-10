using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.EC2;
using Amazon.EC2.Model;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.S3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BotuaStopEC2Server
{
    public class Handler
    {
        private IAmazonEC2 _ec2;
        private IAmazonS3 _s3;
        private IAmazonDynamoDB _dynamo;

        public Handler(IAmazonEC2 ec2, IAmazonS3 s3, IAmazonDynamoDB dynamoDB)
        {
            _ec2 = ec2;
            _s3 = s3;
            _dynamo = dynamoDB;
        }

        public async Task<APIGatewayProxyResponse> Handle(APIGatewayProxyRequest input)
        {
            if (!input.QueryStringParameters.TryGetValue("serverType", out var serverType))
                return new APIGatewayProxyResponse
                {
                    StatusCode = 404,
                    Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } },
                    Body = "Server not found"
                };

            var instances = await _ec2.DescribeInstancesAsync(new DescribeInstancesRequest
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

            var hasRunningInstances = instances.Reservations.Any(x => x.Instances.Where(y => y.State.Name.Value.ToLower() == "running").Any());

            var instanceFriendlyName = "";
            if (serverType == "Minecraft")
            {
                if(!hasRunningInstances)
                {
                    return new APIGatewayProxyResponse
                    {
                        StatusCode = 200,
                        Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } },
                        Body = $"All {instanceFriendlyName} servers are already stopped!"
                    };
                }

                instanceFriendlyName = "Minecraft";

                await _dynamo.UpdateItemAsync(new UpdateItemRequest
                {
                    TableName = "FriendMetadataTable",
                    Key = new Dictionary<string, AttributeValue>
                    {
                        { "server_name", new AttributeValue {S = "Minecraft"} }
                    },
                    ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                    {
                        { ":shouldTerminate", new AttributeValue {BOOL = true} }
                    },
                    ExpressionAttributeNames = new Dictionary<string, string>
                    {
                        { "#ST", "should_terminate" }
                    },
                    UpdateExpression = "SET #ST = :shouldTerminate"
                });

                return new APIGatewayProxyResponse
                {
                    StatusCode = 200,
                    Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } },
                    Body = $"{instanceFriendlyName} server is scheduled to stop soon"
                };
            }
            else if (serverType == "Lego-universe")
            {
                instanceFriendlyName = "Lego Universe";
                foreach (var reservations in instances.Reservations)
                {
                    var instanceToStop = reservations.Instances.Where(x => x.State.Name.Value.ToLower() == "running");
                    if (instanceToStop.Any())
                    {
                        var instanceIdStopped = instanceToStop.First().InstanceId;

                        await _ec2.TerminateInstancesAsync(new TerminateInstancesRequest
                        {
                            InstanceIds = new List<string> { instanceIdStopped }
                        });

                        return new APIGatewayProxyResponse
                        {
                            StatusCode = 200,
                            Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } },
                            Body = $"Stopped {instanceFriendlyName} with id:  {instanceIdStopped}"
                        };
                    }
                }
            }

            

            return new APIGatewayProxyResponse
            {
                StatusCode = 200,
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } },
                Body = $"No {instanceFriendlyName} server to stop"
            };
        }
    }
}