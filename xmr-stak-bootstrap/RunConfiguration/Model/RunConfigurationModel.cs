using CommandLine;

namespace XmrStakBootstrap.RunConfiguration.Model
{
    public class RunConfigurationModel
    {
        [Option('c', "config", HelpText = "Path to master configuration file", DefaultValue = "master.txt")]
        public string MasterConfiguration { get; set; }

        [Option('s', "solution", HelpText = "Name of solution to run", SetName = "mine")]
        public string ActiveSolutionConfiguration { get; set; }

        [Option('g', "generate", HelpText = "Generate sample configuration", SetName = "generator")]
        public bool GenerateConfigurationOnly { get; set; }

        [Option('n', "continuous", HelpText = "Do not close bootstrapper after launching miners", SetName = "mine")]
        public bool ContinuousMode { get; set; }
    }
}