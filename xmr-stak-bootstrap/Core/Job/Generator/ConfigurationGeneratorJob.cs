using Unity.Attributes;
using XmrStakBootstrap.RunConfiguration.Model;

namespace XmrStakBootstrap.Core.Job.Generator
{
    public class ConfigurationGeneratorJob : IJob
    {
        [Dependency]
        public ISampleConfigurationGenerator SampleConfigurationGenerator { get; set; }

        [Dependency]
        public RunConfigurationModel RunConfiguration { get; set; }

        public void Execute()
        {
            SampleConfigurationGenerator.Generate(RunConfiguration.GenerateConfiguration);
        }
    }
}