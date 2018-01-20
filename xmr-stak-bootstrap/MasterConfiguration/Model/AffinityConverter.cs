using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace XmrStakBootstrap.MasterConfiguration.Model
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

            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (token.Type)
            {
                case JTokenType.Boolean:
                    return token.ToObject<bool>();
                case JTokenType.Integer:
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