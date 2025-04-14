using System.Text.Json.Serialization;

namespace Assignment.LoaderConsole.Clients.Models;

public class Info
{
    [JsonPropertyName("pages")]
    public int Pages { get; init; }
}
