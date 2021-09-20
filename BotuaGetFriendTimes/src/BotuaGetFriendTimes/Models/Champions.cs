using System.Text.Json.Serialization;

namespace BotuaGetFriendTimes.Models
{
    public class Champions
    {
        [JsonPropertyName("active")]
        public Champion ActiveChampion { get; set; }
        
        [JsonPropertyName("muted")]
        public Champion Muted { get; set; }
        
        [JsonPropertyName("deafened")]
        public Champion Deafened { get; set; }
        
        [JsonPropertyName("afk")]
        public Champion Afk { get; set; }
        
        [JsonPropertyName("streaming")]
        public Champion Streaming { get; set; }
        
        [JsonPropertyName("video")]
        public Champion Video { get; set; }
        
        public Champions(Champion activeChampion, Champion mutedChampion, Champion deafenedChampion, Champion afkChampion, Champion streamingChampion, Champion videoChampion)
        {
            ActiveChampion = activeChampion;
            Muted = mutedChampion;
            Deafened = deafenedChampion;
            Afk = afkChampion;
            Streaming = streamingChampion;
            Video = videoChampion;
        }
    }
}