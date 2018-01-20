using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace XmrStakBootstrap.MasterConfiguration.Model
{
    [DataContract]
    public class NvidiaThreadEntry
    {
        [DataMember(Name = "affine_to_cpu", EmitDefaultValue = true, IsRequired = true, Order = 20)]
        [JsonConverter(typeof(AffinityConverter))]
        public object AffineToCpu { get; set; }

        [DataMember(Name = "sync_mode", EmitDefaultValue = true, IsRequired = true, Order = 30)]
        public int SyncMode { get; set; }

        [DataMember(Name = "threads", EmitDefaultValue = true, IsRequired = true, Order = 40)]
        public int Threads { get; set; }

        [DataMember(Name = "blocks", EmitDefaultValue = true, IsRequired = true, Order = 50)]
        public int Blocks { get; set; }

        [DataMember(Name = "bfactor", EmitDefaultValue = true, IsRequired = true, Order = 60)]
        public int Bfactor { get; set; }

        [DataMember(Name = "bsleep", EmitDefaultValue = true, IsRequired = true, Order = 70)]
        public int Bsleep { get; set; }

        public NvidiaThreadEntry()
        {
            AffineToCpu = false;
            SyncMode = 3;
            Threads = 10;
            Blocks = 5;
            Bfactor = 8;
            Bsleep = 100;
        }
    }
}