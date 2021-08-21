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
            var scanResponse = await _dynamoDb.ScanAsync(new ScanRequest
            {
                TableName = "FriendTimeTable",
                FilterExpression = "#ST >= :startTimestamp AND #ET <= :endTimestamp",
                ExpressionAttributeNames = new Dictionary<string, string>
                {
                    {"#ST", "StartTimestamp"},
                    {"#ET", "EndTimestamp"}
                },
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    {":startTimestamp", new AttributeValue { N = startTime.ToString()}},
                    {":endTimestamp", new AttributeValue { N = endTime.ToString()}},
                    // ToDo: at some point pu the channel id in here - need to get it from a secure place though
                },
            });

            var timeItems = new List<TimeItem>();

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

            return timeItems;
        }
    }
}