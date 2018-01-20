using Unity.Attributes;
using XmrStakBootstrap.RunConfiguration.Model;

namespace XmrStakBootstrap.Core.Runner.Generator
{
    public class ConfigurationGeneratorRunner : IRunner
    {
        [Dependency]
        public ISampleConfigurationGenerator SampleConfigurationGenerator { get; set; }

        [Dependency]
        public RunConfigurationModel ConfigurationModel { get; set; }

        public void Run()
        {
            SampleConfigurationGenerator.Generate(ConfigurationModel.MasterConfiguration);
        }
    }
}