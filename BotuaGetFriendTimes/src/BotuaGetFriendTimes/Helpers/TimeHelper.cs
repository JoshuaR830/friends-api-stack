using System;
using System.Collections.Generic;
using BotuaGetFriendTimes.Models;
using Microsoft.VisualBasic.CompilerServices;

namespace BotuaGetFriendTimes.Helpers
{
    public static class TimeHelper
    {
        public const long DayLengthMillis = 86400000;

        public static long GetStartOfDayMillis(DateTime timeToConvert)
        {
            var startOfDay = new DateTime(timeToConvert.Year, timeToConvert.Month, timeToConvert.Day, 0, 0, 0, 0);
            return ((DateTimeOffset) startOfDay).ToUnixTimeMilliseconds();
        }
        
        public static long GetEndOfDayMillis(DateTime timeToConvert)
        {
            var endOfDay = new DateTime(timeToConvert.Year, timeToConvert.Month, timeToConvert.Day, 23, 59, 59, 999);
            return ((DateTimeOffset) endOfDay).ToUnixTimeMilliseconds();
        }

        // Convert from a timestamp to a DateTime 
        public static DateTime ConvertToDateTime(long timestamp)
        {
            var initialTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            return initialTime.AddMilliseconds(timestamp).ToLocalTime();
        }

        // Convert from a date time object to a string in day/month format
        public static string ConvertDateTimeToDateString(DateTime dateTime)
        {
            var day = dateTime.Day;
            var month = dateTime.Month;

            return $"{day}/{month}";
        }

        // Convert from a time in dd/mm/yy to a unix timestamp at the beginning of the day
        public static long ConvertDateStringToUnix(string dateString)
        {
            var dateParts = dateString.Split("-");

            if (dateParts.Length < 3)
                return -1;

            if (int.TryParse(dateParts[0], out var day) && int.TryParse(dateParts[1], out var month) && int.TryParse(dateParts[2], out var year))
            {
                var dateSelected = new DateTime(year, month, day, 0, 0, 0, 0, System.DateTimeKind.Utc);
                return ((DateTimeOffset) dateSelected).ToUnixTimeMilliseconds();
            }

            return -1;
        }

        // Get a list of day names and individual timestamps between a start and end timestamp
        public static List<DateData> ConvertToDateStringRange(long startTime, long endTime)
        {
            var currentTime = startTime;

            var dateStringRange = new List<DateData>(); 
            
            while (currentTime < endTime)
            {
                var time = ConvertToDateTime(currentTime);
                dateStringRange.Add(new DateData(currentTime, ConvertDateTimeToDateString(time)));
                currentTime += DayLengthMillis;
            }

            return dateStringRange;
        }
    }
}