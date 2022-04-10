using Amazon.EC2;
using Amazon.EC2.Model;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.S3;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BotuaStopEC2Server
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

            var instanceFriendlyName = "";
            if (serverType == "Minecraft")
            {
                instanceFriendlyName = "Minecraft";
            }
            else if (serverType == "Lego-universe")
            {
                instanceFriendlyName = "Lego Universe";
            }

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

            return new APIGatewayProxyResponse
            {
                StatusCode = 200,
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } },
                Body = $"No {instanceFriendlyName} server to stop"
            };
        }
    }
}