using System.Collections.Generic;
using System.Runtime.Serialization;

namespace xmr_stak_bootstrap.Model
{
    [DataContract]
    public class InstanceConfiguration
    {
        [DataMember(Name = "instance_profiles", EmitDefaultValue = true, IsRequired = true, Order = 10)]
        public IDictionary<string, InstanceEntry> InstanceProfiles { get; set; }
    }
}