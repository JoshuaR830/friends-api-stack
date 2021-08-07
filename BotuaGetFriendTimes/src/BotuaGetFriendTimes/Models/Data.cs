using System.Collections;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BotuaGetFriendTimes.Models
{
    public class Data
    {
        [JsonPropertyName("labels")]
        public List<string> Labels { get; set; }
        
        [JsonPropertyName("datasets")]
        public List<Dataset> Datasets { get; set; }

        public Data(List<string> labels, List<Dataset> datasets)
        {
            Labels = labels;
            Datasets = datasets;
        }
    }
}