using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace AdventuresOfWilbur
{
    public class Function
    {
        private const string BucketBaseUrl = "https://adventures-of-wilbur-images.s3.eu-west-2.amazonaws.com/";
        
        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public APIGatewayProxyResponse FunctionHandlerAsync(APIGatewayProxyRequest input, ILambdaContext context)
        {
            Console.WriteLine(input);
            var imageName = "WP_20160601_20_39_24_Pro.jpg";

            var response = new APIGatewayProxyResponse
            {
                StatusCode = 200,
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } },
                Body = $"BucketBaseUrl/{imageName}"
            };

            return response;
        }
    }
}
