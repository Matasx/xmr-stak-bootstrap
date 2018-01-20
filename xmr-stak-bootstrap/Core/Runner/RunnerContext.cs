using System;
using Unity.Attributes;

namespace XmrStakBootstrap.Core.Runner
{
    public class RunnerContext : IRunnerContext
    {
        [Dependency]
        public IRunner Runner { get; set; }

        [Dependency]
        public IFinalizer Finalizer { get; set; }

        public void Execute()
        {
            try
            {
                Runner.Run();
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
        }
    }
}