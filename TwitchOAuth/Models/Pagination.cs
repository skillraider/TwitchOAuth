using System.Text.Json.Serialization;

namespace TwitchOAuth;

public class Pagination
{
    [JsonPropertyName("cursor")]
    public string Cursor { get; set; } = "";
}