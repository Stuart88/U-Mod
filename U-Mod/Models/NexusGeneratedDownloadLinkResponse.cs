using System.Text.Json.Serialization;

namespace U_Mod.Models
{
    public class NexusGeneratedDownloadLinkResponse
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("short_name")]
        public string ShortName { get; set; }

        [JsonPropertyName("URI")]
        public string Uri { get; set; }
    }
}
