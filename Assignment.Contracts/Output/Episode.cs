using System.Text.Json.Serialization;

namespace Assignment.Contracts.Output
{
    public class Episode
    {
        [JsonPropertyName("id")]
        public required int Id { get; set; }

        [JsonPropertyName("name")]
        public required string Name { get; set; }

        [JsonPropertyName("airDate")]
        public DateTime? AirDate { get; set; }
    }
}
