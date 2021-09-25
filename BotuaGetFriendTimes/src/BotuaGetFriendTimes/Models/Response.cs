using System.Collections.Generic;
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
        
        [JsonPropertyName("championsList")]
        public List<Champion> ChampionsList { get; set; }

        public Response(BarGraph barBarData, PieChart pieBarData, Champions champions, Champion champion, List<Champion> championsList)
        {
            BarBarGraph = barBarData;
            PieBarChart = pieBarData;
            Champions = champions;
            Champion = champion;
            ChampionsList = championsList;
        }
    }
}