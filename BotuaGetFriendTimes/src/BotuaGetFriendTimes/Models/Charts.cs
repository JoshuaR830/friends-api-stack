using System.Text.Json.Serialization;
using ThirdParty.Json.LitJson;

namespace BotuaGetFriendTimes.Models
{
    public class Charts
    {
        [JsonPropertyName("barGraph")]
        public BarData BarBarGraph { get; set; }
        
        [JsonPropertyName("pieChart")]
        public PieChart PieBarChart { get; set; }
     
        public Charts(BarData barBarData, PieChart pieBarData)
        {
            BarBarGraph = barBarData;
            PieBarChart = pieBarData;
        }
    }
}