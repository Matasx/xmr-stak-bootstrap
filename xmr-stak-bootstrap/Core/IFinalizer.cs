using System;

namespace XmrStakBootstrap.Core
{
    public interface IFinalizer
    {
        void DoFinalize();
        void ScheduleFinalization(Action action);
    }
}