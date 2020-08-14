using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        public string FunctionHandlerAsync(object input, ILambdaContext context)
        {
            Console.WriteLine(input);
            var imageName = "WP_20160601_20_39_24_Pro.jpg";
            return $"BucketBaseUrl/{imageName}";
        }
    }
}
