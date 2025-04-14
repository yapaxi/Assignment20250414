using System.Text.Json.Serialization;

namespace Assignment.LoaderConsole.Clients.Models;

public class LocationReferenceView
{
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("url")]
    public string? Url { get; init; }
}