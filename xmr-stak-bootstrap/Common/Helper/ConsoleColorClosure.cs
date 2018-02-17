using System;

namespace XmrStakBootstrap.Common.Helper
{
    public static class ConsoleColorClosure
    {
        public static IDisposable ForegroundColor(ConsoleColor color)
        {
            var original = Console.ForegroundColor;
            return new Closure(() => Console.ForegroundColor = color, () => Console.ForegroundColor = original);
        }
    }
}