using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using BotuaGetFriendTimes.Helpers;
using BotuaGetFriendTimes.Models;
using BotuaGetFriendTimes.Repositories;

namespace BotuaGetFriendTimes
{
    public class Handler
    {
        const string AchievementImageFolderUrl = "https://generic-images.s3.eu-west-2.amazonaws.com/achievement-images";

        private readonly ITimeRepository _timeRepository;
        private readonly INameHelper _nameHelper;

        public Handler(ITimeRepository timeRepository, INameHelper nameHelper)
        {
            _timeRepository = timeRepository;
            // _ssm = ssm;
            _nameHelper = nameHelper;
        }
        
        public async Task<APIGatewayProxyResponse> Handle(APIGatewayProxyRequest input)
        {
            if (!input.MultiValueQueryStringParameters.TryGetValue("championTypes", out var championTypes))
            {
                championTypes = new List<string> {"isActive", "isDeafened", "isMuted", "isStreaming", "isAfk", "isVideoOn", "isReliable"};
            }

            var originalDays = int.Parse(input.QueryStringParameters["days"]);
            var days = originalDays - 1;
            
            Console.WriteLine($"Original days {originalDays}");
            Console.WriteLine($"New days {days}");

            var millis = days * TimeHelper.DayLengthMillis;

            var currentMillis = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            var endTime = TimeHelper.GetEndOfDayMillis(TimeHelper.ConvertToDateTime(currentMillis - TimeHelper.DayLengthMillis));
            var startTime = TimeHelper.GetStartOfDayMillis(TimeHelper.ConvertToDateTime(endTime - millis));

            Console.WriteLine($"Start time {startTime}");
            Console.WriteLine($"End time {endTime}");
            
            var duration = "day";

            var dateData = TimeHelper.ConvertToDateStringRange(startTime, endTime);
            
            var barDateLabels = new List<string>();
            
            foreach (var dateInstance in dateData)
            {
                barDateLabels.Add(dateInstance.DateString);
                Console.WriteLine(dateInstance.DateString);
            }

            var rawTimeScan = (await _timeRepository.GetTimeByTimeRange(startTime, endTime)).ToList();
            
            var sortedData = new DataBuilder()
                .WithIsActiveItems(rawTimeScan.Where(x => !x.IsAfk && !x.IsDeafened && !x.IsMuted))
                .WithIsMutedItems(rawTimeScan.Where(x => x.IsMuted && !x.IsDeafened))
                .WithIsDeafenedItems(rawTimeScan.Where(x => x.IsDeafened))
                .WithIsAfkItems(rawTimeScan.Where(x => x.IsAfk))
                .WithIsStreamingItems(rawTimeScan.Where(x => x.IsStreaming))
                .WithIsVideoOnItems(rawTimeScan.Where(x => x.IsVideoOn))
                .Build();

            List<Champion> championsList = new List<Champion>();


            var pieDataDict = new Dictionary<string, List<BarDataset>>();
            var barDataDict = new Dictionary<string, List<BarDataset>>();

            foreach (var championType in championTypes)
            {
                var (unsortedBarDataset, unsortedPieDataset) = ProcessDatasets(championType, sortedData, dateData);
                var pieDataset = unsortedPieDataset.OrderByDescending(x => x.Data[0]).ToList();
                var barDataset = unsortedBarDataset.OrderByDescending(x => x.Data[0]).ToList();
                
                if (championType == "isReliable")
                    pieDataset = pieDataset.OrderBy(x => x.DaysActive).ThenBy(x => x.Data[0]).ToList();

                if (pieDataset.Any())
                {
                    for (var i = 0; i < Math.Max(pieDataset.Count, 3); i++)
                    {
                        championsList.Add(new ChampionBuilder(originalDays)
                            .WithName(pieDataset[i].Label)
                            .WithColor(pieDataset[i].BackgroundColor)
                            .WithType(championType)
                            .WithTimeActive(pieDataset[i].Data[0])
                            .WithDaysActive(pieDataset[i].DaysActive)
                            .Build());
                    }
                }
                
                barDataDict.Add(championType, barDataset);
                pieDataDict.Add(championType, pieDataset);
            }
            
            var activeBarData = barDataDict["isActive"];
            
            
            // ToDo - build a faster way to calculate - get all the time differences in millis for each session for each user
            // ToDo - calculate the total time in millis
            // ToDo - see who has the largest number of millis
            // ToDo - convert total time to hours
            // ToDo - put the data into a champion


            var barData = new BarData(barDateLabels, activeBarData);

            var pieData = GeneratePieData(pieDataDict["isActive"]);
            var pieChart = new PieChart(pieData, new PieOptions(new PiePlugin(new PieDataLabels(false))));
            var barGraph = new BarGraph(barData);
            
            var charts = new Response(barGraph, pieChart, null, new Champion("", "", 0D), championsList);
            
            var serialisedData = JsonSerializer.Serialize(charts);
            
            // ToDo - work out the streak
            // ToDo - work out the average
            
            return new APIGatewayProxyResponse
            {
                StatusCode = 200,
                Headers = new Dictionary<string, string> {{"Content-Type", "application/json"}},
                Body = serialisedData
            };
        }

