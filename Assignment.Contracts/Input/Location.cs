using System.Text.Json.Serialization;

namespace Assignment.Contracts.Input;

public class Location
{
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("type")]
    public string? Type { get; init; }

    [JsonPropertyName("dimension")]
    public string? Dimension { get; init; }
}