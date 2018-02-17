using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Unity.Attributes;
using XmrStakBootstrap.Common;
using XmrStakBootstrap.MasterConfiguration.Model;
using XmrStakBootstrap.RunConfiguration.Model;

namespace XmrStakBootstrap.Core.Job.Miner
{
    public class MinerRunner : IMinerRunner
    {
        [Dependency]
        public IFinalizer Finalizer { get; set; }

        [Dependency]
        public MasterConfigurationModel MasterConfigurationModel { get; set; }

        [Dependency]
        public RunConfigurationModel RunConfigurationModel { get; set; }

        public void Run()
        {
            var solution = MasterConfigurationModel.SolutionProfiles.GetValue(RunConfigurationModel.ActiveSolutionConfiguration);
            var workload = MasterConfigurationModel.WorkloadProfiles.GetValue(RunConfigurationModel.ActiveWorkloadConfiguration);

            var i = 0;
            foreach (var instances in solution)
            {
                i++;
                var outputPools = GetOutputPools(instances.Pools);

                if (outputPools.Count == 0)
                {
                    Console.WriteLine(@"Instance {0}.{1} does not contain any pool bindings.", RunConfigurationModel.ActiveSolutionConfiguration, i);
                    continue;
                }

                var configArgument = GetConfigurationArgument(outputPools);
                var utilizedHardware = instances.Hardware.Select(x => new UtilizedHardware
                {
                    Hardware = MasterConfigurationModel.Hardware.GetValue(x),
                    Profile = workload.GetValue(x)
                }).ToList();

                var cpuArgument = GetCpuArgument(utilizedHardware.Where(x => x.Hardware.Type == "cpu").ToList());
                var amdArgument = GetAmdArgument(utilizedHardware.Where(x => x.Hardware.Type == "amd").ToList());
                var nvidiaArgument = GetNvidiaArgument(utilizedHardware.Where(x => x.Hardware.Type == "nvidia").ToList());

                RunMiner($"{configArgument} {cpuArgument} {amdArgument} {nvidiaArgument}");
            }
        }

        private List<PrioritizedPoolEntry> GetOutputPools(IEnumerable<string> pools)
        {
            return pools
                .Select(x => MasterConfigurationModel.Pools.GetValue(x))
                .Reverse()
                .Select((x, i) => new PrioritizedPoolEntry
                {
                    PoolWeight = i + 1,
                    PoolAddress = x.PoolAddress,
                    PoolPassword = x.PoolPassword,
                    TlsFingerprint = x.TlsFingerprint,
                    WalletAddress = x.WalletAddress,
                    UseNiceHash = x.UseNiceHash,
                    UseTls = x.UseTls
                })
                .ToList();
        }

        private static void RunMiner(string arguments)
        {
            var startInfo = new ProcessStartInfo(Path.GetFullPath("xmr-stak.exe"), arguments)
            {
                WorkingDirectory = Environment.CurrentDirectory
            };

            Console.WriteLine(@"Starting: xmr-stak.exe {0}", arguments);

            Process.Start(startInfo);
        }

        private string GetConfigurationArgument(IReadOnlyCollection<PrioritizedPoolEntry> pools)
        {
            var configPath = CreateTemporaryConfiguration(MasterConfigurationModel.PathsConfiguration.ConfigTemplate, "config", "%POOLS%", pools);
            ScheduleFileDelete(configPath);

            return $"--config \"{configPath}\"";
        }

        private string GetCpuArgument(ICollection<UtilizedHardware> entry)
        {
            if (entry.Count == 0)
            {
                return "--noCPU";
            }

            var cpuProfile = entry.SelectMany(x => MasterConfigurationModel.CpuProfiles.GetValue(x.Profile)).ToList();
            var path = CreateTemporaryConfiguration(MasterConfigurationModel.PathsConfiguration.CpuTemplate, "cpu", "%THREADS%", cpuProfile);
            ScheduleFileDelete(path);

            return $"--cpu \"{path}\"";
        }

        private string GetAmdArgument(ICollection<UtilizedHardware> entry)
        {
            if (entry.Count == 0)
            {
                return "--noAMD";
            }

            var amdProfile = entry
                .SelectMany(
                    x =>
                        MasterConfigurationModel.AmdProfiles.GetValue(x.Profile)
                            .Select(profile => new IndexedAmdThreadEntry
                            {
                                Index = x.Hardware.Index,
                                AffineToCpu = profile.AffineToCpu,
                                Intensity = profile.Intensity,
                                StridedIndex = profile.StridedIndex,
                                Worksize = profile.Worksize
                            }))
                .ToList();

            var path = CreateTemporaryConfiguration(MasterConfigurationModel.PathsConfiguration.AmdTemplate, "amd", "%THREADS%", amdProfile);
            ScheduleFileDelete(path);

            return $"--amd \"{path}\"";
        }

        private string GetNvidiaArgument(ICollection<UtilizedHardware> entry)
        {
            if (entry.Count == 0)
            {
                return "--noNVIDIA";
            }

            var nvidiaProfile = entry
                .SelectMany(
                    x =>
                        MasterConfigurationModel.NvidiaProfiles.GetValue(x.Profile)
                            .Select(profile => new IndexedNvidiaThreadEntry
                            {
                                Index = x.Hardware.Index,
                                AffineToCpu = profile.AffineToCpu,
                                Bfactor = profile.Bfactor,
                                Blocks = profile.Blocks,
                                Bsleep = profile.Bsleep,
                                SyncMode = profile.SyncMode,
                                Threads = profile.Threads
                            }))
                .ToList();

            var path = CreateTemporaryConfiguration(MasterConfigurationModel.PathsConfiguration.NvidiaTemplate, "nvidia", "%THREADS%", nvidiaProfile);
            ScheduleFileDelete(path);

            return $"--nvidia \"{path}\"";
        }

        private static string CreateTemporaryConfiguration(string templatePath, string type, string variable, object value)
        {
            var configTemplateContent = File.ReadAllText(templatePath);
            var configContent = configTemplateContent.Replace(variable, JsonConvert.SerializeObject(value, Formatting.Indented));
            var configPath = $"{Guid.NewGuid()}.{type}.txt";
            File.WriteAllText(configPath, configContent);
            return configPath;
        }

        private void ScheduleFileDelete(string file)
        {
            Finalizer.ScheduleFinalization(() =>
            {
                if (!File.Exists(file)) return;
                try
                {
                    File.Delete(file);
                    Console.WriteLine(@"Configuration file {0} was deleted.", file);
                }
                catch
                {
                    Console.WriteLine(@"Cannot delete configuration file {0}!", file);
                }
            });
        }

        private class UtilizedHardware
        {
            public HardwareEntry Hardware { get; set; }
            public string Profile { get; set; }
        }
    }
}