using System.Text.Json.Serialization;

namespace NbaApi.ServiceModels;

public class TeamsResponse
{
    [JsonPropertyName("response")]
    public List<TeamData> Response { get; set; }
}