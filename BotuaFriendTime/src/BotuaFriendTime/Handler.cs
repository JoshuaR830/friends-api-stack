using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.Lambda.APIGatewayEvents;

namespace BotuaFriendTime
{
    public class Handler
    {
        private readonly IFriendRepository _friendRepository;

        
        public Handler(IFriendRepository repository)
        {
            _friendRepository = repository;
        }
        
        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<APIGatewayProxyResponse> Handle (APIGatewayProxyRequest input)
        {
            
            var sessionGuid = input.QueryStringParameters["sessionGuid"];
            var userId = long.Parse(input.QueryStringParameters["userId"]);
            var timestamp = long.Parse(input.QueryStringParameters["timestamp"]);
            var serverId = long.Parse(input.QueryStringParameters["serverId"]);
            var channelId = long.Parse(input.QueryStringParameters["channelId"]);
            var connectionStatus = bool.Parse(input.QueryStringParameters["connectionStatus"]);
            var channelName = input.QueryStringParameters["channelName"];
            var isStreaming = bool.Parse(input.QueryStringParameters["isStreaming"]);
            var isVideoOn = bool.Parse(input.QueryStringParameters["isVideoOn"]);
            var isMuted = bool.Parse(input.QueryStringParameters["isMuted"]);
            var isDeafened = bool.Parse(input.QueryStringParameters["isDeafened"]);
            var isAfk = bool.Parse(input.QueryStringParameters["isAfk"]);

            Console.WriteLine(userId);
            Console.WriteLine(timestamp);

            if (connectionStatus)
            {
                await _friendRepository.PutNewFriendTimeData(sessionGuid, userId, timestamp, serverId, channelId, channelName, isStreaming, isVideoOn, isMuted, isDeafened, isAfk);
            }
            else
            {
                await _friendRepository.AlterExistingFriendTimeData(sessionGuid, userId, timestamp, serverId, channelId);
            }
            
            return new APIGatewayProxyResponse
            {
                StatusCode = 200,
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } },
                Body = "Hello"
            };
        }
    }
}