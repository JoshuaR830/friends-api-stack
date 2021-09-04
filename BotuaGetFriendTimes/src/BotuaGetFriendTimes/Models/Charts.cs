using System.Text.Json.Serialization;
using ThirdParty.Json.LitJson;

namespace BotuaGetFriendTimes.Models
{
    public class Charts
    {
        [JsonPropertyName("barGraph")]
        public BarData BarBarGraph { get; set; }
        
        [JsonPropertyName("pieChart")]
        public PieData PieBarChart { get; set; }
     
        public Charts(BarData barBarData, PieData pieBarData)
        {
            BarBarGraph = barBarData;
            PieBarChart = pieBarData;
        }
    }
}