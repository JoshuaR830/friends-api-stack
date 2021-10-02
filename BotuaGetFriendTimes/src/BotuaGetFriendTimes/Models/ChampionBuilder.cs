using System;

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
        
        public ChampionBuilder(int days)
        {
            Days = days;
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

        private string GetTitle()
        {
            return Type switch
            {
                "isMuted" => $"{Name} is the new captain of the muted mutiny :pirate_flag:",
                "isDeafened" => $"{Name} is the king of the squid people :squid:",
                "isAfk" => $"{Name} is the leader of the sleeping sloths :sloth:",
                "isStreaming" => $"{Name} is the superior shark :shark:",
                "isVideoOn" => $"Viva la {Name}, viva la Vlogger :video_camera:",
                "isReliable" => $"{Name} is the Reckless Reliant Robin Rider :fire_engine:",
                "isActive" => $"{Name} is the champion :crown:",
                _ => ""
            };
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
                "isReliable" => $"The most reliable person in the last {Days} days, with {ActiveTime} days in a channel is {Name}, well done, you are undoubtedly the most reliable person - for now!",
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
                "isReliable" => $"{AchievementImageFolderUrl}/reliant-robin-medal.png",
                "isActive" => $"{AchievementImageFolderUrl}/king-medal.png",
                _ => ""
            };
        }

        public Champion Build()
        {
            return new Champion(Name, Color, ActiveTime, GetTitle(), GetDescription(), GetThumbnailUrl());
        }
    }
}