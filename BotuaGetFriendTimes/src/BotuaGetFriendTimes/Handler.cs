﻿using System;
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
        private readonly INameHelper _nameHelper;

        public Handler(ITimeRepository timeRepository, INameHelper nameHelper)
        {
            _timeRepository = timeRepository;
            // _ssm = ssm;
            _nameHelper = nameHelper;
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

            var dateData = TimeHelper.ConvertToDateStringRange(startTime, endTime);
            
            var barDateLabels = new List<string>();
            
            foreach (var dateInstance in dateData)
            {
                barDateLabels.Add(dateInstance.DateString);
                Console.WriteLine(dateInstance.DateString);
            }

            var rawTimeScan = (await _timeRepository.GetTimeByTimeRange(startTime, endTime)).ToList();

            // ToDo - problem - can be muted and streaming - how do you decide what to show - needs to be separate graphs
            // ToDo - keep the graphs separate
            var sortedData = new DataBuilder()
                .WithIsActiveItems(rawTimeScan.Where(x => !x.IsAfk && !x.IsDeafened && !x.IsMuted))
                .WithIsMutedItems(rawTimeScan.Where(x => x.IsMuted))
                .WithIsDeafenedItems(rawTimeScan.Where(x => x.IsDeafened))
                .WithIsAfkItems(rawTimeScan.Where(x => x.IsAfk))
                .WithIsStreamingItems(rawTimeScan.Where(x => x.IsStreaming))
                .WithIsVideoOnItems(rawTimeScan.Where(x => x.IsVideoOn))
                .Build();

            var selectedStat = "isActive";
            var isMutedTimeScan = DataForSelectedStat("isMuted", sortedData).ToList();
            var isDeafenedTimeScan = DataForSelectedStat("isDeafened", sortedData).ToList();
            var isAfkTimeScan = DataForSelectedStat("isAfk", sortedData).ToList();
            var isStreamingTimeScan = DataForSelectedStat("isStreaming", sortedData).ToList();
            var isVideoOnTimeScan = DataForSelectedStat("isVideoOn", sortedData).ToList();
            var isActiveTimeScan = DataForSelectedStat("isActive", sortedData).ToList();
            
            var mutedTimesTuple = GetTimesForData(isMutedTimeScan);
            var deafenedTimesTuple = GetTimesForData(isDeafenedTimeScan);
            var afkTimesTuple = GetTimesForData(isAfkTimeScan);
            var streamingTimesTuple = GetTimesForData(isStreamingTimeScan);
            var videoTimesTuple = GetTimesForData(isVideoOnTimeScan);
            var activeTimesTuple = GetTimesForData(isActiveTimeScan);
            // var userStartTimeList = tuple.Item1;
            // var userEndTimeList = tuple.Item2;

            var userIds = GetUniqueUserIds(isActiveTimeScan);

            Console.WriteLine($"There are {userIds.Count} user Id's");

            var mutedDataTuple = ProcessGraphData(GetUniqueUserIds(isMutedTimeScan), mutedTimesTuple.Item1, mutedTimesTuple.Item2, dateData);
            var deafenedDataTuple = ProcessGraphData(GetUniqueUserIds(isDeafenedTimeScan), deafenedTimesTuple.Item1, deafenedTimesTuple.Item2, dateData);
            var afkDataTuple = ProcessGraphData(GetUniqueUserIds(isAfkTimeScan), afkTimesTuple.Item1, afkTimesTuple.Item2, dateData);
            var streamingDataTuple = ProcessGraphData(GetUniqueUserIds(isStreamingTimeScan), streamingTimesTuple.Item1, streamingTimesTuple.Item2, dateData);
            var videoDataTuple = ProcessGraphData(GetUniqueUserIds(isVideoOnTimeScan), videoTimesTuple.Item1, videoTimesTuple.Item2, dateData);
            var activeDataTuple = ProcessGraphData(GetUniqueUserIds(isActiveTimeScan), activeTimesTuple.Item1, activeTimesTuple.Item2, dateData);

            var barData = new BarData(barDateLabels, activeDataTuple.Item1);

            var pieData = GeneratePieData(activeDataTuple.Item2);
            var pieChart = new PieChart(pieData, new PieOptions(new PiePlugin(new PieDataLabels(false))));
            var barGraph = new BarGraph(barData);


            var orderedMutedPieData = mutedDataTuple.Item2.OrderByDescending(x => x.Data[0]).ToList();
            var orderedDeafenedPieData = deafenedDataTuple.Item2.OrderByDescending(x => x.Data[0]).ToList();
            var orderedAfkPieData = afkDataTuple.Item2.OrderByDescending(x => x.Data[0]).ToList();
            var orderedStreamingPieData = streamingDataTuple.Item2.OrderByDescending(x => x.Data[0]).ToList();
            var orderedVideoPieData = videoDataTuple.Item2.OrderByDescending(x => x.Data[0]).ToList();
            var orderedActivePieData = activeDataTuple.Item2.OrderByDescending(x => x.Data[0]).ToList();

            // ToDo - build a faster way to calculate - get all the time differences in millis for each session for each user
            // ToDo - calculate the total time in millis
            // ToDo - see who has the largest number of millis
            // ToDo - convert total time to hours
            // ToDo - put the data into a champion
            var activeChampion = new Champion(orderedActivePieData[0].Label, orderedActivePieData[0].BackgroundColor, orderedActivePieData[0].Data[0]);
            var mutedChampion = new Champion(orderedMutedPieData[0].Label, orderedMutedPieData[0].BackgroundColor, orderedMutedPieData[0].Data[0]);
            var deafenedChampion = new Champion(orderedDeafenedPieData[0].Label, orderedDeafenedPieData[0].BackgroundColor, orderedDeafenedPieData[0].Data[0]);
            var afkChampion = new Champion(orderedAfkPieData[0].Label, orderedAfkPieData[0].BackgroundColor, orderedAfkPieData[0].Data[0]);
            var streamingChampion = new Champion(orderedStreamingPieData[0].Label, orderedStreamingPieData[0].BackgroundColor, orderedStreamingPieData[0].Data[0]);
            var videoChampion = new Champion(orderedVideoPieData[0].Label, orderedVideoPieData[0].BackgroundColor, orderedVideoPieData[0].Data[0]);

            var champions = new Champions(activeChampion, mutedChampion, deafenedChampion, afkChampion, streamingChampion, videoChampion);
            
            var charts = new Response(barGraph, pieChart, champions, activeChampion);
            
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

        private (List<BarDataset>, List<BarDataset>) ProcessGraphData(List<long> userIds, Dictionary<long, List<long>> userStartTimeList, Dictionary<long, List<long>> userEndTimeList, List<DateData> dateData)
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

                barDataset.Add(new BarDataset(_nameHelper.GetNameById(userId), userTimes, _nameHelper.GetColourById(userId)));

                barDataset = barDataset.OrderBy(x => x.Label).ToList();

                preliminaryPieData.Add(new BarDataset(_nameHelper.GetNameById(userId), new List<double> {Math.Round(userTimes.Sum(), 2, MidpointRounding.AwayFromZero)}, _nameHelper.GetColourById(userId)));
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