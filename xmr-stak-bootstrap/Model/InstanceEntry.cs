using System.Collections.Generic;
using System.Runtime.Serialization;

namespace xmr_stak_bootstrap.Model
{
    [DataContract]
    public class InstanceEntry
    {
        [DataMember(Name = "cpu_profile", EmitDefaultValue = true, IsRequired = true, Order = 10)]
        public string CpuProfile { get; set; }

        [DataMember(Name = "nvidia_profiles", EmitDefaultValue = true, IsRequired = true, Order = 20)]
        public IList<string> NvidiaProfiles { get; set; }

        [DataMember(Name = "amd_profiles", EmitDefaultValue = true, IsRequired = true, Order = 30)]
        public IList<string> AmdProfiles { get; set; }
    }
}