using System;
using System.Linq;
using Newtonsoft.Json;

namespace Erpk.Json
{
    /// <summary>
    ///     Converts JSON token of any type to its corresponding enum value.
    /// </summary>
    public class EnumConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            var matchingField = objectType
                .GetFields().FirstOrDefault(field => field
                    .GetCustomAttributes(typeof(JsonEnumAttribute), true)
                    .Select(a => (JsonEnumAttribute) a)
                    .Where(a => reader.TokenType == a.TokenType)
                    .Any(a => a.Value.Equals(reader.Value)));

            if (matchingField == null)
            {
                throw new JsonSerializationException("Cannot cast this token to enum value!");
            }

            return matchingField.GetValue(null);
        }

        public override bool CanConvert(Type objectType) => objectType.IsEnum;
    }
}