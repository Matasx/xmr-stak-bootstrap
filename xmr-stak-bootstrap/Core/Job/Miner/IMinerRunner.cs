using XmrStakBootstrap.MasterConfiguration.Model;

namespace XmrStakBootstrap.Core.Job.Miner
{
    public interface IMinerRunner
    {
        void Run(MasterConfigurationModel configuration);
    }
}