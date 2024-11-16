using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using NbaApi.Models;
using NbaApi.ServiceModels;

namespace NbaApi.Services;

public class BasketballService : IBasketballService
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly IConfiguration _configuration;
    private readonly IDistributedCache _cache;
    private const string CacheKey = "Team_";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(30);
    
    public BasketballService(IHttpClientFactory clientFactory, IConfiguration configuration, IDistributedCache cache)
    {
        _clientFactory = clientFactory;
        _configuration = configuration;
        _cache = cache;
    }

    public async Task<List<Team?>> GetTeamsWithOrderedPlayers()
    {
        var client = _clientFactory.CreateClient();
        client.DefaultRequestHeaders.Add("X-RapidAPI-Key", _configuration["RapidApiKey"]);
        client.DefaultRequestHeaders.Add("X-RapidAPI-Host", "api-nba-v1.p.rapidapi.com");
        
        var teamsResponse = await client.GetAsync("https://api-nba-v1.p.rapidapi.com/teams");
        teamsResponse.EnsureSuccessStatusCode();
        var teamsContent = await teamsResponse.Content.ReadAsStringAsync();
        var teamsData = JsonSerializer.Deserialize<TeamsResponse>(teamsContent);

        var teams = new List<Team?>();

        foreach (var teamData in teamsData.Response) // Limiting to 5 teams for brevity
        {
            var team = await GetTeamById(client, teamData);

            teams.Add(team);
        }

        return teams;
    }

    private async Task<Team?> GetTeamById(HttpClient client, TeamData teamData)
    {
        string cacheKey = $"{CacheKey}{teamData.Id}";

        string? cachedTeam = await _cache.GetStringAsync(cacheKey);
        if (!string.IsNullOrEmpty(cachedTeam))
        {
            return JsonSerializer.Deserialize<Team>(cachedTeam);
        }
        
        var playersResponse = await client.GetAsync($"https://api-nba-v1.p.rapidapi.com/players?team={teamData.Id}&season=2021");
        playersResponse.EnsureSuccessStatusCode();
        var playersContent = await playersResponse.Content.ReadAsStringAsync();
        var playersData = JsonSerializer.Deserialize<PlayersResponse>(playersContent);

        var players = playersData?.Response.Select(p => new Player
        {
            Id = p.Id,
            FirstName = p.FirstName,
            LastName = p.LastName,
            JerseyNumber = p.Leagues.StandardData?.Jersey ?? 0,
            Active = p.Leagues.StandardData?.Active ?? false,
            Position = p.Leagues.StandardData?.Position ?? string.Empty,
        }).OrderBy(p => p.LastName).ThenBy(p => p.FirstName).ToList();

        Team team = new Team
        {
            Id = teamData.Id,
            Name = teamData.Name,
            Players = players ?? new List<Player>(),
        };

        var cacheEntryOptions = new DistributedCacheEntryOptions()
            .SetSlidingExpiration(CacheDuration);

        await _cache.SetStringAsync(
            cacheKey,
            JsonSerializer.Serialize(team),
            cacheEntryOptions);

        return team;
    }

    private List<Team> GenerateMockData()
    {
        return new List<Team>
        {
            new Team
            {
                Id = 1,
                Name = "Los Angeles Lakers",
                Players = new List<Player>
                {
                    new Player { Id = 1, FirstName = "LeBron", LastName = "James", Position = "Forward" },
                    new Player { Id = 2, FirstName = "Anthony", LastName = "Davis", Position = "Forward-Center" },
                    new Player { Id = 3, FirstName = "Russell", LastName = "Westbrook", Position = "Guard" },
                }
            },
            new Team
            {
                Id = 2,
                Name = "Golden State Warriors",
                Players = new List<Player>
                {
                    new Player { Id = 4, FirstName = "Stephen", LastName = "Curry", Position = "Guard" },
                    new Player { Id = 5, FirstName = "Klay", LastName = "Thompson", Position = "Guard" },
                    new Player { Id = 6, FirstName = "Draymond", LastName = "Green", Position = "Forward" },
                }
            },
            new Team
            {
                Id = 3,
                Name = "Brooklyn Nets",
                Players = new List<Player>
                {
                    new Player { Id = 7, FirstName = "Kevin", LastName = "Durant", Position = "Forward" },
                    new Player { Id = 8, FirstName = "Kyrie", LastName = "Irving", Position = "Guard" },
                    new Player { Id = 9, FirstName = "Ben", LastName = "Simmons", Position = "Guard-Forward" },
                }
            }
        };
    }
}