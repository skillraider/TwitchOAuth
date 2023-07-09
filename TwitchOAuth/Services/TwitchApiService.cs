using System.Text.Json;

namespace TwitchOAuth;

public class TwitchApiService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;

    private readonly string _clientId;

    public TwitchApiService(HttpClient httpClient, IConfiguration config)
    {
        _config = config;

        _clientId = _config.GetSection("ClientId").Value ?? "";

        _httpClient = httpClient;
        _httpClient.BaseAddress = new("https://api.twitch.tv/");
        _httpClient.DefaultRequestHeaders.Add("Client-Id", _clientId);
    }

    public async Task<TwitchUser> GetUserAsync(string token)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new("Bearer", token);
        HttpResponseMessage response = await _httpClient.GetAsync($"helix/users");
        if (response.IsSuccessStatusCode)
        {
            string content = await response.Content.ReadAsStringAsync();
            TwitchApiRepsonse<TwitchUser>? twitchApiRepsonse = JsonSerializer.Deserialize<TwitchApiRepsonse<TwitchUser>>(content);
            if (twitchApiRepsonse is not null)
            {
                return twitchApiRepsonse.Data[0];
            }
        }

        return new();
    }

    public async Task<List<Following>> GetFollowedChannelsAsync(string token, string userId)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new("Bearer", token);

        bool firstRun = true;
        string paging = "";
        List<Following> follows = new();

        while (firstRun || !string.IsNullOrEmpty(paging))
        {
            HttpResponseMessage response;
            if (firstRun)
            {
                response = await _httpClient.GetAsync($"/helix/users/follows?from_id={userId}");
                firstRun = false;
            }
            else
            {
                response = await _httpClient.GetAsync($"/helix/users/follows?from_id={userId}&after={paging}");
            }

            TwitchApiRepsonse<Following>? following = null;
            if (response.IsSuccessStatusCode)
            {
                string body = await response.Content.ReadAsStringAsync();
                following = JsonSerializer.Deserialize<TwitchApiRepsonse<Following>>(body);
                if (following is not null)
                {
                    follows.AddRange(following.Data);
                    paging = following.Pagination.Cursor;
                }
            }
        }

        return follows;
    }

    public async Task<List<TwitchStream>> GetFollowedStreamsAsync(string token, string userId)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new("Bearer", token);

        bool firstRun = true;
        string paging = "";
        List<TwitchStream> follows = new();

        while (firstRun || !string.IsNullOrEmpty(paging))
        {
            HttpResponseMessage response;
            if (firstRun)
            {
                response = await _httpClient.GetAsync($"/helix/streams/followed?user_id={userId}");
                firstRun = false;
            }
            else
            {
                response = await _httpClient.GetAsync($"/helix/streams/followed?user_id={userId}&after={paging}");
            }

            TwitchApiRepsonse<TwitchStream>? following = null;
            if (response.IsSuccessStatusCode)
            {
                string body = await response.Content.ReadAsStringAsync();
                following = JsonSerializer.Deserialize<TwitchApiRepsonse<TwitchStream>>(body);
                if (following is not null)
                {
                    follows.AddRange(following.Data);
                    paging = following.Pagination.Cursor;
                }
            }
        }

        return follows;
    }
}