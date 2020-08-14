using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;

namespace AdventuresOfWilbur
{
    public class Handler
    {
        private const string BucketBaseUrl = "https://adventures-of-wilbur-images.s3.eu-west-2.amazonaws.com";
        
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
                var storyItem = int.Parse(input.QueryStringParameters["storyItemNumber"]);
                Console.WriteLine(storyItem);

                var imageName = "";
                if (storyItem == 1)
                {
                    imageName = "WP_20160601_20_39_24_Pro.jpg";
                }
                else
                {
                    imageName = "WP_20160601_20_38_09_Pro.jpg";
                }
                
                return new APIGatewayProxyResponse
                {
                    StatusCode = 200,
                    Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } },
                    Body = $"{BucketBaseUrl}/{imageName}"
                };
            }
            catch
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