using System.Text.Json.Serialization;

namespace NbaApi.ServiceModels;

public class StandardData
{
    [JsonPropertyName("jersey")]
    public int? Jersey { get; set; }
    [JsonPropertyName("active")]
    public bool Active { get; set; }
    [JsonPropertyName("pos")]
    public string Position { get; set; }
}