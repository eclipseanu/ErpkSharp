using System;
using Newtonsoft.Json;

namespace Erpk.Json
{
    public class JsonEnumAttribute : Attribute
    {
        public JsonEnumAttribute(string value)
        {
            Value = value;
            TokenType = JsonToken.String;
        }

        public JsonEnumAttribute(bool value)
        {
            Value = value;
            TokenType = JsonToken.Boolean;
        }

        public object Value { get; }
        public JsonToken TokenType { get; }
    }
}