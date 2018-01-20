using System.Collections.Generic;

namespace XmrStakBootstrap.MasterConfiguration.Model
{
    public static class SampleConfigurationData
    {
        public static IDictionary<string, PoolEntry> Pools => new Dictionary<string, PoolEntry>
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
        };

        public static IDictionary<string, IList<CpuThreadEntry>> CpuProfiles
            => new Dictionary<string, IList<CpuThreadEntry>>
            {
                {
                    "Slow single", new List<CpuThreadEntry>
                    {
                        new CpuThreadEntry {AffineToCpu = 0, LowPowerMode = true}
                    }
                },
                {
                    "Slow any", new List<CpuThreadEntry>
                    {
                        new CpuThreadEntry {AffineToCpu = false, LowPowerMode = true}
                    }
                },
                {
                    "Fast", new List<CpuThreadEntry>
                    {
                        new CpuThreadEntry {AffineToCpu = 0, LowPowerMode = false},
                        new CpuThreadEntry {AffineToCpu = 1, LowPowerMode = false},
                        new CpuThreadEntry {AffineToCpu = 2, LowPowerMode = false},
                        new CpuThreadEntry {AffineToCpu = 3, LowPowerMode = false}
                    }
                }
            };

        public static IDictionary<string, IList<NvidiaThreadEntry>> NvidiaProfiles
            => new Dictionary<string, IList<NvidiaThreadEntry>>
            {
                {
                    "Slow", new List<NvidiaThreadEntry>
                    {
                        new NvidiaThreadEntry
                        {
                            Blocks = 5,
                            Threads = 50,
                            Bsleep = 5,
                            Bfactor = 5,
                            AffineToCpu = 1
                        }
                    }
                },
                {
                    "Fast", new List<NvidiaThreadEntry>
                    {
                        new NvidiaThreadEntry
                        {
                            Blocks = 20,
                            Threads = 60,
                            Bsleep = 50,
                            Bfactor = 14,
                            AffineToCpu = 2
                        }
                    }
                }
            };

        public static IDictionary<string, IList<AmdThreadEntry>> Amdprofiles
            => new Dictionary<string, IList<AmdThreadEntry>>
            {
                {
                    "Slow", new List<AmdThreadEntry>
                    {
                        new AmdThreadEntry {Intensity = 100}
                    }
                },
                {
                    "Fast", new List<AmdThreadEntry>
                    {
                        new AmdThreadEntry(),
                        new AmdThreadEntry {Intensity = 300}
                    }
                }
            };

        public static IDictionary<string, HardwareEntry> Hardware => new Dictionary<string, HardwareEntry>
        {
            {
                "CPU", new HardwareEntry {Type = "cpu"}
            },
            {
                "nVidia 1060", new HardwareEntry {Type = "nvidia"}
            },
            {
                "nVidia 750 Ti", new HardwareEntry {Type = "nvidia", Index = 1}
            },
            {
                "AMD Rx 470", new HardwareEntry {Type = "amd"}
            }
        };

        public static IDictionary<string, IDictionary<string, string>> WorkloadProfiles
            => new Dictionary<string, IDictionary<string, string>>
            {
                {
                    "Low performance", new Dictionary<string, string>
                    {
                        {"CPU", "Slow single"},
                        {"nVidia 1060", "Slow"},
                        {"nVidia 750 Ti", "Slow"},
                        {"AMD Rx 470", "Slow"}
                    }
                },
                {
                    "Max performance", new Dictionary<string, string>
                    {
                        {"CPU", "Fast"},
                        {"nVidia 1060", "Fast"},
                        {"nVidia 750 Ti", "Fast"},
                        {"AMD Rx 470", "Fast"}
                    }
                }
            };

        public static IDictionary<string, IList<SolutionEntry>> SolutionProfiles
            => new Dictionary<string, IList<SolutionEntry>>
            {
                {
                    "SUMO CPU", new List<SolutionEntry>
                    {
                        new SolutionEntry
                        {
                            Pools = new List<string> {"SUMO","DCY","ETN"},
                            Hardware = new List<string> {"CPU"}
                        }
                    }
                },
                {
                    "ETN+DCY", new List<SolutionEntry>
                    {
                        new SolutionEntry
                        {
                            Pools = new List<string> {"ETN", "SUMO"},
                            Hardware = new List<string> {"CPU", "nVidia 1060"}
                        },
                        new SolutionEntry
                        {
                            Pools = new List<string> {"DCY", "SUMO"},
                            Hardware = new List<string> {"nVidia 750 Ti", "AMD Rx 470"}
                        }
                    }
                },
                {
                    "DCY All", new List<SolutionEntry>
                    {
                        new SolutionEntry
                        {
                            Pools = new List<string> {"DCY", "SUMO"},
                            Hardware = new List<string> {"CPU", "nVidia 1060", "nVidia 750 Ti", "AMD Rx 470"}
                        }
                    }
                }
            };

        public static MasterConfigurationModel MasterConfigurationModel => new MasterConfigurationModel
        {
            Pools = Pools,
            CpuProfiles = CpuProfiles,
            NvidiaProfiles = NvidiaProfiles,
            AmdProfiles = Amdprofiles,
            Hardware = Hardware,
            WorkloadProfiles = WorkloadProfiles,
            SolutionProfiles = SolutionProfiles,
            PathsConfiguration = new PathsConfiguration()
        };
    }
}