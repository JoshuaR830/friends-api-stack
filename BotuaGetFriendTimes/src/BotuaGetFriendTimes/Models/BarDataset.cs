using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace BotuaGetFriendTimes.Models
{
    public class BarDataset
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
        
        public string SelectedStat { get; set; }
        
        public int DaysActive { get; set; }

        public BarDataset(string label, List<double> data, string color, string selectedStat)
        {
            Label = label;
            Data = data;
            FillColor = color;
            StrokeColor = color;
            BackgroundColor = color;
            SelectedStat = selectedStat;
            DaysActive = data.Count(x => x > 0);
        }

    }
}