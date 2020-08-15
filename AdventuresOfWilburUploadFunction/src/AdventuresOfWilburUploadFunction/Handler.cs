using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AdventuresOfWilbur;
using Amazon.Lambda.APIGatewayEvents;
using Newtonsoft.Json;
using System.Drawing;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;

namespace AdventuresOfWilburUploadFunction
{
    public class Handler
    {
        IAmazonS3 _amazonS3;
        
        public Handler(IAmazonS3 amazonS3)
        {
            _amazonS3 = amazonS3;
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

            const string bucketName = "adventures-of-wilbur-images";

            await fileTransferUtility.UploadAsync(new MemoryStream(bytes, 0, bytes.Length), bucketName, fileName);
            
            return new APIGatewayProxyResponse
            {
                StatusCode = 200,
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } },
                Body = input.Body
            };
        }
    }
}