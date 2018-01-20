using System;
using System.Collections.Generic;
using System.Threading;

namespace XmrStakBootstrap.Core
{
    public class Finalizer : IFinalizer
    {
        private readonly IList<Action> _actions = new List<Action>();

        public void DoFinalize()
        {
            if (_actions.Count == 0) return;

            Console.WriteLine(@"Waiting 10 seconds before finalization.");
            Thread.Sleep(10000);

            foreach (var action in _actions)
            {
                action();
            }
        }

        public void ScheduleFinalization(Action action)
        {
            if (action == null) return;

            _actions.Add(action);
        }
    }
}