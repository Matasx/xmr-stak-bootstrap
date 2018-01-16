using System;
using System.IO;
using Newtonsoft.Json;
using xmr_stak_bootstrap.Model;
using xmr_stak_bootstrap.Properties;

namespace xmr_stak_bootstrap
{
    public class SampleConfigurationGenerator : ISampleConfigurationGenerator
    {
        public void Generate(string path)
        {
            var configuration = SampleConfigurationData.MasterConfiguration;
            var contents = JsonConvert.SerializeObject(configuration, Formatting.Indented);
            WriteAllText(path, contents);
            WriteAllText(configuration.ConfigTemplate, Resources.config_source);
            WriteAllText(configuration.CpuTemplate, Resources.cpu_source);
            WriteAllText(configuration.NvidiaTemplate, Resources.nvidia_source);
            WriteAllText(configuration.AmdTemplate, Resources.amd_source);
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