using System.Linq;
using Unity;
using Unity.Injection;
using Unity.Lifetime;
using XmrStakBootstrap.Core;
using XmrStakBootstrap.Core.Runner;
using XmrStakBootstrap.Core.Runner.Generator;
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
                .RegisterType<IMasterConfigurationParser, MasterConfigurationParser>()
                .RegisterType<IRunner>(new RunnerInjectionFactory())
                .RegisterType<ISampleConfigurationGenerator, SampleConfigurationGenerator>()
                .RegisterType<IFinalizer, Finalizer>(new ContainerControlledLifetimeManager())
                ;
        }

        public static IUnityContainer RegisterRunnerContainer(IUnityContainer unityContainer)
        {
            return unityContainer
                .RegisterType<MasterConfigurationModel>(new ContainerControlledLifetimeManager(), new InjectionFactory(LoadMasterConfiguration))
                .RegisterType<IRunnerContext, RunnerContext>()
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
            return config.Errors.Any() ? null : config.Value;
        }
    }
}