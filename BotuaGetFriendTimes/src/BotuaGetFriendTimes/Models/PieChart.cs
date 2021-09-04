using System.Text.Json.Serialization;

namespace BotuaGetFriendTimes.Models
{
    public class PieChart
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }
        
        [JsonPropertyName("data")]
        public PieData Data { get; set; }
        
        [JsonPropertyName("options")]
        public PieOptions Options { get; set; }

        public PieChart(PieData data, PieOptions options)
        {
            Type = "doughnut";
            Data = data;
            Options = options;
        }
    }
}