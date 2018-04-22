using System;
using System.IO;
using Newtonsoft.Json;
using XmrStakBootstrap.MasterConfiguration.Model;
using XmrStakBootstrap.Properties;

namespace XmrStakBootstrap.Core.Job.Generator
{
    public class SampleConfigurationGenerator : ISampleConfigurationGenerator
    {
        public void Generate(string path)
        {
            var configuration = SampleConfigurationData.MasterConfigurationModel;
            var contents = JsonConvert.SerializeObject(configuration, Formatting.Indented);
            WriteAllText(path, contents);
            WriteAllText(configuration.PathsConfiguration.ConfigTemplate, Resources.config_source);
            WriteAllText(configuration.PathsConfiguration.CpuTemplate, Resources.cpu_source);
            WriteAllText(configuration.PathsConfiguration.NvidiaTemplate, Resources.nvidia_source);
            WriteAllText(configuration.PathsConfiguration.AmdTemplate, Resources.amd_source);
            WriteAllText(configuration.PathsConfiguration.PoolsTemplate, Resources.pools_source);
        }

        private static void WriteAllText(string path, string content)
        {
            if (File.Exists(path))
            {
                Console.WriteLine(@"File {0} already exists!", path);
            }
            File.WriteAllText(path, content);
        }
    }
}