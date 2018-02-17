using System;
using System.Diagnostics;
using Unity.Attributes;
using XmrStakBootstrap.Common.Helper;
using XmrStakBootstrap.Common.Menu;
using XmrStakBootstrap.MasterConfiguration.Model;
using XmrStakBootstrap.RunConfiguration.Model;

namespace XmrStakBootstrap.Core.Job.Miner
{
    public class MinerRunnerMenuJob : IJob
    {
        [Dependency]
        public IFinalizer Finalizer { get; set; }

        [Dependency]
        public RunConfigurationModel RunConfigurationModel { get; set; }

        [Dependency]
        public MasterConfigurationModel MasterConfigurationModel { get; set; }

        [Dependency]
        public IMinerRunner Runner { get; set; }

        private bool HasSolution
            =>
                MasterConfigurationModel.SolutionProfiles.ContainsKey(
                    RunConfigurationModel.ActiveSolutionConfiguration ?? string.Empty);

        private bool HasWorkload
            =>
                MasterConfigurationModel.WorkloadProfiles.ContainsKey(
                    RunConfigurationModel.ActiveWorkloadConfiguration ?? string.Empty);

        private bool CanRun => HasSolution && HasWorkload;

        public void Execute()
        {
            if (MasterConfigurationModel == null)
            {
                Console.WriteLine(@"Master configuration file '{0}' was not loaded. Try running this program with argument --help.", RunConfigurationModel.MasterConfiguration);
                return;
            }

            var @continue = true;
            while (@continue)
            {
                Console.Write(@"Active solution: ");
                ColorConsole(HasSolution ? ConsoleColor.White : ConsoleColor.DarkGray, () => Console.WriteLine(HasSolution ? RunConfigurationModel.ActiveSolutionConfiguration : "<UNKNOWN>"));
                Console.Write(@"Active workload: ");
                ColorConsole(HasWorkload ? ConsoleColor.White : ConsoleColor.DarkGray, () => Console.WriteLine(HasWorkload ? RunConfigurationModel.ActiveWorkloadConfiguration : "<UNKNOWN>"));
                Console.WriteLine();
                Console.WriteLine(@"What would you like to do?");

                @continue = MenuBuilder
                    .Create(() =>
                    {
                        Console.Clear();
                        return true;
                    })
                    .AddConditionalOption(@"Start/restart miners", CanRun, () =>
                    {
                        Console.Clear();
                        KillMiners(); //TODO: issue #6
                        Finalizer.DoFinalize();
                        Runner.Run(); //TODO: issue #1
                        return false;
                    })
                    .AddEnabledOption(@"Change solution", () =>
                    {
                        SelectSolution(); //TODO: issue #1
                        Console.Clear();
                        return true;
                    })
                    .AddEnabledOption(@"Change workload", () =>
                    {
                        SelectWorkload(); //TODO: issue #1
                        Console.Clear();
                        return true;
                    })
                    .AddEnabledOption(@"Exit", () =>
                    {
                        Console.Clear();
                        Finalizer.DoFinalize();
                        Environment.Exit(0);
                        return false;
                    })
                    .AddEnabledOption(@"Exit & terminate miners", () =>
                    {
                        Console.Clear();
                        KillMiners();
                        Finalizer.DoFinalize();
                        Environment.Exit(0);
                        return false;
                    })
                    .Execute();
            }
        }

        private static void ColorConsole(ConsoleColor color, Action action)
        {
            using (ConsoleColorClosure.ForegroundColor(color)) action();
        }

        private void SelectSolution()
        {
            Console.WriteLine();
            Console.WriteLine(@"Available solutions: ");
            RunConfigurationModel.ActiveSolutionConfiguration =
                MenuBuilder.CreateTextListMenu(MasterConfigurationModel.SolutionProfiles.Keys)
                    .Execute();
        }

        private void SelectWorkload()
        {
            Console.WriteLine();
            Console.WriteLine(@"Available workloads: ");
            RunConfigurationModel.ActiveWorkloadConfiguration =
                MenuBuilder.CreateTextListMenu(MasterConfigurationModel.WorkloadProfiles.Keys)
                    .Execute();
        }

        private static void KillMiners()
        {
            foreach (var process in Process.GetProcessesByName("xmr-stak"))
            {
                KillProcess(process);
            }
        }

        private static void KillProcess(Process process)
        {
            KillProcess(process.Id, process);
        }

        private static void KillProcess(int processId, Process process)
        {
            try
            {
                process.Kill();
                process.Dispose();
                Console.WriteLine(@"Process {0} was killed.", processId);
            }
            catch (Exception)
            {
                Console.WriteLine(@"Cannot kill process {0}!", processId);
            }
        }
    }
}