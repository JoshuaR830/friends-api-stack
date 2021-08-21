using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.Lambda.APIGatewayEvents;
using BotuaGetFriendTimes.Helpers;
using BotuaGetFriendTimes.Models;
using Microsoft.VisualBasic.CompilerServices;

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
            var days = int.Parse(input.QueryStringParameters["days"]);
            days--;

            var millis = days * TimeHelper.DayLengthMillis;

            var currentMillis = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            var endTime = TimeHelper.GetEndOfDayMillis(TimeHelper.ConvertToDateTime(currentMillis - TimeHelper.DayLengthMillis));
            var startTime = TimeHelper.GetStartOfDayMillis(TimeHelper.ConvertToDateTime(endTime - millis));

            Console.WriteLine($"Start time {startTime}");
            Console.WriteLine($"End time {endTime}");
            
            var duration = "day";

            // var startTimeUnix = TimeHelper.ConvertDateStringToUnix(startTime);
            // var endTimeUnix = TimeHelper.ConvertDateStringToUnix(endTime);

            // ToDo: get the data from the DB between startTimeUnix and endTimeUnix
            
            var dateData = TimeHelper.ConvertToDateStringRange(startTime, endTime);
            
            // ToDo: Sort the date into daily chunks that can be used (between start and end times)

            var dateLabels = new List<string>();
            
            foreach (var dateInstance in dateData)
            {
                dateLabels.Add(dateInstance.DateString);
                Console.WriteLine(dateInstance.DateString);
            }
            
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