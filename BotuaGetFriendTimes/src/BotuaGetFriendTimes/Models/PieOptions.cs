using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BotuaGetFriendTimes.Models
{
    public class PieOptions
    {
        [JsonPropertyName("plugins")]
        public PiePlugin Plugins { get; set; }

        [JsonPropertyName("doughnutlabel")]
        public DoughnutLabel DoughnutLabel { get; set; }

        public PieOptions(PiePlugin plugins, string text)
        {
            Plugins = plugins;
            DoughnutLabel = new DoughnutLabel(text);
        }
    }

    public class DoughnutLabel
    {
        [JsonPropertyName("labels")]
        public List<DoughnutText> Labels { get; set; }

        public DoughnutLabel(string text)
        {
            Labels = new List<DoughnutText>();
            Labels.Add(new DoughnutText(text));
        }
    }

    public class DoughnutText
    {
        [JsonPropertyName("text")]
        public string Text { get; set; }

        public DoughnutText(string text)
        {
            Text = text;
        }
    }
    
    public class PiePlugin
    {
        [JsonPropertyName("datalabels")]
        public PieDataLabels DataLabels { get; set; }
        
        public PiePlugin(PieDataLabels pieDataLabels)
        {
            DataLabels = pieDataLabels;
        }
    }

    public class PieDataLabels
    {
        [JsonPropertyName("display")]
        public bool Display { get; set; }

        public PieDataLabels(bool display)
        {
            Display = display;
        }
    }
}