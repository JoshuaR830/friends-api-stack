using System.Text.Json.Serialization;
using ThirdParty.Json.LitJson;

namespace BotuaGetFriendTimes.Models
{
    public class Charts
    {
        [JsonPropertyName("barGraph")]
        public Data BarGraph { get; set; }
        
        [JsonPropertyName("pieChart")]
        public Data PieChart { get; set; }
     
        public Charts(Data barData, Data pieData)
        {
            BarGraph = barData;
            PieChart = pieData;
        }
    }
}