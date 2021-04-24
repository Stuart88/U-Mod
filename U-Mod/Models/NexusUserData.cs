using System.Text.Json.Serialization;

namespace U_Mod.Models
{
    /// <summary>
    /// Data object for response from https://api.nexusmods.com/v1/users/validate.json
    /// </summary>
    public class NexusUserData
    {
        [JsonPropertyName("user_id")]
        public long  UserId{ get; set; }
        
        [JsonPropertyName("key")]
        public string Key { get; set; }
        
        [JsonPropertyName("name")]
        public string Name { get; set; }
        
        [JsonPropertyName("is_premium?")]
        public bool IsPremium { get; set; }
        
        [JsonPropertyName("is_supporter?")]
        public bool IsSupporter { get; set; }
        
        [JsonPropertyName("email")]
        public string Email { get; set; }
        
        [JsonPropertyName("profile_url")]
        public string ProfileUrl { get; set; }
        
        [JsonPropertyName("is_supporter")]
        public bool IsSupporter2 { get; set; }
        
        [JsonPropertyName("is_premium")]
        public bool IsPremium2 { get; set; }
    }
}
