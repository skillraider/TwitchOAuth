using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace TwitchOAuth;

public class HomeController : Controller
{
    private readonly IConfiguration _config;
    private readonly HttpClient _httpClient;
    private readonly TwitchApiService _twitchApiService;

    private readonly string _clientId;
    private readonly string _clientSecret;
    private readonly string _redirectUri;

    public HomeController(IConfiguration config, HttpClient httpClient, TwitchApiService twitchApiService)
    {
        _config = config;
        _httpClient = httpClient;
        _twitchApiService = twitchApiService;

        _clientId = _config.GetSection("ClientId").Value ?? "";
        _clientSecret = _config.GetSection("ClientSecret").Value ?? "";
        _redirectUri = _config.GetSection("RedirectUri").Value ?? "";
    }

    public IActionResult Index()
    {
        IndexModel indexModel = new()
        {
            TwitchUsername = "",
            ClientId = _clientId,
            RedirectUrl = _redirectUri,
            Scope = "user:read:follows"
        };

        KeyValuePair<string, string> twitchUsernameCookie = Request.Cookies.FirstOrDefault(x => x.Key == "TwitchUserName");
        if (!string.IsNullOrEmpty(twitchUsernameCookie.Key))
        {
            indexModel.TwitchUsername = twitchUsernameCookie.Value;
        }

        return View(indexModel);
    }

    public async Task<IActionResult> SignIn(string code)
    {
        _httpClient.BaseAddress = new Uri("https://id.twitch.tv/");

        Dictionary<string, string> ke = new Dictionary<string, string>()
        {
            {"client_id", _clientId},
            {"client_secret", _clientSecret},
            {"code", code},
            {"grant_type", "authorization_code"},
            {"redirect_uri", _redirectUri}
        };

        FormUrlEncodedContent content = new(ke);
        HttpResponseMessage response = await _httpClient.PostAsync("oauth2/token", content);
        if (response.IsSuccessStatusCode)
        {
            string body = await response.Content.ReadAsStringAsync();
            OAuthResponse? oauthResponse = JsonSerializer.Deserialize<OAuthResponse>(body);
            if (oauthResponse is not null)
            {
                Response.Cookies.Append("TwitchAccessToken", oauthResponse.AccessToken);
                TwitchUser twitchUser = await _twitchApiService.GetUserAsync(oauthResponse.AccessToken);
                if (!string.IsNullOrEmpty(twitchUser.Id))
                {
                    Response.Cookies.Append("TwitchUserId", twitchUser.Id);
                    Response.Cookies.Append("TwitchUserName", twitchUser.DisplayName);
                }
            }
        }
        else
        {
            Console.WriteLine(response.StatusCode);
        }

        return Redirect("~/");
    }

    public IActionResult Signout()
    {
        Response.Cookies.Delete("TwitchAccessToken");
        Response.Cookies.Delete("TwitchUserId");
        Response.Cookies.Delete("TwitchUserName");
        return Redirect("~/");
    }
}