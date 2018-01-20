using System.Collections.Generic;
using System.Runtime.Serialization;

namespace XmrStakBootstrap.MasterConfiguration.Model
{
    [DataContract]
    public class CpuConfiguration
    {
        [DataMember(Name = "cpu_profiles", EmitDefaultValue = true, IsRequired = true, Order = 10)]
        public IDictionary<string, IList<CpuThreadEntry>> Profiles { get; set; }
    }
}