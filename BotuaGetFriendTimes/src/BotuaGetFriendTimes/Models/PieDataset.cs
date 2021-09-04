using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BotuaGetFriendTimes.Models
{
    public class PieDataset : IDataset
    {
        [JsonPropertyName("label")] public string Label { get; set; }

        [JsonPropertyName("data")] public List<double> Data { get; set; }

        [JsonPropertyName("backgroundColor")] public List<string> BackgroundColor { get; set; }

        public PieDataset(string label, List<double> data, List<string> backgroundColor)
        {
            Label = label;
            Data = data;
            BackgroundColor = backgroundColor;
        }
    }
}