using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Erpk.Json
{
    /// <summary>
    ///     https://stackoverflow.com/questions/26725138/force-conversion-of-empty-json-array-to-dictionary-type
    /// </summary>
    public class DictionaryOrEmptyArrayConverter<T, F> : JsonConverter
    {
        public override bool CanWrite => false;

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Dictionary<T, F>);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            switch (reader.TokenType)
            {
                case JsonToken.StartArray:
                    reader.Read();
                    if (reader.TokenType == JsonToken.EndArray)
                        return new Dictionary<T, F>();
                    throw new JsonSerializationException("Non-empty JSON array does not make a valid Dictionary!");
                case JsonToken.Null:
                    return null;
                case JsonToken.StartObject:
                    var tw = new StringWriter();
                    var writer = new JsonTextWriter(tw);
                    writer.WriteStartObject();
                    var initialDepth = reader.Depth;
                    while (reader.Read() && reader.Depth > initialDepth)
                    {
                        writer.WriteToken(reader);
                    }
                    writer.WriteEndObject();
                    writer.Flush();
                    return JsonConvert.DeserializeObject<Dictionary<T, F>>(tw.ToString());
                default:
                    throw new JsonSerializationException("Unexpected token!");
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}