using System;
using System.Collections.Generic;

namespace XmrStakBootstrap.Core
{
    public class Finalizer : IFinalizer
    {
        private readonly IList<Action> _actions = new List<Action>();
        private readonly object _lock = new object();

        public void DoFinalize()
        {
            lock (_lock)
            {
                if (_actions.Count == 0) return;

                foreach (var action in _actions)
                {
                    action();
                }
            }
        }

        public void ScheduleFinalization(Action action)
        {
            lock (_lock)
            {
                if (action == null) return;

                _actions.Add(action);
            }
        }
    }
}