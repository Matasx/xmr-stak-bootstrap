using System.Collections.Generic;
using System.Runtime.Serialization;

namespace xmr_stak_bootstrap.Model
{
    [DataContract]
    public class AmdConfiguration
    {
        [DataMember(Name = "amd_profiles", EmitDefaultValue = true, IsRequired = true, Order = 10)]
        public IDictionary<string, IList<AmdThreadEntry>> Profiles { get; set; }
    }
}