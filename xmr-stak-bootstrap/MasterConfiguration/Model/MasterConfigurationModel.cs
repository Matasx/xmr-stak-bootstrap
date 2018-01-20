using System.Collections.Generic;
using System.Runtime.Serialization;

namespace XmrStakBootstrap.MasterConfiguration.Model
{
    [DataContract]
    public class MasterConfigurationModel
    {
        [DataMember(Name = "paths", EmitDefaultValue = true, IsRequired = true, Order = 10)]
        public PathsConfiguration PathsConfiguration { get; set; }

        [DataMember(Name = "pools", EmitDefaultValue = true, IsRequired = true, Order = 20)]
        public IDictionary<string, PoolEntry> Pools { get; set; }

        [DataMember(Name = "cpu_profiles", EmitDefaultValue = true, IsRequired = true, Order = 30)]
        public IDictionary<string, IList<CpuThreadEntry>> CpuProfiles { get; set; }

        [DataMember(Name = "nvidia_profiles", EmitDefaultValue = true, IsRequired = true, Order = 40)]
        public IDictionary<string, IList<NvidiaThreadEntry>> NvidiaProfiles { get; set; }

        [DataMember(Name = "amd_profiles", EmitDefaultValue = true, IsRequired = true, Order = 50)]
        public IDictionary<string, IList<AmdThreadEntry>> AmdProfiles { get; set; }

        [DataMember(Name = "hardware", EmitDefaultValue = true, IsRequired = true, Order = 60)]
        public IDictionary<string, HardwareEntry> Hardware { get; set; }

        [DataMember(Name = "workload_profiles", EmitDefaultValue = true, IsRequired = true, Order = 70)]
        public IDictionary<string, IDictionary<string, string>> WorkloadProfiles { get; set; }

        [DataMember(Name = "solutions", EmitDefaultValue = true, IsRequired = true, Order = 80)]
        public IDictionary<string, IList<SolutionEntry>> SolutionProfiles { get; set; }
    }
}