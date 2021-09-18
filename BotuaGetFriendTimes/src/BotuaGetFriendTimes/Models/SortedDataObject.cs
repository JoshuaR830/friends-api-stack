using System.Collections.Generic;
using BotuaGetFriendTimes.Helpers;

namespace BotuaGetFriendTimes.Models
{
    public class SortedDataObject
    {
        public IEnumerable<TimeItem> IsMutedItems { get; private set; }
        public IEnumerable<TimeItem> IsDeafenedItems { get; private set; }
        public IEnumerable<TimeItem> IsAfkItems { get; private set; }
        public IEnumerable<TimeItem> IsStreamingItems { get; private set; }
        public IEnumerable<TimeItem> IsVideoOnItems { get; private set; }
        public IEnumerable<TimeItem> ActiveTimeItems { get; private set; }


        public SortedDataObject(IEnumerable<TimeItem> mutedItems, IEnumerable<TimeItem> deafenedItems,
            IEnumerable<TimeItem> afkItems, IEnumerable<TimeItem> streamingItems, IEnumerable<TimeItem> videoItems, IEnumerable<TimeItem> activeItems)
        {
            IsMutedItems = mutedItems;
            IsDeafenedItems = deafenedItems;
            IsAfkItems = afkItems;
            IsStreamingItems = streamingItems;
            IsVideoOnItems = videoItems;
            ActiveTimeItems = activeItems;
        }
    }
}