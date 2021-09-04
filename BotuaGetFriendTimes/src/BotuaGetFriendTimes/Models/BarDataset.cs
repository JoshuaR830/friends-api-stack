using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BotuaGetFriendTimes.Models
{
    public class BarDataset : IDataset
    {
        [JsonPropertyName("label")]
        public string Label { get; set; }
        
        [JsonPropertyName("data")]
        public List<double> Data { get; set; }
        
        [JsonPropertyName("fillColor")]
        public string FillColor { get; set; }
        
        [JsonPropertyName("strokeColor")]
        public string StrokeColor { get; set; }
        
        [JsonPropertyName("backgroundColor")]
        public string BackgroundColor { get; set; }

        public BarDataset(string label, List<double> data, string color)
        {
            Label = label;
            Data = data;
            FillColor = color;
            StrokeColor = color;
            BackgroundColor = color;
        }
    }
}