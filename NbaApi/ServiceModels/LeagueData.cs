using System.Text.Json.Serialization;

namespace NbaApi.ServiceModels;

public class LeagueData
{
    [JsonPropertyName("standard")]
    public StandardData StandardData { get; set; }
}