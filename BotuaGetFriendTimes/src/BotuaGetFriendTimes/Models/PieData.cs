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

        [JsonPropertyName("options")] 
        public PieOptions Options { get; set; }

        public PieData(List<string> labels, List<PieDataset> datasets, PieOptions options)
        {
            Labels = labels;
            Datasets = datasets;
            Options = options;
        }
    }
}