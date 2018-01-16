using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace xmr_stak_bootstrap.Model
{
    public class AffinityConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(bool) || objectType == typeof(int);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var token = JToken.Load(reader);
            if (token.Type == JTokenType.Boolean)
            {
                return token.ToObject<bool>();
            }
            if (token.Type == JTokenType.Integer)
            {
                return token.ToObject<int>();
            }
            return null;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }
}