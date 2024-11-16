using System.Text.Json.Serialization;

namespace NbaApi.ServiceModels;

public class PlayerData
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("firstname")]
    public string FirstName { get; set; }
    [JsonPropertyName("lastname")]
    public string LastName { get; set; }
    [JsonPropertyName("leagues")]
    public LeagueData Leagues { get; set; }
}