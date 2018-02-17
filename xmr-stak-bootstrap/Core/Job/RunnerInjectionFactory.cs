using System.Diagnostics.CodeAnalysis;
using Unity;
using Unity.Attributes;
using Unity.Injection;
using XmrStakBootstrap.Core.Job.Generator;
using XmrStakBootstrap.Core.Job.Miner;
using XmrStakBootstrap.RunConfiguration.Model;

namespace XmrStakBootstrap.Core.Job
{
    public class RunnerInjectionFactory : InjectionFactory
    {
        public RunnerInjectionFactory() : base(CreateRunner)
        {
        }

        private static IJob CreateRunner(IUnityContainer container)
        {
            return container.Resolve<RunnerFactory>().CreateRunner();
        }

        // ReSharper disable once ClassNeverInstantiated.Local
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Local")]
        private class RunnerFactory
        {
            [Dependency]
            public RunConfigurationModel RunConfiguration { get; set; }

            [Dependency]
            public IUnityContainer UnityContainer { get; set; }

            public IJob CreateRunner()
            {
                if (RunConfiguration.GenerateConfigurationOnly)
                {
                    return UnityContainer.Resolve<ConfigurationGeneratorJob>();
                }

                return UnityContainer.Resolve<MinerRunnerMenuJob>();
            }
        }
    }
}