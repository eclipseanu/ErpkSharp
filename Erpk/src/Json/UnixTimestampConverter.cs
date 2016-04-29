using System;
using Newtonsoft.Json;

namespace Erpk.Json
{
    /// <summary>
    ///     Converts Unix seconds to DateTimeOffset.
    /// </summary>
    public class UnixTimestampConverter : JsonConverter
    {
        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            long timestamp;

            switch (reader.TokenType)
            {
                case JsonToken.Null:
                    return null;
                case JsonToken.String:
                    timestamp = long.Parse((string) reader.Value);
                    break;
                case JsonToken.Integer:
                    timestamp = (long) reader.Value;
                    break;
                default:
                    throw new JsonSerializationException("Cannot parse this token as unix timestamp!");
            }

            return DateTimeOffset.FromUnixTimeSeconds(timestamp);
        }

        public override bool CanConvert(Type objectType)
            => objectType == typeof(DateTimeOffset) || objectType == typeof(DateTimeOffset?);
    }
}