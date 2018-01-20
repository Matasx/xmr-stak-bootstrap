using System.Collections.Generic;
using System.Runtime.Serialization;

namespace XmrStakBootstrap.MasterConfiguration.Model
{
    [DataContract]
    public class AmdConfiguration
    {
        [DataMember(Name = "amd_profiles", EmitDefaultValue = true, IsRequired = true, Order = 10)]
        public IDictionary<string, IList<AmdThreadEntry>> Profiles { get; set; }
    }
}