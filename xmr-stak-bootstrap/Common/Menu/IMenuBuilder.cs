using System;

namespace XmrStakBootstrap.Common.Menu
{
    public interface IMenuBuilder<T>
    {
        Func<T> DefaultAction { get; set; }
        void AddOption(string text, bool enabled, Func<T> action);
        T Execute();
    }
}