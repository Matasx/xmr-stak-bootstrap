using System;

namespace xmr_stak_bootstrap
{
    public interface IFinalizer
    {
        void DoFinalize();
        void ScheduleFinalization(Action action);
    }
}