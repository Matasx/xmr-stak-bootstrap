using System.Collections.Generic;

namespace xmr_stak_bootstrap.Model
{
    public static class SampleConfigurationData
    {
        public static List<PrioritizedPoolEntry> PrioritizedPoolEntries => new List<PrioritizedPoolEntry>
        {
            new PrioritizedPoolEntry
            {
                PoolAddress = "pool.address.tld:1234",
                PoolPassword = "x",
                PoolWeight = 1,
                TlsFingerprint = string.Empty,
                UseNiceHash = false,
                UseTls = false,
                WalletAddress = "your_wallet_address"
            }
        };

        public static PoolConfiguration PoolConfiguration => new PoolConfiguration
        {
            Pools = new Dictionary<string, PoolEntry>
            {
                {
                    "ETN", new PoolEntry
                    {
                        PoolAddress = "pool.etn.spacepools.org:7777"
                    }
                },
                {
                    "DCY", new PoolEntry
                    {
                        PoolAddress = "poolmining1.dinastycoin.com:5555"
                    }
                },
                {
                    "SUMO", new PoolEntry
                    {
                        PoolAddress = "sumopool.sonofatech.com:3333"
                    }
                }
            },
            PoolSets = new Dictionary<string, IList<string>>
            {
                {"Electroneum", new List<string> {"ETN", "DCY", "SUMO" } },
                {"Dinastycoin", new List<string> {"DCY", "ETN", "SUMO" } },
                {"Sumokoin", new List<string> {"SUMO", "ETN", "DCY" } }
            }
        };

        public static CpuConfiguration CpuConfiguration => new CpuConfiguration
        {
            Profiles = new Dictionary<string, IList<CpuThreadEntry>>
            {
                {"No", null},
                {
                    "Low power @ 0", new List<CpuThreadEntry>
                    {
                        new CpuThreadEntry {AffineToCpu = 0, LowPowerMode = true}
                    }
                },
                {
                    "Low power @ any", new List<CpuThreadEntry>
                    {
                        new CpuThreadEntry {AffineToCpu = false, LowPowerMode = true}
                    }
                },
                {
                    "All cores", new List<CpuThreadEntry>
                    {
                        new CpuThreadEntry {AffineToCpu = 0, LowPowerMode = false},
                        new CpuThreadEntry {AffineToCpu = 1, LowPowerMode = false},
                        new CpuThreadEntry {AffineToCpu = 2, LowPowerMode = false},
                        new CpuThreadEntry {AffineToCpu = 3, LowPowerMode = false}
                    }
                }
            }
        };

        public static NvidiaConfiguration NvidiaConfiguration => new NvidiaConfiguration
        {
            Profiles = new Dictionary<string, IList<NvidiaThreadEntry>>
            {
                {
                    "GeForce GTX 1060 UI", new List<NvidiaThreadEntry>
                    {
                        new NvidiaThreadEntry
                        {
                            Index = 0,
                            Blocks = 20,
                            Threads = 60,
                            Bsleep = 50,
                            Bfactor = 14,
                            AffineToCpu = 2
                        }
                    }
                },
                {
                    "NVidia 750 Ti fast", new List<NvidiaThreadEntry>
                    {
                        new NvidiaThreadEntry
                        {
                            Index = 1,
                            Blocks = 5,
                            Threads = 50,
                            Bsleep = 5,
                            Bfactor = 5,
                            AffineToCpu = 1
                        }
                    }
                }
            }
        };

        public static AmdConfiguration AmdConfiguration => new AmdConfiguration
        {
            Profiles = new Dictionary<string, IList<AmdThreadEntry>>
            {
                {
                    "Radeon fast", new List<AmdThreadEntry>
                    {
                        new AmdThreadEntry {Index = 0},
                        new AmdThreadEntry {Index = 1}
                    }
                }
            }
        };

        public static InstanceConfiguration InstanceConfiguration => new InstanceConfiguration
        {
            InstanceProfiles = new Dictionary<string, InstanceEntry>
            {
                {
                    "CPU low power @ 0",
                    new InstanceEntry
                    {
                        CpuProfile = "Low power @ 0"
                    }
                },
                {
                    "NVidia 750 Ti",
                    new InstanceEntry
                    {
                        NvidiaProfiles = new List<string> { "NVidia 750 Ti fast" }
                    }
                },
                {
                    "NVidia 750 + 1060",
                    new InstanceEntry
                    {
                        NvidiaProfiles = new List<string> { "NVidia 750 Ti fast", "GeForce GTX 1060 UI" }
                    }
                },
                {
                    "All fast",
                    new InstanceEntry
                    {
                        CpuProfile = "All cores",
                        NvidiaProfiles = new List<string> { "NVidia 750 Ti fast", "GeForce GTX 1060 UI" }
                    }
                },
                {
                    "Amd",
                    new InstanceEntry
                    {
                        CpuProfile = "All cores",
                        AmdProfiles = new List<string> { "Radeon fast" }
                    }
                },
            }
        };

        public static SolutionConfiguration SolutionConfiguration => new SolutionConfiguration
        {
            SolutionProfiles = new Dictionary<string, IDictionary<string, IList<string>>>
            {
                {
                    "ETN CPU + SUMO GK", new Dictionary<string, IList<string>>
                    {
                        {"Electroneum", new List<string> {"CPU low power @ 0"}},
                        {"Sumokoin", new List<string> {"NVidia 750 + 1060"}}
                    }
                },
                {
                    "DCY All", new Dictionary<string, IList<string>>
                    {
                        {"Dinastycoin", new List<string> {"All fast"}}
                    }
                }
            }
        };

        public static MasterConfiguration MasterConfiguration => new MasterConfiguration
        {
            PoolConfiguration = PoolConfiguration,
            CpuConfiguration = CpuConfiguration,
            NvidiaConfiguration = NvidiaConfiguration,
            AmdConfiguration = AmdConfiguration,
            InstanceConfiguration = InstanceConfiguration,
            SolutionConfiguration = SolutionConfiguration
        };
    }
}