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
        public RunConfigurationModel RunConfigurationModel { get; set; }

        public void Run(MasterConfigurationModel configuration)
        {
            var solution = configuration.SolutionProfiles.GetValue(RunConfigurationModel.ActiveSolutionConfiguration);
            var workload = configuration.WorkloadProfiles.GetValue(RunConfigurationModel.ActiveWorkloadConfiguration);

            var i = 0;
            foreach (var instances in solution)
            {
                i++;
                var outputPools = GetOutputPools(configuration.Pools, instances.Pools);

                if (outputPools.Count == 0)
                {
                    Console.WriteLine(@"Instance {0}.{1} does not contain any pool bindings.", RunConfigurationModel.ActiveSolutionConfiguration, i);
                    continue;
                }

                var currency = GetPoolCurrency(outputPools);
                if (string.IsNullOrWhiteSpace(currency))
                {
                    Console.WriteLine(@"Instance {0}.{1} does not contain valid currency. Make sure currency for all pools is set to the same value.", RunConfigurationModel.ActiveSolutionConfiguration, i);
                    continue;
                }

                var utilizedHardware = instances.Hardware.Select(x => new UtilizedHardware
                {
                    Hardware = configuration.Hardware.GetValue(x),
                    Profile = workload.GetValue(x)
                }).ToList();

                var cpuArgument = GetCpuArgument(configuration.PathsConfiguration.CpuTemplate, configuration.CpuProfiles, utilizedHardware.Where(x => x.Hardware.Type == "cpu").ToList());
                var amdArgument = GetAmdArgument(configuration.PathsConfiguration.AmdTemplate, configuration.AmdProfiles, utilizedHardware.Where(x => x.Hardware.Type == "amd").ToList());
                var nvidiaArgument = GetNvidiaArgument(configuration.PathsConfiguration.NvidiaTemplate, configuration.NvidiaProfiles, utilizedHardware.Where(x => x.Hardware.Type == "nvidia").ToList());

                var configArgument = GetConfigurationArgument(configuration.PathsConfiguration.ConfigTemplate);
                var poolsArgument = GetPoolsArgument(configuration.PathsConfiguration.PoolsTemplate, outputPools, currency);

                RunMiner($"{configArgument} {poolsArgument} {cpuArgument} {amdArgument} {nvidiaArgument}");
            }
        }

        private static List<PrioritizedPoolEntry> GetOutputPools(IDictionary<string, PoolEntry> poolConfiguration, IEnumerable<string> pools)
        {
            return pools
                .Select(poolConfiguration.GetValue)
                .Reverse()
                .Select((x, i) => new PrioritizedPoolEntry
                {
                    PoolWeight = i + 1,
                    PoolAddress = x.PoolAddress,
                    PoolPassword = x.PoolPassword,
                    TlsFingerprint = x.TlsFingerprint,
                    WalletAddress = x.WalletAddress,
                    UseNiceHash = x.UseNiceHash,
                    UseTls = x.UseTls,
                    RigId = x.RigId,
                    Currency = x.Currency
                })
                .ToList();
        }

        private static string GetPoolCurrency(IEnumerable<PrioritizedPoolEntry> pools)
        {
            var currencies = pools.Select(x => (x.Currency ?? string.Empty).ToLower()).Distinct().ToList();
            return currencies.Count != 1 ? null : currencies.First();
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

        private string GetConfigurationArgument(string configurationTemplatePath)
        {
            var configPath = CreateTemporaryConfiguration(configurationTemplatePath, "config");
            ScheduleFileDelete(configPath);

            return $"--config \"{configPath}\"";
        }

        private string GetPoolsArgument(string poolsTemplatePath, IReadOnlyCollection<PrioritizedPoolEntry> pools, string currency)
        {
            foreach (var prioritizedPoolEntry in pools)
            {
                prioritizedPoolEntry.Currency = null;
            }

            var configPath = CreateTemporaryConfiguration(poolsTemplatePath, "pools",
                new VariableReplacement("%POOLS%", pools), 
                new VariableReplacement("%CURRENCY%", currency));
            ScheduleFileDelete(configPath);

            return $"--poolconf \"{configPath}\"";
        }

        private string GetCpuArgument(string cpuTemplatePath, IDictionary<string, IList<CpuThreadEntry>> cpuConfiguration, ICollection<UtilizedHardware> entry)
        {
            if (entry.Count == 0)
            {
                return "--noCPU";
            }

            var cpuProfile = entry.SelectMany(x => cpuConfiguration.GetValue(x.Profile)).ToList();
            var path = CreateTemporaryConfiguration(cpuTemplatePath, "cpu", new VariableReplacement("%THREADS%", cpuProfile));
            ScheduleFileDelete(path);

            return $"--cpu \"{path}\"";
        }

        private string GetAmdArgument(string amdTemplatePath, IDictionary<string, IList<AmdThreadEntry>> amdConfiguration, ICollection<UtilizedHardware> entry)
        {
            if (entry.Count == 0)
            {
                return "--noAMD";
            }

            var amdProfile = entry
                .SelectMany(
                    x =>
                        amdConfiguration.GetValue(x.Profile)
                            .Select(profile => new IndexedAmdThreadEntry
                            {
                                Index = x.Hardware.Index,
                                AffineToCpu = profile.AffineToCpu,
                                Intensity = profile.Intensity,
                                StridedIndex = profile.StridedIndex,
                                Worksize = profile.Worksize
                            }))
                .ToList();

            var path = CreateTemporaryConfiguration(amdTemplatePath, "amd", new VariableReplacement("%THREADS%", amdProfile));
            ScheduleFileDelete(path);

            return $"--amd \"{path}\"";
        }

        private string GetNvidiaArgument(string nvidiaTemplatePath, IDictionary<string, IList<NvidiaThreadEntry>> nvidiaConfiguration, ICollection<UtilizedHardware> entry)
        {
            if (entry.Count == 0)
            {
                return "--noNVIDIA";
            }

            var nvidiaProfile = entry
                .SelectMany(
                    x =>
                        nvidiaConfiguration.GetValue(x.Profile)
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

            var path = CreateTemporaryConfiguration(nvidiaTemplatePath, "nvidia", new VariableReplacement("%THREADS%", nvidiaProfile));
            ScheduleFileDelete(path);

            return $"--nvidia \"{path}\"";
        }

        private static string CreateTemporaryConfiguration(string templatePath, string type, params VariableReplacement[] variables)
        {
            var content = File.ReadAllText(templatePath);
            if (variables != null)
            {
                foreach(var variable in variables)
                {
                    content = content.Replace(variable.Variable, JsonConvert.SerializeObject(variable.Value, Formatting.Indented));
                }
            }

            var configPath = $"{Guid.NewGuid()}.{type}.txt";
            File.WriteAllText(configPath, content);
            return configPath;
        }

        private class VariableReplacement
        {
            public string Variable { get; set; }
            public object Value { get; set; }

            public VariableReplacement(string variable, object value)
            {
                Variable = variable;
                Value = value;
            }
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