using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime.Internal.Transform;

namespace BotuaFriendTime
{
    public class FriendRepository : IFriendRepository
    {
        private IAmazonDynamoDB _dynamo;

        public FriendRepository(IAmazonDynamoDB dynamo)
        {
            _dynamo = dynamo;
        }

        public async Task PutNewFriendTimeData(string sessionGuid, long userId, long timestamp, long serverId, long channelId, string channelName, bool isStreaming, bool isVideoOn, bool isMuted, bool isDeafened, bool isAfk)
        {
            var putTimeRequest = new PutItemRequest
            {
                TableName = "FriendTimeTable",
                Item = new Dictionary<string, AttributeValue>
                {
                    {"SessionGuid", new AttributeValue{S = sessionGuid}},
                    {"UserId", new AttributeValue{N = userId.ToString()}},
                    {"ServerId", new AttributeValue{N = serverId.ToString()}},
                    {"ChannelId", new AttributeValue{N = channelId.ToString()}},
                    {"StartTimestamp", new AttributeValue{N = timestamp.ToString()}},
                    {"ChanelName", new AttributeValue{S = channelName}},
                    {"IsStreaming", new AttributeValue{S = isStreaming.ToString()}},
                    {"IsVideoOn", new AttributeValue{S = isVideoOn.ToString()}},
                    {"IsMuted", new AttributeValue{S = isMuted.ToString()}},
                    {"IsDeafened", new AttributeValue{S = isDeafened.ToString()}},
                    {"IsAfk", new AttributeValue{S = isAfk.ToString()}}
                }
            };

            await _dynamo.PutItemAsync(putTimeRequest);
        }

        public async Task AlterExistingFriendTimeData(string sessionGuid, long userId, long timestamp, long serverId, long channelId)
        {
            var updateItemRequest = new UpdateItemRequest
            {
                TableName = "FriendTimeTable",
                Key = new Dictionary<string, AttributeValue>
                {
                    {
                        // This isn't the key anymore
                        "SessionGuid", new AttributeValue
                        {
                            S = sessionGuid
                        }
                    }
                },
                // ConditionExpression = "#SId = :sessionId",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    {":timestamp", new AttributeValue {N = timestamp.ToString()}},
                },
                ExpressionAttributeNames = new Dictionary<string, string>
                {
                    {"#ET", "EndTimestamp"},
                },
                UpdateExpression = "SET #ET = :timestamp"
            };

            await _dynamo.UpdateItemAsync(updateItemRequest);
        }
    }
}