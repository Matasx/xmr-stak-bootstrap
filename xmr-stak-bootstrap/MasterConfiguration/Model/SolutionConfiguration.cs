using System.Collections.Generic;
using System.Runtime.Serialization;

namespace XmrStakBootstrap.MasterConfiguration.Model
{
    [DataContract]
    public class SolutionConfiguration
    {
        [DataMember(Name = "solution_profiles", EmitDefaultValue = true, IsRequired = true, Order = 10)]
        public IDictionary<string, IDictionary<string, IList<string>>> SolutionProfiles { get; set; }
    }
}