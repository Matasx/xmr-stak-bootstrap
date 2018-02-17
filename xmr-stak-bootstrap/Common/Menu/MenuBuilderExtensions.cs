using System;

namespace XmrStakBootstrap.Common.Menu
{
    public static class MenuBuilderExtensions
    {
        public static IMenuBuilder<T> AddDisabledOption<T>(this IMenuBuilder<T> menuBuilder, string text)
        {
            return menuBuilder.AddConditionalOption(text, false, null);
        }

        public static IMenuBuilder<T> AddEnabledOption<T>(this IMenuBuilder<T> menuBuilder, string text, Func<T> action)
        {
            return menuBuilder.AddConditionalOption(text, true, action);
        }

        public static IMenuBuilder<T> AddConditionalOption<T>(this IMenuBuilder<T> menuBuilder, string text, bool enabled, Func<T> action)
        {
            menuBuilder.AddOption(text, enabled, action);
            return menuBuilder;
        }
    }
}