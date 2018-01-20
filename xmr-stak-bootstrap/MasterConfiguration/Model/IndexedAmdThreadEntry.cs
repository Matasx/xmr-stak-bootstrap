using System.Runtime.Serialization;

namespace XmrStakBootstrap.MasterConfiguration.Model
{
    [DataContract]
    public class IndexedAmdThreadEntry : AmdThreadEntry
    {
        [DataMember(Name = "index", EmitDefaultValue = true, IsRequired = true, Order = 10)]
        public int Index { get; set; }
    }
}