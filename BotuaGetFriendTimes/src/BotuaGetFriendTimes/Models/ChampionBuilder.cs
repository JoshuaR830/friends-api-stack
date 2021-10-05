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
        private Dictionary<string, List<string>> _descriptions = new Dictionary<string, List<string>>();
        private Dictionary<string, List<string>> _thumbnails = new Dictionary<string, List<string>>();

        public ChampionBuilder(int days)
        {
            Days = days;
            Position = 0;
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

        public ChampionBuilder WithZeroIndexedPosition(int position)
        {
            Position = position;
            return this;
        }

        private string GetTitle()
        {
            if (!_titles.TryGetValue(Type, out var titles))
                return "";

            return titles[Position];
        }

        private string GetDescription()
        {
            if (!_descriptions.TryGetValue(Type, out var descriptions))
                return "";

            return descriptions[Position];
        }

        private string GetThumbnailUrl()
        {
            var medalColour = (Position == 0 ? "gold" : (Position == 1 ? "silver" : "bronze"));

            return Type switch
            {
                "isMuted" => $"{AchievementImageFolderUrl}/muted-{medalColour}-medal.png",
                "isDeafened" => $"{AchievementImageFolderUrl}/deafened-{medalColour}-medal.png",
                "isAfk" => $"{AchievementImageFolderUrl}/afk-{medalColour}-medal.png",
                "isStreaming" => $"{AchievementImageFolderUrl}/streaming-{medalColour}-medal.png",
                "isVideoOn" => $"{AchievementImageFolderUrl}/video-{medalColour}-medal.png",
                "isReliable" => $"{AchievementImageFolderUrl}/reliable-{medalColour}-medal.png",
                "isActive" => $"{AchievementImageFolderUrl}/champion-{medalColour}-medal.png",
                _ => ""
            };
        }

        private void DefineTitles()
        {
            _titles = new Dictionary<string, List<string>>
            {
                {
                    "isMuted", new List<string>
                    {
                        $"{Name} is the new captain of the muted mutiny :pirate_flag:",
                        $"{Name} is the new young cabin person of the muted mutiny :child:",
                        $"{Name} is a peasantly crew member of the muted mutiny :person_rowing_boat:"
                    }
                },
                {
                    "isDeafened", new List<string>
                    {
                        $"{Name} is the colossal squid, king of the ocean :crown:",
                        $"{Name} is a baby squid :squid:",
                        $"{Name} got inked by a squid :pen_fountain:"
                    }
                },
                {
                    "isAfk", new List<string>
                    {
                        $"{Name} is the heroic leader of the sleeping sloth clan :zzz:",
                        $"{Name} is the light sleeper of the sleeping sloth clan :sleeping:",
                        $"{Name} is the poor, tired watchmen of the sleeping sloth clan :flashlight:"
                    }
                },
                {
                    "isStreaming", new List<string>
                    {
                        $"{Name} is the superior shark :shark:",
                        $"{Name} is the inferior whimpering whale :whale:",
                        $"{Name} is the weak little fish that the superior shark shall devour :fishing_pole_and_fish:"
                    }
                },
                {
                    "isVideoOn", new List<string>
                    {
                        $"Viva la {Name}, viva la vlogger :video_camera:",
                        $"Viva la... oh, it would appear {Name} has lost vlogger viewership :chart_with_downwards_trend:",
                        $"{Name} is the viewerless vlogger, try harder or you'll never vive like a vlogger :ghost:"
                    }
                },
                {
                    "isReliable", new List<string>
                    {
                        $"{Name} is the reliable roman, leader of legions, ROAD BUILDER! No potholes here! :shield:",
                        $"{Name} is the reckless Reliant Robin rider :fire_engine:",
                        $"{Name} is the porcupine at the puncture repair place :pushpin:"
                    }
                },
                {
                    "isActive", new List<string>
                    {
                        $"{Name} is the heroic champion :superhero:",
                        $"{Name} is the princely person :prince:",
                        $"{Name} is a lowly peasant :farmer:",
                    }
                }
            };
        }

        private void DefineDescriptions()
        {
            _descriptions = new Dictionary<string, List<string>>()
            {
                {
                    "isMuted", new List<string>
                    {
                        $"With a muted time of {ActiveTime} hours in the previous {Days} days we had no option but to promote {Name} to the prestigious position of captain of the muted mutiny!",
                        $"With a muted time of {ActiveTime} hours in the previous {Days} cabin person is the only fitting role for {Name} on this epic voyage!",
                        $"With a muted time of {ActiveTime} hours in the previous {Days} days we had no option but to demote {Name} to a peasantly crew member on this soul crushing voyage, but at least your not in the brig - yet!",
                    }
                },
                {
                    "isDeafened", new List<string>
                    {
                        $"{Name}, with a deafened time of {ActiveTime} hours in the previous {Days} days you were deafened so long that like the earless colossal squid, you have crushed your opponents!",
                        $"{Name}, with a deafened time of {ActiveTime} hours in the previous {Days} days you were deafened so long that like a tiny baby squid, you may as well have no ears!",
                        $"{Name}, with a deafened time of {ActiveTime} hours in the previous {Days} days, those ink splatters looks like you've been in the presence of an earless squid, but you're not one of them, I know you heard!",
                    }
                },
                {
                    "isAfk", new List<string>
                    {
                        $"With {ActiveTime} hours away from keyboard in the last {Days} days, there's no question, {Name} is the new leader of the sleeping sloths!",
                        $"With {ActiveTime} hours away from keyboard in the last {Days} days, looks like {Name} is a light sleeper, but one of the sleeping sloths non-the-less!",
                        $"{Name}, with {ActiveTime} hours away from keyboard in the last {Days} days, it's not going to win you any medals, except bronze for that is the medal you won, but everyone needs a watchperson, and it looks like you are the chosen one!",
                    }
                },
                {
                    "isStreaming", new List<string>
                    {
                        $"With a streaming time of {ActiveTime} hours in the previous {Days} days, like the mighty shark, normal streams could not hold your superiority, so {Name} takes the title of Superior Shark - congratulations!",
                        $"With a streaming time of {ActiveTime} hours in the previous {Days} days, like an inferior whimpering whale, {Name} you came in at second for the streamers, letting a shark take the top spot - unbelievable!",
                        $"Quaking in your fin shaped boots, you find yourself nose to nose with the superior shark, with a streaming time of a mere {ActiveTime} hours in the previous {Days} days, {Name} you are about to be devoured by your competitors - unless you can up your game"
                    }
                },
                {
                    "isVideoOn", new List<string>
                    {
                        $"Born to be a vlogger, your time on video of {ActiveTime} hours in the previous {Days} days means {Name} you are closer than anyone else to being a fully fledged vlogger!",
                        $"Wow the viewership is down, with a shoddy video uptime of {ActiveTime} hours in the previous {Days} days {Name} you have lost out on the tp spot!",
                        $"{Name}, it's not looking good for you, with a pathetic video time of {ActiveTime} hours in the previous {Days} days your viewership is basically zero, your vlogging career is toast!"
                    }
                },
                {
                    "isReliable", new List<string>
                    {
                        $"As the romans built solid roads that can be depended on, so {Name} leads this channel, with solid reliability and {DaysActive} days active in the last {Days} days, the might of a Roman emperor is well deserved",
                        $"Alas, {Name} was the second most reliable person in the last {Days} days, with {DaysActive} days active, reliable by name, but less so by nature!",
                        $"Coming in at 3rd, {Name} tries their best, but alas, their reliability is like that of a porcupine at a puncture repair shop, with a mere {DaysActive} days active in the last {Days} days!",
                    }
                },
                {
                    "isActive", new List<string>
                    {
                        $"{Name} our hero, the one who was here for us at all hours as long as it was in the {ActiveTime} hours that you were active in the last {Days} days - you're a champion!",
                        $"The second most active user for the previous {Days} days was {Name} with an active time of {ActiveTime} hours, the slightly inferior title of princely person belongs to you!",
                        $"In third place for active time for the previous {Days} days was {Name}, you got the not at all sought after title of lowly peasant attaining a peasantly active time of {ActiveTime} hours, you need to up your game!",
                    }
                }
            };
        }

        public Champion Build()
        {
            DefineTitles();
            DefineDescriptions();
            return new Champion(Name, Color, ActiveTime, GetTitle(), GetDescription(), GetThumbnailUrl());
        }
    }
}