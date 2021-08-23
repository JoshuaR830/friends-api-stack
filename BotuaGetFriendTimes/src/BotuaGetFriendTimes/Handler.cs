using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Schema;
using Amazon.Lambda.APIGatewayEvents;
using BotuaGetFriendTimes.Helpers;
using BotuaGetFriendTimes.Models;
using BotuaGetFriendTimes.Repositories;

namespace BotuaGetFriendTimes
{
    public class Handler
    {
        private readonly ITimeRepository _timeRepository;
    
        public Handler(ITimeRepository timeRepository)
        {
            _timeRepository = timeRepository;
        }
        
        public async Task<APIGatewayProxyResponse> Handle(APIGatewayProxyRequest input)
        {
            var days = int.Parse(input.QueryStringParameters["days"]);
            days--;

            var millis = days * TimeHelper.DayLengthMillis;

            var currentMillis = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            var endTime = TimeHelper.GetEndOfDayMillis(TimeHelper.ConvertToDateTime(currentMillis - TimeHelper.DayLengthMillis));
            var startTime = TimeHelper.GetStartOfDayMillis(TimeHelper.ConvertToDateTime(endTime - millis));

            Console.WriteLine($"Start time {startTime}");
            Console.WriteLine($"End time {endTime}");
            
            var duration = "day";

            // var startTimeUnix = TimeHelper.ConvertDateStringToUnix(startTime);
            // var endTimeUnix = TimeHelper.ConvertDateStringToUnix(endTime);

            // ToDo: get the data from the DB between startTimeUnix and endTimeUnix
            
            var dateData = TimeHelper.ConvertToDateStringRange(startTime, endTime);
            
            // ToDo: Sort the date into daily chunks that can be used (between start and end times)

            var dateLabels = new List<string>();
            
            foreach (var dateInstance in dateData)
            {
                dateLabels.Add(dateInstance.DateString);
                Console.WriteLine(dateInstance.DateString);
            }

            var timeScan = (await _timeRepository.GetTimeByTimeRange(startTime, endTime)).ToList();

            var orderedTimes = timeScan.OrderBy(x => x.StartTimestamp).ToList();

            var userStartTimeList = new Dictionary<long, List<long>>();
            var userEndTimeList = new Dictionary<long, List<long>>();
            
            foreach (var item in timeScan)
            {
                Console.WriteLine(item.SessionId);

                var sessionStarts = new List<long>();
                var sessionEnds = new List<long>();


                var sessionStart = item.StartTimestamp;
                var sessionEnd = item.EndTimestamp;

                var endOfDayMillis = TimeHelper.GetEndOfDayMillis(TimeHelper.ConvertToDateTime(sessionStart));

                // Add the start time
                sessionStarts.Add(sessionStart);

                // Split up a session that could be longer than a day into day sized chunks
                while (sessionEnd > endOfDayMillis)
                {
                    sessionEnds.Add(endOfDayMillis);

                    // Add 1 to make it the next day (midnight)
                    sessionStart = endOfDayMillis + 1;
                    sessionStarts.Add(sessionStart);

                    // Move on to the next day maximum session length
                    endOfDayMillis += TimeHelper.DayLengthMillis;
                }

                // Add the end time
                sessionEnds.Add(sessionEnd);

                // Keep a list of all the session times for users
                if (userStartTimeList.ContainsKey(item.UserId))
                {
                    userStartTimeList[item.UserId].AddRange(sessionStarts);
                }
                else
                {
                    userStartTimeList.Add(item.UserId, sessionStarts);
                }

                if (userEndTimeList.ContainsKey(item.UserId))
                {
                    userEndTimeList[item.UserId].AddRange(sessionEnds);
                }
                else
                {
                    userEndTimeList.Add(item.UserId, sessionEnds);
                }
            }

            var userIds = timeScan.Select(x => x.UserId).Distinct().ToList();

            Console.WriteLine($"There are {userIds.Count} user Id's");
            var dataset = new List<Dataset>();

            foreach (var userId in userIds)
            {
                var specificUserStartList = userStartTimeList[userId];
                var specificUserEndList = userEndTimeList[userId];

                var firstTime = specificUserStartList[0];
                var lastTime = specificUserEndList[^1];

                // This is the number of days to calculate
                var difference = lastTime - firstTime;

                // Todo - loop through everything in those time ranges
                // ToDo - need to know the differences between them
                // ToDo: Need to be able to collate the time and convert to hours

                var userTimes = new List<double>(); 
                
                foreach (var dataPoint in dateData)
                {
                    var specificStartDay = dataPoint.StartOfDay;
                    var specificEndDay = dataPoint.EndOfDay;
                    
                    // Get today's start and end times
                    var startTimesForToday = specificUserStartList.Where(x => x <= specificEndDay && x >= specificStartDay).ToList();
                    var endTimesForToday = specificUserEndList.Where(x => x <= specificEndDay && x >= specificStartDay).ToList();

                    Console.WriteLine($"Start: {startTimesForToday.Count}, End: {endTimesForToday.Count} Equal: {startTimesForToday.Count == endTimesForToday.Count}");

                    if (startTimesForToday.Count == endTimesForToday.Count)
                    {
                        var hoursOnline = 0d;
                        
                        for (int i = 0; i < startTimesForToday.Count; i++)
                        {
                            // Work out how many hours for each session
                            var sessionTimeMillis = endTimesForToday[i] - startTimesForToday[i];
                            
                            // Add up the hours online for if multiple sessions in a day
                            hoursOnline += TimeHelper.ConvertToHours(sessionTimeMillis);
                            
                        }
                        
                        Console.WriteLine($"Hours online {hoursOnline} stored");
                        userTimes.Add(hoursOnline);
                    }
                }
                
                dataset.Add(new Dataset(userId.ToString(), userTimes));
            }

            Data data = new Data(
                dateLabels,
                dataset
            );

            var serialisedData = JsonSerializer.Serialize(data);
            
            return new APIGatewayProxyResponse
            {
                StatusCode = 200,
                Headers = new Dictionary<string, string> {{"Content-Type", "application/json"}},
                Body = serialisedData
            };
        }
    }
}