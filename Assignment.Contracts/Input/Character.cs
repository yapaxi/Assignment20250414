using System.Text.Json.Serialization;

namespace Assignment.Contracts.Input;

public class Character
{
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("status")]
    public required string Status { get; init; }

    [JsonPropertyName("species")]
    public string? Species { get; init; }

    [JsonPropertyName("type")]
    public string? Type { get; init; }

    [JsonPropertyName("gender")]
    public string? Gender { get; init; }

    [JsonPropertyName("knownOriginId")]
    public int? KnownOriginId { get; init; }

    [JsonPropertyName("newOrigin")]
    public Location? NewOrigin { get; init; }

    [JsonPropertyName("knownLocationId")]
    public int? KnownLocationId { get; init; }

    [JsonPropertyName("newLocation")]
    public Location? NewLocation { get; init; }

    [JsonPropertyName("knownEpisodes")]
    public List<int>? KnownEpisodes { get; init; }

    [JsonPropertyName("newEpisodes")]
    public List<Episode>? NewEpisodes { get; init; }
}