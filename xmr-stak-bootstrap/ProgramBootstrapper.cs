using System.Linq;
using Unity;
using Unity.Injection;
using Unity.Lifetime;
using XmrStakBootstrap.Core;
using XmrStakBootstrap.Core.Job;
using XmrStakBootstrap.Core.Job.Generator;
using XmrStakBootstrap.Core.Job.Miner;
using XmrStakBootstrap.MasterConfiguration;
using XmrStakBootstrap.MasterConfiguration.Model;
using XmrStakBootstrap.RunConfiguration;
using XmrStakBootstrap.RunConfiguration.Model;

namespace XmrStakBootstrap
{
    public static class ProgramBootstrapper
    {
        public static IUnityContainer RegisterGlobalContainer(IUnityContainer unityContainer, string[] args)
        {
            return unityContainer
                .RegisterType<RunConfigurationModel>(new ContainerControlledLifetimeManager(), new InjectionFactory(container => ParseRunConfiguration(container, args)))
                .RegisterType<IRunConfigurationParser, RunConfigurationParser>()
                .RegisterType<IFinalizer, Finalizer>(new ContainerControlledLifetimeManager())
                .RegisterType<MasterConfigurationModel>(new InjectionFactory(LoadMasterConfiguration))
                .RegisterType<IMasterConfigurationParser, MasterConfigurationParser>()
                .RegisterType<IJob>(new RunnerInjectionFactory())
                .RegisterType<IMinerRunner, MinerRunner>()
                .RegisterType<ISampleConfigurationGenerator, SampleConfigurationGenerator>()
                ;
        }

        private static MasterConfigurationModel LoadMasterConfiguration(IUnityContainer unityContainer)
        {
            var parser = unityContainer.Resolve<IMasterConfigurationParser>();
            return parser.Parse();
        }

        private static RunConfigurationModel ParseRunConfiguration(IUnityContainer unityContainer, string[] args)
        {
            var parser = unityContainer.Resolve<IRunConfigurationParser>();
            var config = parser.Parse(args);
            return config.Errors.Any() ? RunConfigurationModel.InvalidModel : config.Value;
        }
    }
}