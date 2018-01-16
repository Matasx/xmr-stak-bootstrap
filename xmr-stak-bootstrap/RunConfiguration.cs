using CommandLine;

namespace xmr_stak_bootstrap
{
    public class RunConfiguration
    {
        [Option('c', "config", HelpText = "Path to master configuration file", DefaultValue = "master.txt")]
        public string MasterConfiguration { get; set; }

        [Option('s', "solution", HelpText = "Name of solution to run")]
        public string ActiveSolutionConfiguration { get; set; }

        [Option('g', "generate", HelpText = "Generate sample configuration")]
        public bool GenerateConfigurationOnly { get; set; }

        [Option('n', "continuous", HelpText = "Do not close bootstrapper after launching miners")]
        public bool ContinuousMode { get; set; }
    }
}