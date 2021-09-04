using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BotuaGetFriendTimes.Models
{
    public class PieDataset
    { 
        [JsonPropertyName("data")] public List<double> Data { get; set; }

        [JsonPropertyName("backgroundColor")] public List<string> BackgroundColor { get; set; }

        public PieDataset(List<double> data, List<string> backgroundColor)
        {
            Data = data;
            BackgroundColor = backgroundColor;
        }
    }
}