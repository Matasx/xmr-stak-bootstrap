using System.Collections.Generic;
using System.Runtime.Serialization;

namespace xmr_stak_bootstrap.Model
{
    [DataContract]
    public class PoolConfiguration
    {
        [DataMember(Name = "pools", EmitDefaultValue = true, IsRequired = true, Order = 1)]
        public IDictionary<string, PoolEntry> Pools { get; set; }

        [DataMember(Name = "pool_sets", EmitDefaultValue = true, IsRequired = true, Order = 2)]
        public IDictionary<string, IList<string>> PoolSets { get; set; }
    }
}