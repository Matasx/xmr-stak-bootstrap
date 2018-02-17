using System;

namespace XmrStakBootstrap.Common.Helper
{
    public class Closure : IDisposable
    {
        private readonly Action _postAction;
        private bool _disposed;

        public Closure(Action preAction, Action postAction)
        {
            _postAction = postAction;

            preAction?.Invoke();
        }

        public void Dispose()
        {
            if (_disposed) return;

            _postAction?.Invoke();
            _disposed = true;
        }
    }
}