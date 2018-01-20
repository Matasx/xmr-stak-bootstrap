using System.Diagnostics.CodeAnalysis;
using Unity;
using Unity.Attributes;
using Unity.Injection;
using XmrStakBootstrap.Core.Runner.Generator;
using XmrStakBootstrap.Core.Runner.Miner;
using XmrStakBootstrap.RunConfiguration.Model;

namespace XmrStakBootstrap.Core.Runner
{
    public class RunnerInjectionFactory : InjectionFactory
    {
        public RunnerInjectionFactory() : base(CreateRunner)
        {
        }

        private static IRunner CreateRunner(IUnityContainer container)
        {
            return container.Resolve<RunnerFactory>().CreateRunner();
        }

        // ReSharper disable once ClassNeverInstantiated.Local
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Local")]
        private class RunnerFactory
        {
            [Dependency]
            public RunConfigurationModel ConfigurationModel { get; set; }

            [Dependency]
            public IUnityContainer UnityContainer { get; set; }

            public IRunner CreateRunner()
            {
                if (ConfigurationModel.GenerateConfigurationOnly)
                {
                    return UnityContainer.Resolve<ConfigurationGeneratorRunner>();
                }

                return UnityContainer.Resolve<MinerRunner>();
            }
        }
    }
}