using System.Text.Json.Serialization;

namespace NbaApi.ServiceModels;

public class TeamData
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("name")]
    public string Name { get; set; }
    [JsonPropertyName("logo")]
    public string Logo { get; set; }
}