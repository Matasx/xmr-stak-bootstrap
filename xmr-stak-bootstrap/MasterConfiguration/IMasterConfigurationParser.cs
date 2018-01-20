using XmrStakBootstrap.MasterConfiguration.Model;

namespace XmrStakBootstrap.MasterConfiguration
{
    public interface IMasterConfigurationParser
    {
        MasterConfigurationModel Parse();
    }
}