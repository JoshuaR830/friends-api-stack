using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.Lambda.APIGatewayEvents;
using BotuaGetFriendTimes.Models;

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

            Data data = new Data(
                new List<string>
                {
                    "Monday",
                    "Tuesday",
                    "Wednesday",
                    "Thursday",
                    "Friday",
                    "Saturday",
                    "Sunday"
                }, new List<Dataset>
                {
                    new Dataset("Andrew", new List<double> { 1.5, 2, 3, 2, 3, 5, 1 }),
                    new Dataset("Dayle", new List<double> { 1.5, 2, 3, 2, 3, 5, 1 }),
                    new Dataset("Jonny", new List<double> { 1.5, 2, 3, 2, 3, 5, 1 }),
                    new Dataset("Jordan", new List<double> { 1.5, 2, 3, 2, 3, 5, 1 }),
                    new Dataset("Joshua", new List<double> { 1.5, 2, 3, 2, 3, 5, 1 }),
                    new Dataset("Lucas", new List<double> { 1.5, 2, 3, 2, 3, 5, 1 }),
                    new Dataset("Madalyn", new List<double> { 1.5, 2, 3, 2, 3, 5, 1 }),
                    new Dataset("Martin", new List<double> { 1.5, 2, 3, 2, 3, 5, 1 }),
                }
            );

            var serialisedData = JsonSerializer.Serialize(data);
            
            return new APIGatewayProxyResponse
            {
                StatusCode = 200,
                Headers = new Dictionary<string, string> {{"Content-Type", "application/json"}},
                Body = serialisedData
            };
        }
    }
}