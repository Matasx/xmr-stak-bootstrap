using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace XmrStakBootstrap.MasterConfiguration.Model
{
    [DataContract]
    public class AmdThreadEntry
    {
        [DataMember(Name = "affine_to_cpu", EmitDefaultValue = true, IsRequired = true, Order = 20)]
        [JsonConverter(typeof(AffinityConverter))]
        public object AffineToCpu { get; set; }

        [DataMember(Name = "intensity", EmitDefaultValue = true, IsRequired = true, Order = 30)]
        public int Intensity { get; set; }

        [DataMember(Name = "worksize", EmitDefaultValue = true, IsRequired = true, Order = 40)]
        public int Worksize { get; set; }

        [DataMember(Name = "strided_index", EmitDefaultValue = true, IsRequired = true, Order = 50)]
        public bool StridedIndex { get; set; }

        [DataMember(Name = "mem_chunk", EmitDefaultValue = true, IsRequired = true, Order = 60)]
        public int MemChunk { get; set; }

        [DataMember(Name = "comp_mode", EmitDefaultValue = true, IsRequired = true, Order = 70)]
        public bool CompMode { get; set; }

        public AmdThreadEntry()
        {
            Intensity = 1000;
            Worksize = 8;
            AffineToCpu = false;
            StridedIndex = true;
            MemChunk = 2;
            CompMode = true;
        }
    }
}