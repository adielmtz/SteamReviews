using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SteamReviewsFetcher.Steam.Converters
{
    internal sealed class NumberToBoolConverter : JsonConverter<bool>
    {
        public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.True || reader.TokenType == JsonTokenType.False)
            {
                return reader.GetBoolean();
            }

            if (reader.TokenType == JsonTokenType.Number)
            {
                return reader.GetInt32() != 0;
            }

            throw new JsonException("Conversion not implemented for type: " + reader.TokenType);
        }

        public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
