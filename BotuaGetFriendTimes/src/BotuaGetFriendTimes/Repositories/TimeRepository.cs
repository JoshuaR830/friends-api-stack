using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using BotuaGetFriendTimes.Models;

namespace BotuaGetFriendTimes.Repositories
{
    public class TimeRepository : ITimeRepository
    {
        private readonly IAmazonDynamoDB _dynamoDb;
        
        public TimeRepository(IAmazonDynamoDB dynamoDb)
        {
            _dynamoDb = dynamoDb;
        }
        
        public async Task<IEnumerable<TimeItem>> GetTimeByTimeRange(long startTime, long endTime)
        {
            ScanResponse scanResponse = null;
            var timeItems = new List<TimeItem>();

            do
            {
                Console.WriteLine("Looping");
                var scanRequest = new ScanRequest
                {
                    Limit = 2,
                    TableName = "FriendTimeTable",
                    FilterExpression = "#ST >= :startTimestamp AND #ET <= :endTimestamp",
                    ExpressionAttributeNames = new Dictionary<string, string>
                        {{"#ST", "StartTimestamp"}, {"#ET", "EndTimestamp"}},
                    ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                    {
                        {":startTimestamp", new AttributeValue {N = startTime.ToString()}},
                        {":endTimestamp", new AttributeValue {N = endTime.ToString()}},
                        // ToDo: at some point pu the channel id in here - need to get it from a secure place though
                    },
                };

                if (scanResponse != null)
                {
                    if (scanResponse.LastEvaluatedKey.ContainsKey("SessionGuid"))
                    {
                        scanRequest.ExclusiveStartKey = scanResponse.LastEvaluatedKey;
                    }
                }
                
                scanResponse = await _dynamoDb.ScanAsync(scanRequest);

                var scanResponseItems = scanResponse.Items;
                foreach (var item in scanResponseItems)
                {
                    // ToDo: check that the results actually match this
                    var sessionGuid = item["SessionGuid"].S;
                    var channelId = item["ChannelId"].N;
                    var endTimestamp = item["EndTimestamp"].N;
                    var startTimestamp = item["StartTimestamp"].N;
                    var serverId = item["ServerId"].N;
                    var userId = item["UserId"].N;

                    timeItems.Add(new TimeItem(sessionGuid, channelId, endTimestamp, startTimestamp, serverId, userId));
                }
            } while (scanResponse.LastEvaluatedKey.ContainsKey("SessionGuid"));

            Console.WriteLine($"Item count {timeItems.Count}");

            return timeItems;
        }
    }
}