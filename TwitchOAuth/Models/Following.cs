using System.Text.Json.Serialization;

namespace TwitchOAuth;

public class Following
{
    [JsonPropertyName("from_id")]
    public string FromId { get; set; } = "";

    [JsonPropertyName("from_login")]
    public string FromLogin { get; set; } = "";

    [JsonPropertyName("from_name")]
    public string FromName { get; set; } = "";

    [JsonPropertyName("to_id")]
    public string ToId { get; set; } = "";

    public long ToIdLong { get; set; }

    [JsonPropertyName("to_name")]
    public string ToName { get; set; } = "";

    [JsonPropertyName("followed_at")]
    public DateTimeOffset FollowedAt { get; set; }
}