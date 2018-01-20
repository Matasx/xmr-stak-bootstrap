using Unity.Attributes;
using XmrStakBootstrap.RunConfiguration.Model;

namespace XmrStakBootstrap.Core.Runner.Generator
{
    public class ConfigurationGeneratorRunner : IRunner
    {
        [Dependency]
        public ISampleConfigurationGenerator SampleConfigurationGenerator { get; set; }

        [Dependency]
        public RunConfigurationModel RunConfiguration { get; set; }

        public void Run()
        {
            SampleConfigurationGenerator.Generate(RunConfiguration.GenerateConfiguration);
        }
    }
}