using CommandLine;
using XmrStakBootstrap.RunConfiguration.Model;

namespace XmrStakBootstrap.RunConfiguration
{
    public interface IRunConfigurationParser
    {
        ParserResult<RunConfigurationModel> Parse(string[] args);
    }
}