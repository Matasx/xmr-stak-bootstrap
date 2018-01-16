using CommandLine;

namespace xmr_stak_bootstrap
{
    public class ConfigurationParser : IConfigurationParser
    {
        public ParserResult<RunConfiguration> Parse(string[] args)
        {
            return Parser.Default.ParseArguments<RunConfiguration>(args);
        }
    }
}