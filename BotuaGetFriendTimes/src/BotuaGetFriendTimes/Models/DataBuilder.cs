using System.Collections.Generic;
using System.Linq;

namespace BotuaGetFriendTimes.Models
{
    public class DataBuilder
    {
        private List<TimeItem> IsMutedItems { get; set; }
        private List<TimeItem> IsDeafenedItems { get; set; }
        private List<TimeItem> IsAfkItems { get; set; }
        private List<TimeItem> IsStreamingItems { get; set; }
        private List<TimeItem> IsVideoOnItems { get; set; }
        public List<TimeItem> ActiveTimeItems { get; set; }

        public DataBuilder WithIsMutedItems(IEnumerable<TimeItem> mutedItems)
        {
            IsMutedItems = mutedItems.OrderBy(x => x.StartTimestamp).ToList();
            return this;
        }
        
        public DataBuilder WithIsDeafenedItems(IEnumerable<TimeItem> deafenedItems)
        {
            IsDeafenedItems = deafenedItems.OrderBy(x => x.StartTimestamp).ToList();
            return this;
        }
        
        public DataBuilder WithIsAfkItems(IEnumerable<TimeItem> afkItems)
        {
            IsAfkItems = afkItems.OrderBy(x => x.StartTimestamp).ToList();
            return this;
        }
        
        public DataBuilder WithIsStreamingItems(IEnumerable<TimeItem> streamingItems)
        {
            IsStreamingItems = streamingItems.OrderBy(x => x.StartTimestamp).ToList();
            return this;
        }
        
        public DataBuilder WithIsVideoOnItems(IEnumerable<TimeItem> videoItems)
        {
            IsVideoOnItems = videoItems.OrderBy(x => x.StartTimestamp).ToList();
            return this;
        }

        public DataBuilder WithIsActiveItems(IEnumerable<TimeItem> activeItems)
        {
            ActiveTimeItems = activeItems.OrderBy(x => x.StartTimestamp).ToList();
            return this;
        }
        
        public SortedDataObject Build()
        {
            return new SortedDataObject(IsMutedItems, IsDeafenedItems, IsAfkItems, IsStreamingItems, IsVideoOnItems, ActiveTimeItems);
        }
    }
}