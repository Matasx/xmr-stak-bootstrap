using System.Runtime.Serialization;

namespace XmrStakBootstrap.MasterConfiguration.Model
{
    [DataContract]
    public class PrioritizedPoolEntry : PoolEntry
    {
        [DataMember(Name = "pool_weight", EmitDefaultValue = true, IsRequired = true, Order = 1)]
        public int PoolWeight { get; set; }
    }
}