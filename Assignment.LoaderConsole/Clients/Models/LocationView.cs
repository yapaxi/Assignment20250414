using System.Text.Json.Serialization;

namespace Assignment.LoaderConsole.Clients.Models;

public class LocationView
{
    [JsonPropertyName("id")]
    public int Id { get; init; }

    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("type")]
    public string? Type { get; init; }

    [JsonPropertyName("dimension")]
    public string? Dimension { get; init; }

    [JsonPropertyName("url")]
    public string? Url { get; init; }

    [JsonPropertyName("created")]
    public DateTime Created { get; init; }
}
