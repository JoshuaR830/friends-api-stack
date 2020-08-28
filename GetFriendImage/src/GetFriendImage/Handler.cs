using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.APIGatewayEvents;
using Newtonsoft.Json;

namespace GetFriendImage
{
    public class Handler
    {
        private readonly IAmazonDynamoDB _dynamoDb;
        
        private const string BucketBaseUrl = "https://generic-images.s3.eu-west-2.amazonaws.com/jordan/2948bd67-99fa-4fb5-9500-8c4f21d41b2e/7a35d82f-7662-42d5-a235-dfe34acd8109/images";

        public Handler(IAmazonDynamoDB dynamoDb)
        {
            _dynamoDb = dynamoDb;
        }
        
        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<APIGatewayProxyResponse> Handle(APIGatewayProxyRequest input)
        {
            try
            {
                var scanRequest = new ScanRequest
                {
                    TableName = "FriendImageTable",
                    ProjectionExpression = "#imageUrl",
                    ExpressionAttributeNames = new Dictionary<string, string>
                    {
                        {"#imageUrl", "ImageUrl"},
                    },
                };

                var items = new List<Dictionary<string, AttributeValue>>();

                var response = await _dynamoDb.ScanAsync(scanRequest);

                items.AddRange(response.Items);
                
                while (response.LastEvaluatedKey.Values.Count != 0)
                {
                    scanRequest = new ScanRequest
                    {
                        TableName = "FriendImageTable",
                        ExclusiveStartKey = new Dictionary<string, AttributeValue>
                        {
                            {"ImageId", new AttributeValue{ S = response.LastEvaluatedKey.ToString() }}
                        },
                        ProjectionExpression = "#imageUrl",
                        ExpressionAttributeNames = new Dictionary<string, string>
                        {
                            {"#imageUrl", "ImageUrl"},
                        },
                    };
                    
                    response = await _dynamoDb.ScanAsync(scanRequest);

                    items.AddRange(response.Items);
                }

                if(items.Count == 0)
                    return new APIGatewayProxyResponse
                    {
                        StatusCode = 403,
                        Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } },
                        Body = ""
                    };

                Console.WriteLine($"Number of items: {items.Count}");

                var random = new Random();
                var index = random.Next(0, items.Count);

                Console.WriteLine($"Index: {index}");
                
                var imageName = items[index]["ImageUrl"].S;

                Console.WriteLine(imageName);

                var imageUrl = $"{BucketBaseUrl}/{imageName}";
                
                var imageData = new ImageData(imageUrl);
                var serializedImageData = JsonConvert.SerializeObject(imageData);
                
                return new APIGatewayProxyResponse
                {
                    StatusCode = 200,
                    Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } },
                    Body = serializedImageData
                };
            }
            catch(KeyNotFoundException)
            {
                return new APIGatewayProxyResponse
                {
                    StatusCode = 403,
                    Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } },
                    Body = ""
                };
            }
        }
    }
}