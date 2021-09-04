using System.Reflection.PortableExecutable;
using System.Text.Json.Serialization;

namespace BotuaGetFriendTimes.Models
{
    public class PieOptions
    {
        [JsonPropertyName("plugins")]
        public PiePlugin Plugins { get; set; }

        public PieOptions(PiePlugin plugins)
        {
            Plugins = plugins;
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