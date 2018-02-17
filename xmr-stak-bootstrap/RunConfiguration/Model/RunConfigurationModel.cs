using CommandLine;

namespace XmrStakBootstrap.RunConfiguration.Model
{
    public class RunConfigurationModel
    {
        [Option('c', "config", HelpText = "Path to master configuration file", DefaultValue = "master.txt", SetName = "mine")]
        public string MasterConfiguration { get; set; }

        [Option('s', "solution", HelpText = "Name of solution to run", SetName = "mine")]
        public string ActiveSolutionConfiguration { get; set; }

        [Option('w', "workload", HelpText = "Name of workload to run", SetName = "mine")]
        public string ActiveWorkloadConfiguration { get; set; }

        [Option('d', "delay", HelpText = "Number of seconds to wait before terminating old miners and starting new ones", SetName = "mine", DefaultValue = 10)]
        public int MinerStartDelay { get; set; }

        [Option('n', "continuous", HelpText = "Do not close bootstrapper after launching miners", SetName = "mine")]
        public bool ContinuousMode { get; set; }

        [Option('g', "generate", HelpText = "Generate sample configuration", SetName = "generator")]
        public string GenerateConfiguration { get; set; }

        public bool GenerateConfigurationOnly => !string.IsNullOrWhiteSpace(GenerateConfiguration);
    }
}