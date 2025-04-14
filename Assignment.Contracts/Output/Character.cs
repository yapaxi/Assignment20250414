using System.Text.Json.Serialization;

namespace Assignment.Contracts.Output
{
    public class Character
    {
        [JsonPropertyName("id")]
        public required int Id { get; init; }

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
        public Location? Origin { get; init; }

        [JsonPropertyName("location")]
        public Location? Location { get; init; }

        [JsonPropertyName("episodes")]
        public IReadOnlyList<Episode>? Episodes { get; init; }
    }
}
