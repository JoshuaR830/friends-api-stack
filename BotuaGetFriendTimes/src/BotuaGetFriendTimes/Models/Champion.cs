using System.Text.Json.Serialization;

namespace BotuaGetFriendTimes.Models
{
    public class Champion
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        
        [JsonPropertyName("color")]
        public string Color { get; set; }
        
        [JsonPropertyName("timeActive")]
        public double TimeActive { get; set; }
        
        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("description")] 
        public string Description { get; set; }

        [JsonPropertyName("thumbnailUrl")]
        public string ThumbnailUrl { get; set; }

        public Champion(string name, string color, double timeActive)
        {
            Name = name;
            Color = color;
            TimeActive = timeActive;
        }
        
        public Champion(string name, string color, double timeActive, string title, string description, string thumbnailUrl)
        {
            Name = name;
            Color = color;
            TimeActive = timeActive;
            Title = title;
            Description = description;
            ThumbnailUrl = thumbnailUrl;
        }
    }
}