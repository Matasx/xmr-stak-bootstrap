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

namespace XmrStakBootstrap.Core.Runner.Miner
{
    public class MinerRunner : IRunner
    {
        [Dependency]
        public IFinalizer Finalizer { get; set; }

        [Dependency]
        public RunConfigurationModel RunConfigurationModel { get; set; }

        [Dependency]
        public MasterConfigurationModel MasterConfigurationModel { get; set; }

        public void Run()
        {
            if (MasterConfigurationModel == null)
            {
                Console.WriteLine(@"Master configuration file '{0}' was not loaded. Try running this program with argument --help.", RunConfigurationModel.MasterConfiguration);
                return;
            }

            DoRun();
        }

        private void DoRun()
        {
            var activeSolution = GetActiveSolution();
            KillMiners();

            var solution = MasterConfigurationModel.SolutionConfiguration.SolutionProfiles.GetValue(activeSolution);
            foreach (var instances in solution)
            {
                var outputPools = GetOutputPools(instances);

                if (outputPools.Count == 0)
                {
                    Console.WriteLine(@"Solution does not contain any pool bindings.");
                    continue;
                }

                var configArgument = GetConfigurationArgument(outputPools);

                foreach (var instance in instances.Value)
                {
                    var entry = MasterConfigurationModel.InstanceConfiguration.InstanceProfiles.GetValue(instance);

                    var cpuArgument = GetCpuArgument(entry);
                    var amdArgument = GetAmdArgument(entry);
                    var nvidiaArgument = GetNvidiaArgument(entry);

                    RunMiner($"{configArgument} {cpuArgument} {amdArgument} {nvidiaArgument}");
                }
            }
        }

        private string GetActiveSolution()
        {
            //TODO: rewrite once configuration is changed
            var activeSolution = RunConfigurationModel.ActiveSolutionConfiguration;

            while (string.IsNullOrEmpty(activeSolution))
            {
                Console.WriteLine(@"Available solutions: ");
                var keys = MasterConfigurationModel.SolutionConfiguration.SolutionProfiles.Keys.ToList();
                var i = 0;
                foreach (var key in keys)
                {
                    Console.WriteLine(@"{0,3}: {1}", i, key);
                    i++;
                }
                Console.Write(@"Select: ");
                int index;
                if (int.TryParse(Console.ReadLine(), out index) && index >= 0 && index < keys.Count)
                {
                    activeSolution = keys[index];
                }
            }

            return activeSolution;
        }

        private List<PrioritizedPoolEntry> GetOutputPools(KeyValuePair<string, IList<string>> instances)
        {
            return MasterConfigurationModel
                .PoolConfiguration
                .PoolSets.GetValue(instances.Key)
                .Reverse()
                .Select(x => MasterConfigurationModel.PoolConfiguration.Pools.GetValue(x))
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
            var configPath = CreateTemporaryConfiguration(MasterConfigurationModel.ConfigTemplate, "config", "%POOLS%", pools);
            ScheduleFileDelete(configPath);

            return $"--config \"{configPath}\"";
        }

        private string GetCpuArgument(InstanceEntry entry)
        {
            if (string.IsNullOrEmpty(entry.CpuProfile))
            {
                return "--noCPU";
            }

            var cpuProfile = MasterConfigurationModel.CpuConfiguration.Profiles.GetValue(entry.CpuProfile);
            var path = CreateTemporaryConfiguration(MasterConfigurationModel.CpuTemplate, "cpu", "%THREADS%", cpuProfile);
            ScheduleFileDelete(path);

            return $"--cpu \"{path}\"";
        }

        private string GetAmdArgument(InstanceEntry entry)
        {
            if (entry.AmdProfiles == null || entry.AmdProfiles.Count == 0)
            {
                return "--noAMD";
            }

            var amdProfile = entry.AmdProfiles.SelectMany(x => MasterConfigurationModel.AmdConfiguration.Profiles.GetValue(x)).ToList();
            var path = CreateTemporaryConfiguration(MasterConfigurationModel.AmdTemplate, "amd", "%THREADS%", amdProfile);
            ScheduleFileDelete(path);

            return $"--amd \"{path}\"";
        }

        private string GetNvidiaArgument(InstanceEntry entry)
        {
            if (entry.NvidiaProfiles == null || entry.NvidiaProfiles.Count == 0)
            {
                return "--noNVIDIA";
            }

            var nvidiaProfile = entry.NvidiaProfiles.SelectMany(x => MasterConfigurationModel.NvidiaConfiguration.Profiles.GetValue(x)).ToList();
            var path = CreateTemporaryConfiguration(MasterConfigurationModel.NvidiaTemplate, "nvidia", "%THREADS%", nvidiaProfile);
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

        private static void KillMiners()
        {
            foreach (var process in Process.GetProcessesByName("xmr-stak"))
            {
                KillProcess(process);
            }
        }

        private static void KillProcess(Process process)
        {
            KillProcess(process.Id, process);
        }

        private static void KillProcess(int processId, Process process)
        {
            try
            {
                process.Kill();
                process.Dispose();
                Console.WriteLine(@"Process {0} was killed.", processId);
            }
            catch (Exception)
            {
                Console.WriteLine(@"Cannot kill process {0}!", processId);
            }
        }
    }
}