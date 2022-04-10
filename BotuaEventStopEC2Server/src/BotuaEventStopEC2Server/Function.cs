using Amazon.DynamoDBv2;
using Amazon.Lambda.Core;
using Amazon.Lambda.S3Events;
using Amazon.S3;
using Amazon.EC2;
using Amazon.DynamoDBv2.Model;
using System;
using System.Threading.Tasks;
using Amazon.EC2.Model;
using System.Collections.Generic;
using System.Linq;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace BotuaEventStopEC2Server
{
    public class Function
    {
        IAmazonS3 S3Client { get; set; }
        IAmazonDynamoDB DynamoDbClient { get; set; }
        IAmazonEC2 Ec2Client { get; set; }

        /// <summary>
        /// Default constructor. This constructor is used by Lambda to construct the instance. When invoked in a Lambda environment
        /// the AWS credentials will come from the IAM role associated with the function and the AWS region will be set to the
        /// region the Lambda function is executed in.
        /// </summary>
        public Function()
        {
            S3Client = new AmazonS3Client();
            DynamoDbClient = new AmazonDynamoDBClient();
            Ec2Client = new AmazonEC2Client();
        }

        /// <summary>
        /// Constructs an instance with a preconfigured S3 client. This can be used for testing the outside of the Lambda environment.
        /// </summary>
        /// <param name="s3Client"></param>
        public Function(IAmazonS3 s3Client)
        {
            this.S3Client = s3Client;
        }
    
        /// <summary>
        /// This method is called for every Lambda invocation. This method takes in an S3 event object and can be used 
        /// to respond to S3 notifications.
        /// </summary>
        /// <param name="evnt"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<string?> FunctionHandler(S3Event evnt, ILambdaContext context)
        {
            var s3Event = evnt.Records?[0].S3;
            if(s3Event == null)
            {
                return null;
            }

            try
            {
                var response = await this.S3Client.GetObjectMetadataAsync(s3Event.Bucket.Name, s3Event.Object.Key);

                if (s3Event.Bucket.Name == "joshua-game-backup" && s3Event.Object.Key == "minecraft-backup/save.zip")
                {
                    var dynamoDbResponse = await DynamoDbClient.GetItemAsync(new GetItemRequest
                    {
                        TableName = "FriendMetadataTable",
                        Key = new Dictionary<string, AttributeValue>
                        {
                            { "server_name", new AttributeValue {S = "Minecraft"} }
                        }
                    });

                    if(dynamoDbResponse.IsItemSet)
                    {
                        Console.WriteLine($"The server name is {dynamoDbResponse.Item["server_name"]}");

                        if(dynamoDbResponse.Item.TryGetValue("should_terminate", out var shouldTerminate))
                        {
                            // ToDo - let EC2 do some whacky stuff here

                            if (!shouldTerminate.BOOL)
                            {
                                Console.WriteLine($"Not going to terminate");
                                return null;
                            }

                            var instances = await Ec2Client.DescribeInstancesAsync(new DescribeInstancesRequest
                            {
                                Filters = new List<Filter>
                                {
                                    new Filter
                                    {
                                        Name = $"tag:Name",
                                        Values = new List<string>
                                        {
                                            "Minecraft"
                                        }
                                    }
                                }
                             });

                            foreach (var reservations in instances.Reservations)
                            {
                                var instanceToStop = reservations.Instances.Where(x => x.State.Name.Value.ToLower() == "running");
                                if (instanceToStop.Any())
                                {
                                    var instanceIdStopped = instanceToStop.First().InstanceId;

                                    await Ec2Client.TerminateInstancesAsync(new TerminateInstancesRequest
                                    {
                                        InstanceIds = new List<string> { instanceIdStopped }
                                    });

                                    Console.WriteLine($"Stopped {instanceIdStopped}");
                                }
                            }

                            await DynamoDbClient.UpdateItemAsync(new UpdateItemRequest
                            {
                                TableName = "FriendMetadataTable",
                                Key = new Dictionary<string, AttributeValue>
                                {
                                    { "server_name", new AttributeValue {S = "Minecraft"} }
                                },
                                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                                {
                                    { ":shouldTerminate", new AttributeValue {BOOL = false} }
                                },
                                ExpressionAttributeNames = new Dictionary<string, string>
                                {
                                    { "#ST", "should_terminate" }
                                },
                                UpdateExpression = "SET #ST = :shouldTerminate"
                            });
                        }
                    } else
                    {
                        Console.WriteLine("Item not set");
                    }

                    Console.WriteLine("Minecraft backed up");

                } else
                {
                    Console.WriteLine("Not the save zip");
                }

                return response.Headers.ContentType;
            }
            catch(Exception e)
            {
                Console.WriteLine($"Error getting object {s3Event.Object.Key} from bucket {s3Event.Bucket.Name}. Make sure they exist and your bucket is in the same region as this function.");
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                throw;
            }
        }
    }
}

