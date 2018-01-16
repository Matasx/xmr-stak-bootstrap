using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace xmr_stak_bootstrap.Model
{
    [DataContract]
    public class CpuThreadEntry
    {
        [DataMember(Name = "affine_to_cpu", EmitDefaultValue = true, IsRequired = true, Order = 10)]
        [JsonConverter(typeof(AffinityConverter))]
        public object AffineToCpu { get; set; }

        [DataMember(Name = "low_power_mode", EmitDefaultValue = true, IsRequired = true, Order = 20)]
        public bool LowPowerMode { get; set; }

        [DataMember(Name = "no_prefetch", EmitDefaultValue = true, IsRequired = true, Order = 30)]
        public bool NoPrefetch { get; set; }

        public CpuThreadEntry()
        {
            AffineToCpu = false;
            NoPrefetch = true;
        }
    }
}