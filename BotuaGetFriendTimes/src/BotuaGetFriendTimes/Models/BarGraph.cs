using System.Text.Json.Serialization;

namespace BotuaGetFriendTimes.Models
{
    public class BarGraph
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }
        
        [JsonPropertyName("data")]
        public BarData Data { get; set; }

        public BarGraph(BarData data)
        {
            Type = "bar";
            Data = data;
        }
    }
}