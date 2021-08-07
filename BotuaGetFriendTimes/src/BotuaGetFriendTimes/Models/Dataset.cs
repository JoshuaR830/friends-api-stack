using System.Collections.Generic;
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

        public Dataset(string label, List<double> data)
        {
            Label = label;
            Data = data;
        }
    }
}