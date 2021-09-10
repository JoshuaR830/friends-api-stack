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
        public string ChannelName {get; set;}
        public bool IsAfk {get; set;}
        public bool IsMuted {get; set;}
        public bool IsDeafened {get; set;}
        public bool IsVideoOn {get; set;}
        public bool IsStreaming {get; set;}

        public TimeItem(string sessionGuid, string channelId, string endTimestamp, string startTimestamp, string serverId, string userId, string channelName, bool isAfk, bool isMuted, bool isDeafened, bool isVideoOn, bool isStreaming)
        {
            SessionId = new Guid(sessionGuid);
            ChannelId = Convert.ToInt64(channelId);
            EndTimestamp = Convert.ToInt64(endTimestamp);
            StartTimestamp = Convert.ToInt64(startTimestamp);
            ServerId = Convert.ToInt64(serverId);
            UserId = Convert.ToInt64(userId);
            ChannelName = channelName;
            IsAfk = isAfk;
            IsMuted = isMuted;
            IsDeafened = isDeafened;
            IsVideoOn = isVideoOn;
            IsStreaming = isStreaming;
        }
    }
}