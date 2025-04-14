using System.Text.Json.Serialization;

namespace Assignment.LoaderConsole.Clients.Models;

public class CharacterView
{
    [JsonPropertyName("id")]
    public int Id { get; init; }

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

    [JsonPropertyName("origin")]
    public LocationReferenceView? Origin { get; init; }

    [JsonPropertyName("location")]
    public LocationReferenceView? Location { get; init; }

    [JsonPropertyName("image")]
    public string? Image { get; init; }

    [JsonPropertyName("episode")]
    public List<string>? Episode { get; init; }

    [JsonPropertyName("url")]
    public string? Url { get; init; }

    [JsonPropertyName("created")]
    public DateTime Created { get; init; }
}
