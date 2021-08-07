using System.Collections;
using System.Collections.Generic;

namespace BotuaGetFriendTimes.Models
{
    public class Data
    {
        public List<string> Labels { get; set; }
        public List<Dataset> Datasets { get; set; }

        public Data(List<string> labels, List<Dataset> datasets)
        {
            Labels = labels;
            Datasets = datasets;
        }
    }
}