using System.Runtime.Serialization;

namespace XmrStakBootstrap.MasterConfiguration.Model
{
    [DataContract]
    public class PoolEntry
    {
        [DataMember(Name = "pool_address", EmitDefaultValue = true, IsRequired = true, Order = 10)]
        public string PoolAddress { get; set; }

        [DataMember(Name = "wallet_address", EmitDefaultValue = true, IsRequired = true, Order = 20)]
        public string WalletAddress { get; set; }

        [DataMember(Name = "rig_id", EmitDefaultValue = true, IsRequired = true, Order = 25)]
        public string RigId { get; set; }

        [DataMember(Name = "pool_password", EmitDefaultValue = true, IsRequired = true, Order = 30)]
        public string PoolPassword { get; set; }

        [DataMember(Name = "use_nicehash", EmitDefaultValue = true, IsRequired = true, Order = 40)]
        public bool UseNiceHash { get; set; }

        [DataMember(Name = "use_tls", EmitDefaultValue = true, IsRequired = true, Order = 50)]
        public bool UseTls { get; set; }

        [DataMember(Name = "tls_fingerprint", EmitDefaultValue = true, IsRequired = true, Order = 60)]
        public string TlsFingerprint { get; set; }

        [DataMember(Name = "currency", EmitDefaultValue = false, IsRequired = true, Order = 70)]
        public string Currency { get; set; }

        public PoolEntry()
        {
            PoolAddress = "pool.address.tld:1234";
            PoolPassword = "x";
            TlsFingerprint = string.Empty;
            UseNiceHash = false;
            UseTls = false;
            WalletAddress = "your_wallet_address";
            RigId = string.Empty;
            Currency = "electroneum";
        }
    }
}