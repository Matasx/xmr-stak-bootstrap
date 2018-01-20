using System.Collections.Generic;
using System.Runtime.Serialization;

namespace XmrStakBootstrap.MasterConfiguration.Model
{
    [DataContract]
    public class NvidiaConfiguration
    {
        [DataMember(Name = "nvidia_profiles", EmitDefaultValue = true, IsRequired = true, Order = 10)]
        public IDictionary<string, IList<NvidiaThreadEntry>> Profiles { get; set; }
    }
}