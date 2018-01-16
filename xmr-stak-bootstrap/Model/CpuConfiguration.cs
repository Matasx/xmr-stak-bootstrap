using System.Collections.Generic;
using System.Runtime.Serialization;

namespace xmr_stak_bootstrap.Model
{
    [DataContract]
    public class CpuConfiguration
    {
        [DataMember(Name = "cpu_profiles", EmitDefaultValue = true, IsRequired = true, Order = 10)]
        public IDictionary<string, IList<CpuThreadEntry>> Profiles { get; set; }
    }
}