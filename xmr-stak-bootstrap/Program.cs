﻿using System;
using System.Runtime.InteropServices;
using System.Threading;
using Unity;
using Unity.Attributes;
using XmrStakBootstrap.Core;
using XmrStakBootstrap.Core.Job;
using XmrStakBootstrap.RunConfiguration.Model;

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
                Console.WriteLine(@"Unexpected error occured: ");
                Console.WriteLine(e);
            }
        }

        private bool ConsoleEventCallback(int eventType)
        {
            Finalizer?.DoFinalize();
            return false;
        }

        private static ConsoleEventDelegate _handler;
        private delegate bool ConsoleEventDelegate(int eventType);
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleCtrlHandler(ConsoleEventDelegate callback, bool add);

        [Dependency]
        public RunConfigurationModel ConfigurationModel { get; set; }

        [Dependency]
        public IFinalizer Finalizer { get; set; }

        [Dependency]
        public IUnityContainer UnityContainer { get; set; }

        public void Run()
        {
            if (ConfigurationModel == null) return;

            _handler = ConsoleEventCallback;
            SetConsoleCtrlHandler(_handler, true);

            try
            {
                while (true)
                {
                    using (var container = UnityContainer.CreateChildContainer())
                    {
                        ProgramBootstrapper.RegisterRunnerContainer(container);
                        var context = container.Resolve<IJob>();
                        context.Execute();
                    }

                    if (ConfigurationModel.ContinuousMode) continue;

                    Console.WriteLine(@"Waiting 30 seconds before finalization.");
                    Thread.Sleep(30000);
                    break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(@"Unexpected error occured: ");
                Console.WriteLine(e);
            }
            finally
            {
                Finalizer?.DoFinalize();
            }
        }
    }
}
