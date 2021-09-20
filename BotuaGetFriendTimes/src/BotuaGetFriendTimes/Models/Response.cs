using System.Text.Json.Serialization;
using ThirdParty.Json.LitJson;

namespace BotuaGetFriendTimes.Models
{
    public class Response
    {
        [JsonPropertyName("barGraph")]
        public BarGraph BarBarGraph { get; set; }
        
        [JsonPropertyName("pieChart")]
        public PieChart PieBarChart { get; set; }
        
        [JsonPropertyName("champion")]
        public Champion Champion { get; set; }
        
        [JsonPropertyName("champions")]
        public Champions Champions { get; set; }
     
        public Response(BarGraph barBarData, PieChart pieBarData, Champions champions, Champion champion)
        {
            BarBarGraph = barBarData;
            PieBarChart = pieBarData;
            Champions = champions;
            Champion = champion;
        }
    }
}