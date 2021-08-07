using System.Collections.Generic;
using System.Reflection.Emit;

namespace BotuaGetFriendTimes.Models
{
    public class Dataset
    {
        public string Label { get; set; }
        public List<double> Data { get; set; }

        public Dataset(string label, List<double> data)
        {
            Label = label;
            Data = data;
        }
    }
}