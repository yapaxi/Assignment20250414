using System.Text.Json.Serialization;

namespace Assignment.LoaderConsole.Clients.Models;

public class PagedResponse<T>
{
    [JsonPropertyName("info")]
    public required Info Info { get; init; }

    [JsonPropertyName("results")]
    public IReadOnlyList<T>? Results { get; init; }
}
