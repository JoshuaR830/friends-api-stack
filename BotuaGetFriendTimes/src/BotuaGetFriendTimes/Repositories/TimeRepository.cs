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
                var scanRequest = new ScanRequest
                {
                    TableName = "FriendTimeTable",
                    FilterExpression = "#ST >= :startTimestamp AND #ET <= :endTimestamp",
                    ExpressionAttributeNames = new Dictionary<string, string>
                        {{"#ST", "StartTimestamp"}, {"#ET", "EndTimestamp"}},
                    ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                    {
                        {":startTimestamp", new AttributeValue {N = startTime.ToString()}},
                        {":endTimestamp", new AttributeValue {N = endTime.ToString()}},
                        // ToDo: at some point put the channel id in here - need to get it from a secure place though
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


                    Console.WriteLine("About to get channel name");
                    
                    var channelName = GetStringValue(item, "ChannelName") ?? "";
                    Console.WriteLine($"Channel name is {channelName}");
                    
                    var isAfk = TryConvertToBool(GetStringValue(item, "IsAfk"), false);
                    var isMuted = TryConvertToBool(GetStringValue(item, "IsMuted"), false);
                    var isDeafened = TryConvertToBool(GetStringValue(item, "IsDeafened"), false);
                    var isVideoOn = TryConvertToBool(GetStringValue(item, "IsVideoOn"), false);
                    var isStreaming = TryConvertToBool(GetStringValue(item, "IsStreaming"), false);

                    timeItems.Add(new TimeItem(sessionGuid, channelId, endTimestamp, startTimestamp, serverId, userId, channelName, isAfk, isMuted, isDeafened, isVideoOn, isStreaming));
                }
            } while (scanResponse.LastEvaluatedKey.ContainsKey("SessionGuid"));

            return timeItems;
        }

        private bool TryConvertToBool(string @string, bool @bool)
        {
            Console.WriteLine($"Ths will be converted to bool {@string}");
            return @string != null ? bool.Parse(@string) : @bool;
        }

        private string GetStringValue(Dictionary<string, AttributeValue> item, string key)
        {
            if (item.ContainsKey("ChannelName"))
            {
                Console.WriteLine("Contains key");
                return item["ChannelName"].S;
            }

            Console.WriteLine("Does not contain key");
            return null;
        }
    }
}