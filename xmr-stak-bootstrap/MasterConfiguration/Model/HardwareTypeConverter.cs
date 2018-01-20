using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace XmrStakBootstrap.MasterConfiguration.Model
{
    public class HardwareTypeConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var token = JToken.Load(reader);

            if (token.Type == JTokenType.String)
            {
                var value = token.ToObject<string>();
                if (new[] {"cpu", "nvidia", "amd"}.Contains(value))
                {
                    return value;
                }
            }

            throw new InvalidCastException("Hardware type must be string and one of: cpu, nvidia or amd.");
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }
}