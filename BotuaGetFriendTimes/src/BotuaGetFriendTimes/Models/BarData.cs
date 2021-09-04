using System.Collections;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BotuaGetFriendTimes.Models
{
    public class BarData
    {
        [JsonPropertyName("labels")]
        public List<string> Labels { get; set; }
        
        [JsonPropertyName("datasets")]
        public List<BarDataset> Datasets { get; set; }

        public BarData(List<string> labels, List<BarDataset> datasets)
        {
            Labels = labels;
            Datasets = datasets;
        }
    }
}