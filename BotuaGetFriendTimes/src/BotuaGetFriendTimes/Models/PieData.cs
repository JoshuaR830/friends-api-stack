using System.Collections;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BotuaGetFriendTimes.Models
{
    public class PieData
    {
        [JsonPropertyName("labels")]
        public List<string> Labels { get; set; }
        
        [JsonPropertyName("datasets")]
        public List<PieDataset> Datasets { get; set; }

        public PieData(List<string> labels, List<PieDataset> datasets)
        {
            Labels = labels;
            Datasets = datasets;
        }
    }
}