using BotuaGetFriendTimes.Helpers;

namespace BotuaGetFriendTimes.Models
{
    public class DateData
    {
        public long StartOfDay { get; set; }
        public long EndOfDay { get; set; }
        public string DateString { get; set; }
        
        public DateData(long currentStartOfDay, string dateString)
        {
            StartOfDay = currentStartOfDay;
            EndOfDay = currentStartOfDay + TimeHelper.DayLengthMillis - 1;
            DateString = dateString;
        }
    }
}