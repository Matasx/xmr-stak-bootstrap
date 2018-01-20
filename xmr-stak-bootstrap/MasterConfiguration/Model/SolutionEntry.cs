using System.Collections.Generic;
using System.Runtime.Serialization;

namespace XmrStakBootstrap.MasterConfiguration.Model
{
    [DataContract]
    public class SolutionEntry
    {
        [DataMember(Name = "hardware", EmitDefaultValue = true, IsRequired = true, Order = 10)]
        public IList<string> Hardware { get; set; }

        [DataMember(Name = "pools", EmitDefaultValue = true, IsRequired = true, Order = 20)]
        public IList<string> Pools { get; set; }
    }
}