using System;
using System.Diagnostics;
using System.Threading;
using Unity.Attributes;
using XmrStakBootstrap.Common.Helper;
using XmrStakBootstrap.Common.Menu;
using XmrStakBootstrap.MasterConfiguration.Model;
using XmrStakBootstrap.RunConfiguration.Model;

namespace XmrStakBootstrap.Core.Job.Miner
{
    public class MinerRunnerMenuJob : IJob
    {
        private const bool Continue = true;
        private const bool Terminate = false;

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
                        return Continue;
                    })
                    .AddConditionalOption(@"Start/restart miners", CanRun, MenuOptionStartMiners)
                    .AddEnabledOption(@"Change solution", MenuOptionChangeSolution)
                    .AddEnabledOption(@"Change workload", MenuOptionChangeWorkload)
                    .AddEnabledOption(@"Exit", MenuOptionExit)
                    .AddEnabledOption(@"Exit & terminate miners", MenuOptionExitAndTerminateMiners)
                    .Execute();
            }
        }

        #region Menu options

        private bool MenuOptionStartMiners()
        {
            Console.Clear();
            if (KillMiners() && RunConfigurationModel.MinerStartDelay > 0)
            {
                Console.WriteLine($@"Waiting {RunConfigurationModel.MinerStartDelay} second(s) before starting new miner(s).");
                Thread.Sleep(RunConfigurationModel.MinerStartDelay * 1000);
            }
            Finalizer.DoFinalize();
            Runner.Run(); //TODO: issue #1
            return Terminate;
        }

        private bool MenuOptionChangeSolution()
        {
            SelectSolution(); //TODO: issue #1
            Console.Clear();
            return Continue;
        }

        private bool MenuOptionChangeWorkload()
        {
            SelectWorkload(); //TODO: issue #1
            Console.Clear();
            return Continue;
        }

        private bool MenuOptionExit()
        {
            Console.Clear();
            Finalizer.DoFinalize();
            Environment.Exit(0);
            return Terminate;
        }

        private bool MenuOptionExitAndTerminateMiners()
        {
            Console.Clear();
            KillMiners();
            Finalizer.DoFinalize();
            Environment.Exit(0);
            return Terminate;
        }

        #endregion

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

        private static bool KillMiners()
        {
            var any = false;
            foreach (var process in Process.GetProcessesByName("xmr-stak"))
            {
                any = true;
                KillProcess(process);
            }
            return any;
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