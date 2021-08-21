using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BotuaGetFriendTimes.Models;

namespace BotuaGetFriendTimes.Repositories
{
    public interface ITimeRepository
    {
        Task<IEnumerable<TimeItem>> GetTimeByTimeRange(long startTime, long endTime);
    }
}