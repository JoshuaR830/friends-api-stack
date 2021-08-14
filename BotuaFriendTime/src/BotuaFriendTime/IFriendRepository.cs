using System.Threading.Tasks;

namespace BotuaFriendTime
{
    public interface IFriendRepository
    {
        Task PutNewFriendTimeData(string sessionGuid, long userId, long timestamp, long serverId, long channelId);
        Task AlterExistingFriendTimeData(string sessionGuid, long userId, long timestamp, long serverId, long channelId);
    }
}