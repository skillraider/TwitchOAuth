namespace TwitchOAuth;

public class IndexModel
{
    public string TwitchUsername { get; set; } = "";

    public string ClientId { get; set; } = "";

    public string RedirectUrl { get; set; } = "";

    public string Scope { get; set; } = "";
}
