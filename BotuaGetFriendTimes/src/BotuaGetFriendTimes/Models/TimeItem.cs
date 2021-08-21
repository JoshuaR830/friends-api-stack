using System;

namespace BotuaGetFriendTimes.Models
{
    public class TimeItem
    {
        public Guid SessionId {get; set;}
        public long ChannelId {get; set;}
        public long EndTimestamp {get; set;}
        public long StartTimestamp {get; set;}
        public long ServerId {get; set;}
        public long UserId {get; set;}
        public TimeItem(string sessionGuid, string channelId, string endTimestamp, string startTimestamp, string serverId, string userId)
        {
            SessionId = new Guid(sessionGuid);
            ChannelId = Convert.ToInt64(channelId);
            EndTimestamp = Convert.ToInt64(endTimestamp);
            StartTimestamp = Convert.ToInt64(startTimestamp);
            ServerId = Convert.ToInt64(serverId);
            UserId = Convert.ToInt64(userId);
        }
    }
}