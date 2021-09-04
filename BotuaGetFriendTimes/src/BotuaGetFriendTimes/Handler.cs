using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using BotuaGetFriendTimes.Helpers;
using BotuaGetFriendTimes.Models;
using BotuaGetFriendTimes.Repositories;

namespace BotuaGetFriendTimes
{
    public class Handler
    {
        private readonly ITimeRepository _timeRepository;
        private readonly IAmazonSimpleSystemsManagement _ssm;
        private long _jordanDiscordId;
        private long _joshuaDiscordId;
        private long _dayleDiscordId;
        private long _madalynDiscordId;
        private long _jonnyDiscordId;
        private long _lucasDiscordId;
        private long _callanDiscordId;
        private long _andrewDiscordId;
        private long _martinDiscordId;

        public Handler(ITimeRepository timeRepository, IAmazonSimpleSystemsManagement ssm)
        {
            _timeRepository = timeRepository;
            _ssm = ssm;
        }
        
        public async Task<APIGatewayProxyResponse> Handle(APIGatewayProxyRequest input)
        {
            _jordanDiscordId = await GetSSMValue("JordanId");
            _joshuaDiscordId = await GetSSMValue("JoshuaId");
            _dayleDiscordId = await GetSSMValue("DayleId");
            _madalynDiscordId = await GetSSMValue("DeclynId");
            _jonnyDiscordId = await GetSSMValue("JonnyId");
            _lucasDiscordId = await GetSSMValue("LucasId");
            _callanDiscordId = await GetSSMValue("CallanId");
            _andrewDiscordId = await GetSSMValue("AndrewId");
            _martinDiscordId = await GetSSMValue("MartinId");
            
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

            var barDateLabels = new List<string>();
            
            foreach (var dateInstance in dateData)
            {
                barDateLabels.Add(dateInstance.DateString);
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
            var barDataset = new List<BarDataset>();
            var preliminaryPieData = new List<BarDataset>();

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
                
                var colors = new Dictionary<long, string>
                {
                    {_jordanDiscordId, "rgba(143, 164, 199, 0.5)"},
                    {_joshuaDiscordId, "rgba(149, 0, 255, 0.5)"},
                    {_dayleDiscordId, "rgba(21, 128, 11, 0.5)"},
                    {_madalynDiscordId, "rgba(238, 255, 0, 0.5)"},
                    {_jonnyDiscordId, "rgba(252, 3, 202, 0.5)"},
                    {_lucasDiscordId, "rgba(158, 14, 14, 0.5)"},
                    {_callanDiscordId, "rgba(255, 111, 0, 0.5)"},
                    {_andrewDiscordId, "rgba(158, 132, 14, 0.5)"},
                    {_martinDiscordId, "rgba(201, 16, 118, 0.5)"}
                };

                barDataset.Add(new BarDataset(GetNameById(userId), userTimes, colors[userId]));

                barDataset = barDataset.OrderBy(x => x.Label).ToList();

                preliminaryPieData.Add(new BarDataset(GetNameById(userId), new List<double> {userTimes.Sum()}, colors[userId]));
                preliminaryPieData = preliminaryPieData.OrderBy(x => x.Label).ToList();
            }

            var barData = new Data(barDateLabels, new List<IDataset>(barDataset));

            var pieDataLabels = preliminaryPieData.Select(x => x.Label).ToList();

            var pieDataPoints = new List<double>();
            var pieColors = new List<string>();
            
            preliminaryPieData.ForEach(x =>
            {
                pieDataPoints.Add(x.Data[0]);
                pieColors.Add(x.BackgroundColor);
            });
            
            var pieDataset = new PieDataset("Collated time", pieDataPoints, pieColors);
            
            var pieData = new Data(pieDataLabels, new List<IDataset> {pieDataset});

            var charts = new Charts(barData, pieData);
            
            var serialisedData = JsonSerializer.Serialize(charts);
            
            // ToDo - Work out the most time
            // ToDo - work out the streak
            // ToDo - work out the average
            
            return new APIGatewayProxyResponse
            {
                StatusCode = 200,
                Headers = new Dictionary<string, string> {{"Content-Type", "application/json"}},
                Body = serialisedData
            };
        }

        private async Task<long> GetSSMValue(string parameterName)
        {
            return Convert.ToInt64((await _ssm.GetParameterAsync(new GetParameterRequest
            {
                Name = parameterName
            })).Parameter.Value);
        }

        private string GetNameById(long id)
        {
            if (id == _jordanDiscordId)
                return "Jordan";
            
            if (id == _joshuaDiscordId)
                return "Joshua";
            
            if (id == _dayleDiscordId)
                return "Dayle";
            
            if (id == _madalynDiscordId)
                return "Maddie";
            
            if (id == _jonnyDiscordId)
                return "Jonny";
            
            if (id == _lucasDiscordId)
                return "Lucas";
            
            if (id == _callanDiscordId)
                return "Callan";
            
            if (id == _andrewDiscordId)
                return "Andrew";
            
            if (id == _martinDiscordId)
                return "Martin";

            return "Other";
        }
    }
}