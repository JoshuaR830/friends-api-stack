using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.Lambda.APIGatewayEvents;

namespace BotuaGetFriendTimes
{
    public class Handler
    {
        private readonly IAmazonDynamoDB _dynamoDb;
    
        public Handler(IAmazonDynamoDB dynamoDb)
        {
            _dynamoDb = dynamoDb;
        }
        
        public async Task<APIGatewayProxyResponse> Handle(APIGatewayProxyRequest input)
        {
            return new APIGatewayProxyResponse
            {
                StatusCode = 200,
                Headers = new Dictionary<string, string> {{"Content-Type", "application/json"}},
                Body = "Time"
            };
        }
    }
}