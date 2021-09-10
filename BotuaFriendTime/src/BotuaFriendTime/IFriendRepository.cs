using System.Threading.Tasks;

namespace BotuaFriendTime
{
    public interface IFriendRepository
    {
        Task AlterExistingFriendTimeData(string sessionGuid, long userId, long timestamp, long serverId, long channelId);
        Task PutNewFriendTimeData(string sessionGuid, long userId, long timestamp, long serverId, long channelId, string channelName, bool isStreaming, bool isVideoOn, bool isMuted, bool isDeafened, bool isAfk);
    }
}