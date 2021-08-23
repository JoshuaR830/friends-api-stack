using System.Collections.Generic;
using System.Drawing;
using System.Reflection.Emit;
using System.Text.Json.Serialization;

namespace BotuaGetFriendTimes.Models
{
    public class Dataset
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

        public Dataset(string label, List<double> data, string color)
        {
            Label = label;
            Data = data;
            FillColor = color;
            StrokeColor = color;
            BackgroundColor = color;
        }
    }
}