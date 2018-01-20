using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace XmrStakBootstrap.MasterConfiguration.Model
{
    [DataContract]
    public class HardwareEntry
    {
        [DataMember(Name = "type", EmitDefaultValue = true, IsRequired = true, Order = 10)]
        [JsonConverter(typeof(HardwareTypeConverter))]
        public string Type { get; set; }

        [DataMember(Name = "index", EmitDefaultValue = false, IsRequired = false, Order = 20)]
        public int Index { get; set; }
    }
}