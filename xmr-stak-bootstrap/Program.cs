using System;
using Unity;
using Unity.Attributes;
using XmrStakBootstrap.Core.Runner;

namespace XmrStakBootstrap
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                ProgramBootstrapper
                    .RegisterGlobalContainer(new UnityContainer(), args)
                    .Resolve<Program>()
                    .Run();
            }
            catch (Exception e)
            {
                Console.WriteLine(@"Unexpected error occured during initialization: ");
                Console.WriteLine(e);
            }
        }

        [Dependency]
        public RunConfiguration.Model.RunConfigurationModel ConfigurationModel { get; set; }

        [Dependency]
        public IUnityContainer UnityContainer { get; set; }

        public void Run()
        {
            if (ConfigurationModel == null) return;

            while (true)
            {
                using (var container = UnityContainer.CreateChildContainer())
                {
                    ProgramBootstrapper.RegisterRunnerContainer(container);
                    var context = container.Resolve<IRunnerContext>();
                    context.Execute();
                }

                if (!ConfigurationModel.ContinuousMode) break;
            }
        }
    }
}
