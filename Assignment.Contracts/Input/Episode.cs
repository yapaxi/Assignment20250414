using System.Text.Json.Serialization;

namespace Assignment.Contracts.Input;

public class Episode
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("airDate")]
    public DateTime? AirDate { get; set; }
}
