using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Unity.Attributes;
using xmr_stak_bootstrap.Model;

namespace xmr_stak_bootstrap
{
    public class Runner : IRunner
    {
        //TODO: can be refactored further to classes

        [Dependency]
        public IFinalizer Finalizer { get; set; }

        [Dependency]
        public Lazy<ISampleConfigurationGenerator> SampleConfigurationGenerator { get; set; }

        public void Run(RunConfiguration args)
        {
            if (args.GenerateConfigurationOnly)
            {
                SampleConfigurationGenerator.Value.Generate(args.MasterConfiguration);
            }
            else
            {
                if (!File.Exists(args.MasterConfiguration))
                {
                    Console.WriteLine(@"Configuration file '{0}' does not exist. Try running this program with argument --help.", args.MasterConfiguration);
                    return;
                }

                DoRun(args);
            }
        }

        private void DoRun(RunConfiguration args)
        {
            var contents = File.ReadAllText(args.MasterConfiguration);
            var configuration = JsonConvert.DeserializeObject<MasterConfiguration>(contents);

            var activeSolution = GetActiveSolution(args, configuration);
            KillMiners();

            var solution = configuration.SolutionConfiguration.SolutionProfiles.GetValue(activeSolution);
            foreach (var instances in solution)
            {
                var outputPools = GetOutputPools(configuration, instances);

                if (outputPools.Count == 0)
                {
                    Console.WriteLine(@"Solution does not contain any pool bindings.");
                    continue;
                }

                var configArgument = GetConfigurationArgument(configuration, outputPools);

                foreach (var instance in instances.Value)
                {
                    var entry = configuration.InstanceConfiguration.InstanceProfiles.GetValue(instance);

                    var cpuArgument = GetCpuArgument(configuration, entry);
                    var amdArgument = GetAmdArgument(configuration, entry);
                    var nvidiaArgument = GetNvidiaArgument(configuration, entry);

                    RunMiner($"{configArgument} {cpuArgument} {amdArgument} {nvidiaArgument}");
                }
            }
        }

        private static string GetActiveSolution(RunConfiguration args, MasterConfiguration configuration)
        {
            var activeSolution = args.ActiveSolutionConfiguration;

            while (string.IsNullOrEmpty(activeSolution))
            {
                Console.WriteLine(@"Available solutions: ");
                var keys = configuration.SolutionConfiguration.SolutionProfiles.Keys.OrderBy(x => x).ToList();
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

        private static List<PrioritizedPoolEntry> GetOutputPools(MasterConfiguration configuration, KeyValuePair<string, IList<string>> instances)
        {
            return configuration
                .PoolConfiguration
                .PoolSets.GetValue(instances.Key)
                .Reverse()
                .Select(x => configuration.PoolConfiguration.Pools.GetValue(x))
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

        private void RunMiner(string arguments)
        {
            var startInfo = new ProcessStartInfo(Path.GetFullPath("xmr-stak.exe"), arguments)
            {
                WorkingDirectory = Environment.CurrentDirectory,
                UseShellExecute = false
            };

            Console.WriteLine(@"Starting: xmr-stak.exe {0}", arguments);

            ScheduleProcessKill(Process.Start(startInfo));
        }

        private string GetConfigurationArgument(MasterConfiguration configuration, List<PrioritizedPoolEntry> pools)
        {
            var configPath = CreateTemporaryConfiguration(configuration.ConfigTemplate, "config", "%POOLS%", pools);
            ScheduleFileDelete(configPath);

            return $"--config \"{configPath}\"";
        }

        private string GetCpuArgument(MasterConfiguration configuration, InstanceEntry entry)
        {
            if (string.IsNullOrEmpty(entry.CpuProfile))
            {
                return "--noCPU";
            }

            var cpuProfile = configuration.CpuConfiguration.Profiles.GetValue(entry.CpuProfile);
            var path = CreateTemporaryConfiguration(configuration.CpuTemplate, "cpu", "%THREADS%", cpuProfile);
            ScheduleFileDelete(path);

            return $"--cpu \"{path}\"";
        }

        private string GetAmdArgument(MasterConfiguration configuration, InstanceEntry entry)
        {
            if (entry.AmdProfiles == null || entry.AmdProfiles.Count == 0)
            {
                return "--noAMD";
            }

            var amdProfile = entry.AmdProfiles.SelectMany(x => configuration.AmdConfiguration.Profiles.GetValue(x)).ToList();
            var path = CreateTemporaryConfiguration(configuration.AmdTemplate, "amd", "%THREADS%", amdProfile);
            ScheduleFileDelete(path);

            return $"--amd \"{path}\"";
        }

        private string GetNvidiaArgument(MasterConfiguration configuration, InstanceEntry entry)
        {
            if (entry.NvidiaProfiles == null || entry.NvidiaProfiles.Count == 0)
            {
                return "--noNVIDIA";
            }

            var nvidiaProfile = entry.NvidiaProfiles.SelectMany(x => configuration.NvidiaConfiguration.Profiles.GetValue(x)).ToList();
            var path = CreateTemporaryConfiguration(configuration.NvidiaTemplate, "nvidia", "%THREADS%", nvidiaProfile);
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

        private void ScheduleProcessKill(Process process)
        {
            var proc = process.Id; //capture
            Finalizer.ScheduleFinalization(() => KillProcess(proc, process));
        }
    }
}