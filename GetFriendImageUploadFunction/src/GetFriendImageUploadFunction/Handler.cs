using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Newtonsoft.Json;
using System.Drawing;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.S3;
using Amazon.S3.Transfer;

namespace GetFriendImageUploadFunction
{
    public class Handler
    {
        private readonly IAmazonS3 _amazonS3;
        private readonly IAmazonDynamoDB _dynamoDb;
        
        public Handler(IAmazonS3 amazonS3, IAmazonDynamoDB dynamoDb)
        {
            _amazonS3 = amazonS3;
            _dynamoDb = dynamoDb;
        }

        public async Task<APIGatewayProxyResponse> Handle(APIGatewayProxyRequest input)
        {
            Console.WriteLine(input.Body);
            var body = JsonConvert.DeserializeObject<ImageData>(input.Body);
            var bytes = Convert.FromBase64String(body.Image);
            var fileName = body.FileName;

            

            Console.WriteLine(bytes);
            
            Image newImage;

            try
            {
                using (var stream = new MemoryStream(bytes, 0, bytes.Length))
                {
                    stream.Write(bytes, 0, bytes.Length);
                    newImage = Image.FromStream(stream, true);
                }
            }
            catch
            {
                newImage = null;
            }

            Console.WriteLine(newImage);
            
            var fileTransferUtility = new TransferUtility(_amazonS3);

            const string bucketName = "generic-images";

            await fileTransferUtility.UploadAsync(new MemoryStream(bytes, 0, bytes.Length), bucketName, fileName);
            
            var putItemRequest = new PutItemRequest
            {
                TableName = "FriendImageTable",
                Item = new Dictionary<string, AttributeValue>
                {
                    {"ImageId", new AttributeValue {N = body.SequenceNumber.ToString()}},
                    {"Description", new AttributeValue {S = body.Description}},
                    {"Friends", new AttributeValue {SS = body.Friends }},
                    {"ImageKey", new AttributeValue {S = body.FileName}},
                    {"Title", new AttributeValue {S = body.Title}}
                }
            };

            await _dynamoDb.PutItemAsync(putItemRequest);
            
            return new APIGatewayProxyResponse
            {
                StatusCode = 200,
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } },
                Body = input.Body
            };
        }
    }
}