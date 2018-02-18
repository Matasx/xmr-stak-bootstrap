using System;
using System.Diagnostics;
using System.Threading;
using Newtonsoft.Json;
using Unity;
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

        private MasterConfigurationModel _masterConfigurationModel;

        [Dependency]
        public IUnityContainer UnityContainer { get; set; }

        [Dependency]
        public IFinalizer Finalizer { get; set; }

        [Dependency]
        public IMinerRunner Runner { get; set; }

        [Dependency]
        public RunConfigurationModel RunConfigurationModel { get; set; }

        private bool HasSolution
            =>
                GetMasterConfigurationModel().SolutionProfiles.ContainsKey(
                    RunConfigurationModel.ActiveSolutionConfiguration ?? string.Empty);

        private bool HasWorkload
            =>
                GetMasterConfigurationModel().WorkloadProfiles.ContainsKey(
                    RunConfigurationModel.ActiveWorkloadConfiguration ?? string.Empty);

        private bool CanRun => HasSolution && HasWorkload;

        public void Execute()
        {
            var @continue = true;
            while (@continue)
            {
                GetMasterConfigurationModel();

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
                    .AddEnabledOption(@"Reload configuration", MenuOptionReloadConfiguration)
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
            Runner.Run(_masterConfigurationModel);
            return Terminate;
        }

        private bool MenuOptionReloadConfiguration()
        {
            Console.Clear();
            InvalidateConfiguration();
            return Continue;
        }

        private bool MenuOptionChangeSolution()
        {
            InvalidateConfiguration();
            SelectSolution();
            Console.Clear();
            return Continue;
        }

        private bool MenuOptionChangeWorkload()
        {
            InvalidateConfiguration();
            SelectWorkload();
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

        private MasterConfigurationModel GetMasterConfigurationModel()
        {
            return _masterConfigurationModel ?? (_masterConfigurationModel = LoadConfiguration());
        }

        private void InvalidateConfiguration()
        {
            _masterConfigurationModel = null;
        }

        private MasterConfigurationModel LoadConfiguration()
        {
            while (true)
            {
                try
                {
                    var config = UnityContainer.Resolve<MasterConfigurationModel>();
                    if (config == null)
                    {
                        Console.WriteLine(@"Master configuration file '{0}' was not loaded. Try running this program with argument --help.", RunConfigurationModel.MasterConfiguration);
                        WaitForAnyKeyToReloadConfiguration();
                        continue;
                    }
                    Console.WriteLine(@"Configuration was loaded.");
                    return config;
                }
                catch (Exception e)
                {
                    using (ConsoleColorClosure.ForegroundColor(ConsoleColor.Red))
                    {
                        Console.WriteLine(@"An error occured while loading master configuration:");
                        Console.WriteLine((e.InnerException as JsonReaderException)?.Message ?? e.Message);
                    }
                    WaitForAnyKeyToReloadConfiguration();
                }
            }
        }

        private static void WaitForAnyKeyToReloadConfiguration()
        {
            Console.WriteLine();
            Console.Write(@"Press any key to reload configuration.");
            Console.ReadKey(true);
            Console.Clear();
        }

        private void SelectSolution()
        {
            var configuration = GetMasterConfigurationModel();
            Console.WriteLine();
            Console.WriteLine(@"Available solutions: ");
            RunConfigurationModel.ActiveSolutionConfiguration =
                MenuBuilder.CreateTextListMenu(configuration.SolutionProfiles.Keys)
                    .Execute();
        }

        private void SelectWorkload()
        {
            var configuration = GetMasterConfigurationModel();
            Console.WriteLine();
            Console.WriteLine(@"Available workloads: ");
            RunConfigurationModel.ActiveWorkloadConfiguration =
                MenuBuilder.CreateTextListMenu(configuration.WorkloadProfiles.Keys)
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