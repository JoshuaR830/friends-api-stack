using System;
using System.Collections.Generic;

namespace BotuaGetFriendTimes.Models
{
    public class ChampionBuilder
    {
        const string AchievementImageFolderUrl = "https://generic-images.s3.eu-west-2.amazonaws.com/achievement-images";
        
        public string Name { get; set; }
        public double ActiveTime { get; set; }
        public string Type { get; set; }
        public string Color { get; set; }
        public int Days { get; set; }
        public int DaysActive { get; set; }
        public int Position { get; set; }


        private Dictionary<string, List<string>> _titles = new Dictionary<string, List<string>>();

        public ChampionBuilder(int days)
        {
            Days = days;
            Position = 1;
        }


        public ChampionBuilder WithName(string name)
        {
            Name = name;
            return this;
        }

        public ChampionBuilder WithType(string type)
        {
            Type = type;
            return this;
        }

        public ChampionBuilder WithColor(string color)
        {
            Color = color;
            return this;
        }

        public ChampionBuilder WithTimeActive(double timeActive)
        {
            ActiveTime = timeActive;
            return this;
        }
        
        public ChampionBuilder WithDaysActive(int daysActive)
        {
            DaysActive = daysActive;
            return this;
        }

        private string GetTitle()
        {
            List<string> titles;
            if (!_titles.TryGetValue(Type, out titles))
                return "";

            return titles[Position - 1];
        }

        private string GetDescription()
        {
            return Type switch
            {
                "isMuted" => $"With a muted time of {ActiveTime} hours in the previous {Days} days we had no option but to promote {Name} to the prestigious position of captain of the muted mutiny!",
                "isDeafened" => $"{Name}, with a deafened time of {ActiveTime} hours in the previous {Days} days you were deafened so long that like the squid, you may as well have no ears!",
                "isAfk" => $"With {ActiveTime} hours away from keyboard in the last {Days} days, there's no question, {Name} is the new leader of the sleeping sloths!",
                "isStreaming" => $"With a streaming time of {ActiveTime} hours in the previous {Days} days, like the mighty shark, normal streams could not hold your superiority, so {Name} takes the title of Superior Shark - congratulations!",
                "isVideoOn" => $"Born to be a vlogger, your time on video of {ActiveTime} hours in the previous {Days} days means {Name} you are closer than anyone else to being a fully fledged vlogger!",
                "isReliable" => $"The most reliable person in the last {Days} days, with {DaysActive} days in a channel is {Name}, well done, you are undoubtedly the most reliable person - for now!",
                "isActive" => $"The most active user for the previous {Days} days was {Name} with an active time of {ActiveTime} hours what a champion!",
                _ => ""
            };
        }

        private string GetThumbnailUrl()
        {
            return Type switch
            {
                "isMuted" => $"{AchievementImageFolderUrl}/pirate-medal.png",
                "isDeafened" => $"{AchievementImageFolderUrl}/squid-medal.png",
                "isAfk" => $"{AchievementImageFolderUrl}/sloth-medal.png",
                "isStreaming" => $"{AchievementImageFolderUrl}/shark-medal.png",
                "isVideoOn" => $"{AchievementImageFolderUrl}/video-medal.png",
                "isReliable" => $"{AchievementImageFolderUrl}/reliable-gold-medal.png",
                "isActive" => $"{AchievementImageFolderUrl}/king-medal.png",
                _ => ""
            };
        }

        public Champion Build()
        {
            _titles = new Dictionary<string, List<string>>
            {
                {"isMuted", new List<string>
                {
                    $"{Name} is the new captain of the muted mutiny :pirate_flag:",
                    $"{Name} is the new cabin person of the muted mutiny :pirate_flag:",
                    $"{Name} is a peasantly crew member of the muted mutiny :pirate_flag:"
                }},
                {"isDeafened", new List<string>
                {
                    $"{Name} is the king of the squid people :squid:",
                    $"{Name} is a princely squid person :squid:",
                    $"{Name} is a lowly squid person :squid:"
                }},
                {"isAfk", new List<string>
                {
                    $"{Name} is the heroic leader of the sleeping sloths :sloth:",
                    $"{Name} is the light sleeper of the sleeping sloths :sloth:",
                    $"{Name} is the poor, tired watchmen of the sleeping sloths :sloth:"
                }},
                {"isStreaming", new List<string>
                {
                    $"{Name} is the superior shark :shark:",
                    $"{Name} is the inferior shark :shark:",
                    $"{Name} is the weak little fish that the superior shark shall devour :fishing_pole_and_fish:"
                }},
                {"isVideoOn", new List<string>
                {
                    $"Viva la {Name}, viva la vlogger :video_camera:",
                    $"Viva la... oh, it would appear {Name} has lost vlogger viewership :video_camera:",
                    $"Try harder {Name} or you'll never vive like a vlogger :video_camera:"
                }},
                {"isReliable", new List<string>
                {
                    $"{Name} is the reliable roman, leader of legions, ROAD BUILDER! No potholes here! :shield:",
                    $"{Name} is the reckless Reliant Robin rider :fire_engine:",
                    $"{Name} is the porcupine at the puncture repair place :pushpin:"
                }},
                {"isActive", new List<string>
                {
                    $"{Name} is the champion :crown:",
                    $"{Name} is the princely person :prince:",
                    $"{Name} is a lowly peasant :farmer:",
                }}
            };
            return new Champion(Name, Color, ActiveTime, GetTitle(), GetDescription(), GetThumbnailUrl());
        }
    }
}