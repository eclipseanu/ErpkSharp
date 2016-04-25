using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;

namespace Erpk.Json
{
    /// <summary>
    ///     Converts any object to its binary representation using BinaryFormatter class.
    /// </summary>
    public class BinaryFormatterConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, value);
                writer.WriteValue(stream.ToArray());
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            switch (reader.TokenType)
            {
                case JsonToken.Null:
                    return null;
                case JsonToken.String:
                    // current token is already at base64 string
                    // unable to call ReadAsBytes so do it the old fashion way
                    var encodedData = reader.Value.ToString();
                    var data = Convert.FromBase64String(encodedData);
                    using (var stream = new MemoryStream(data))
                    {
                        var formatter = new BinaryFormatter();
                        return formatter.Deserialize(stream);
                    }
            }

            throw new JsonSerializationException("Unexpected token when parsing binary.");
        }

        public override bool CanConvert(Type objectType) => true;
    }
}