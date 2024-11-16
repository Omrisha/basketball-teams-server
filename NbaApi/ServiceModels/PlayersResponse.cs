using System.Text.Json.Serialization;

namespace NbaApi.ServiceModels;

public class PlayersResponse
{
    [JsonPropertyName("response")]
    public List<PlayerData> Response { get; set; }
}