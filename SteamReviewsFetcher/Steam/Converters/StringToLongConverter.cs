using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SteamReviewsFetcher.Steam.Converters
{
    internal sealed class StringToLongConverter : JsonConverter<long>
    {
        public override long Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                if (long.TryParse(reader.GetString(), out long result))
                {
                    return result;
                }
                else
                {
                    throw new JsonException("Malformed input cannot be converted to long");
                }
            }

            throw new JsonException("Conversion not implemented for type: " + reader.TokenType);
        }

        public override void Write(Utf8JsonWriter writer, long value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
