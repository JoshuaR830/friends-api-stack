using System;
using System.Collections.Generic;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Newtonsoft.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace AdventuresOfWilbur
{
    public class Function
    {
        private const string BucketBaseUrl = "https://adventures-of-wilbur-images.s3.eu-west-2.amazonaws.com";
        
        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public APIGatewayProxyResponse FunctionHandlerAsync(APIGatewayProxyRequest input, ILambdaContext context)
        {
            try
            {
                var somethingElse = int.Parse(input.QueryStringParameters["storyItemNumber"]);
                Console.WriteLine(somethingElse);

                var imageName = "";
                if (somethingElse == 1)
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
