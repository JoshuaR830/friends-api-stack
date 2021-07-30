using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.Lambda.APIGatewayEvents;

namespace BotuaFriendTime
{
    public class Handler
    {
        private readonly IAmazonDynamoDB _dynamoDb;

        
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
        public async Task<APIGatewayProxyResponse> Handle (APIGatewayProxyRequest input)
        {
            return new APIGatewayProxyResponse
            {
                StatusCode = 200,
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } },
                Body = "Hello"
            };
        }
    }
}