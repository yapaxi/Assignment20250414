using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Assignment.LoaderConsole.Clients.Models;

public class EpisodeView
{
    private class AirDateConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.Null:
                    return default;
                case JsonTokenType.String:
                    return DateTime.ParseExact(reader.GetString()!, "MMMM d, yyyy", System.Globalization.CultureInfo.InvariantCulture);
                default:
                    throw new Exception("Unexpected token");
            }
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("MMMM d, yyyy"));
        }
    }

    [JsonPropertyName("id")]
    public required int Id { get; set; }

    [JsonPropertyName("episode")]
    public required string Name { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonPropertyName("air_date")]
    [JsonConverter(typeof(AirDateConverter))]
    public DateTime AirDate { get; set; }

    [JsonPropertyName("created")]
    public DateTime Created { get; set; }
}
