using CommandLine;
using XmrStakBootstrap.RunConfiguration.Model;

namespace XmrStakBootstrap.RunConfiguration
{
    public class RunConfigurationParser : IRunConfigurationParser
    {
        public ParserResult<RunConfigurationModel> Parse(string[] args)
        {
            return Parser.Default.ParseArguments<RunConfigurationModel>(args);
        }
    }
}