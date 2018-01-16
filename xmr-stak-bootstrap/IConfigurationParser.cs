using CommandLine;

namespace xmr_stak_bootstrap
{
    public interface IConfigurationParser
    {
        ParserResult<RunConfiguration> Parse(string[] args);
    }
}