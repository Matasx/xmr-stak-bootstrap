using System;
using System.Linq;
using Unity;
using Unity.Attributes;
using Unity.Lifetime;

namespace xmr_stak_bootstrap
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                //todo: register loaded configuration and arguments to unity, use as a dependency everywhere

                new UnityContainer()

                    .RegisterType<IConfigurationParser, ConfigurationParser>()
                    .RegisterType<IRunner, Runner>()
                    .RegisterType<ISampleConfigurationGenerator, SampleConfigurationGenerator>()
                    .RegisterType<IFinalizer, Finalizer>(new ContainerControlledLifetimeManager())

                    .Resolve<Program>()
                    .Run(args);
            }
            catch (Exception e)
            {
                Console.WriteLine(@"Unexpected error occured during initialization: ");
                Console.WriteLine(e);
            }
        }

        [Dependency]
        public IConfigurationParser ConfigurationParser { get; set; }

        [Dependency]
        public IRunner Runner { get; set; }

        [Dependency]
        public IFinalizer Finalizer { get; set; }

        public void Run(string[] args)
        {
            var config = ConfigurationParser.Parse(args);
            if (config.Errors.Any()) return;

            while (true)
            {
                try
                {
                    Runner.Run(config.Value);
                }
                catch (Exception e)
                {
                    Console.WriteLine(@"Unexpected error occured during runtime: ");
                    Console.WriteLine(e);
                }
                finally
                {
                    Finalizer.DoFinalize();
                }

                if (!config.Value.ContinuousMode) break;
            }
        }
    }
}
