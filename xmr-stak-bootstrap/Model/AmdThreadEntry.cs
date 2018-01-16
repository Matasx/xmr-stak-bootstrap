using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace xmr_stak_bootstrap.Model
{
    [DataContract]
    public class AmdThreadEntry
    {
        [DataMember(Name = "index", EmitDefaultValue = true, IsRequired = true, Order = 10)]
        public int Index { get; set; }

        [DataMember(Name = "affine_to_cpu", EmitDefaultValue = true, IsRequired = true, Order = 20)]
        [JsonConverter(typeof(AffinityConverter))]
        public object AffineToCpu { get; set; }

        [DataMember(Name = "intensity", EmitDefaultValue = true, IsRequired = true, Order = 30)]
        public int Intensity { get; set; }

        [DataMember(Name = "worksize", EmitDefaultValue = true, IsRequired = true, Order = 40)]
        public int Worksize { get; set; }

        [DataMember(Name = "strided_index", EmitDefaultValue = true, IsRequired = true, Order = 50)]
        public bool StridedIndex { get; set; }

        public AmdThreadEntry()
        {
            Intensity = 1000;
            Worksize = 8;
            AffineToCpu = false;
            StridedIndex = true;
        }
    }
}