        private List<BarDataset> CalculateIsReliableStat(List<BarDataset> activeBarData)
        {
            // ToDo - this should go somewhere else to make it cleaner
            var bestDays = 0;
            var selectedName = "";
            var selectedColor = "";
            var timeOnline = 0D;
            
            foreach (var item in activeBarData)
            {
                var tempDays = item.Data.Count(x => x > 0);
                Console.WriteLine($"{item.Label} data:");
                foreach(var thing in item.Data)
                    Console.WriteLine(thing);
                
                if (tempDays > bestDays)
                {
                    bestDays = tempDays;
                    selectedName = item.Label;
                    selectedColor = item.BackgroundColor;
                    timeOnline = item.Data.Sum();
                }
                else if (tempDays == bestDays)
                {
                    var tempTime = item.Data.Sum();

                    if (tempTime <= timeOnline) 
                        continue;
                    
                    timeOnline = tempTime;
                    bestDays = tempDays;
                    selectedName = item.Label;
                    selectedColor = item.BackgroundColor;
                }
            }
            
            return new List<BarDataset>
            {
                new BarDataset(selectedName, new List<double> {bestDays}, selectedColor, "isReliable")    
            };
        }

        private (List<BarDataset>, List<BarDataset>) ProcessDatasets(string selectedStat, SortedDataObject sortedData, List<DateData> dateData)
        {
            var specificTimeScan = DataForSelectedStat(selectedStat, sortedData).ToList();
            var timesTuple = GetTimesForData(specificTimeScan);
            return ProcessGraphData(GetUniqueUserIds(specificTimeScan), timesTuple.Item1, timesTuple.Item2, dateData, selectedStat);
        }

        private Champion GenerateChampion(List<BarDataset> data, int originalDays)
        {
            if (data.Any())
            {
                var orderedActivePieData = data.OrderByDescending(x => x.Data[0]).ToList();

                var name = orderedActivePieData[0].Label;
                var time = orderedActivePieData[0].Data[0];
                var title = $"{name} is the champion :crown:";
                var description = $"The most active user for the previous {originalDays} days was {name} with an active time of {time} hours what a champion!";
                var thumbnailUrl = $"{AchievementImageFolderUrl}/king-medal.png";
                
                return new Champion(name, orderedActivePieData[0].BackgroundColor, time, title, description, thumbnailUrl);
            }

            return null;
        }

        public PieData GeneratePieData(List<BarDataset> preliminaryPieData)
        {
            var pieDataLabels = preliminaryPieData.Select(x => x.Label).ToList();

            var pieDataPoints = new List<double>();
            var pieColors = new List<string>();
            
            preliminaryPieData.ForEach(x =>
            {
                pieDataPoints.Add(x.Data[0]);
                pieColors.Add(x.BackgroundColor);
            });
            
            var pieDataset = new PieDataset(pieDataPoints, pieColors);
            
            return new PieData(pieDataLabels, new List<PieDataset> {pieDataset});
        }

        private (List<BarDataset>, List<BarDataset>) ProcessGraphData(List<long> userIds, Dictionary<long, List<long>> userStartTimeList, Dictionary<long, List<long>> userEndTimeList, List<DateData> dateData, string selectedStat)
        {
            var barDataset = new List<BarDataset>();
            var preliminaryPieData = new List<BarDataset>();

            Console.WriteLine($"Log user ID count: {userIds.Count}");
            
            foreach (var userId in userIds)
            {
                Console.WriteLine(userId);
                var specificUserStartList = userStartTimeList[userId];
                var specificUserEndList = userEndTimeList[userId];

                var firstTime = specificUserStartList[0];
                var lastTime = specificUserEndList[^1];

                // This is the number of days to calculate
                var difference = lastTime - firstTime;

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

                barDataset.Add(new BarDataset(_nameHelper.GetNameById(userId), userTimes, _nameHelper.GetColourById(userId), selectedStat));

                barDataset = barDataset.OrderBy(x => x.Label).ToList();

                preliminaryPieData.Add(new BarDataset(_nameHelper.GetNameById(userId), new List<double> {Math.Round(userTimes.Sum(), 2, MidpointRounding.AwayFromZero)}, _nameHelper.GetColourById(userId), selectedStat));
                preliminaryPieData = preliminaryPieData.OrderBy(x => x.Label).ToList();
            }

            return (barDataset, preliminaryPieData);
        }

        private IEnumerable<TimeItem> DataForSelectedStat(string selectedStat, SortedDataObject sortedData)
        {
            return selectedStat switch
            {
                "isMuted" => sortedData.IsMutedItems,
                "isDeafened" => sortedData.IsDeafenedItems,
                "isAfk" => sortedData.IsAfkItems,
                "isStreaming" => sortedData.IsStreamingItems,
                "isVideoOn" => sortedData.IsVideoOnItems,
                "isActive" => sortedData.ActiveTimeItems,
                _ => sortedData.ActiveTimeItems
            };
        }

        private List<long> GetUniqueUserIds(IEnumerable<TimeItem> timeItems)
        {
            return timeItems.Select(x => x.UserId).Distinct().ToList();
        }

        private (Dictionary<long, List<long>>, Dictionary<long, List<long>>) GetTimesForData(IEnumerable<TimeItem> timeScan)
        {
            var userStartTimeList = new Dictionary<long, List<long>>();
            var userEndTimeList = new Dictionary<long, List<long>>();
            
            foreach (var item in timeScan)
            {
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

            return (userStartTimeList, userEndTimeList);
        }
    }
    
}