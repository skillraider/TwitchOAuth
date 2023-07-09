using System.Text.Json.Serialization;

namespace TwitchOAuth;

public class TwitchApiRepsonse<T>
{
    [JsonPropertyName("total")]
    public int Total { get; set; }

    [JsonPropertyName("data")]
    public List<T> Data { get; set; } = new();

    [JsonPropertyName("pagination")]
    public Pagination Pagination { get; set; } = new();
}