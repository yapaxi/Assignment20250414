using System.Text.Json.Serialization;

namespace Assignment.Contracts.Output
{
    public class Location
    {
        [JsonPropertyName("id")]
        public required int Id { get; set; }

        [JsonPropertyName("name")]
        public required string Name { get; init; }

        [JsonPropertyName("type")]
        public string? Type { get; init; }

        [JsonPropertyName("dimension")]
        public string? Dimension { get; init; }
    }
}
