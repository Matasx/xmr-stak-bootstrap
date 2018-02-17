using System;
using System.Collections.Generic;
using XmrStakBootstrap.Common.Helper;

namespace XmrStakBootstrap.Common.Menu
{
    public static class MenuBuilder
    {
        public static IMenuBuilder<T> Create<T>(Func<T> defaultAction = null)
        {
            return new MenuBuilder<T>
            {
                DefaultAction = defaultAction
            };
        }

        public static IMenuBuilder<string> CreateTextListMenu(IEnumerable<string> values, string defaultValue = null)
        {
            var menu = new MenuBuilder<string>
            {
                DefaultAction = () => defaultValue
            };

            foreach (var option in values)
            {
                menu.AddEnabledOption(option, () => option);
            }

            return menu;
        }
    }

    public class MenuBuilder<T> : IMenuBuilder<T>
    {
        public Func<T> DefaultAction { get; set; }
        private readonly IList<Option> _options = new List<Option>();

        public void AddOption(string text, bool enabled, Func<T> action)
        {
            _options.Add(new Option
            {
                Action = action,
                IsEnabled = enabled,
                Text = text
            });
        }

        public T Execute()
        {
            if (_options.Count == 0) return ExecuteDefault();

            RenderOptions();
            var input = GetInput();

            if (!IsInputValid(input)) return ExecuteDefault();

            var option = _options[input - 1];
            return option.IsEnabled ? ExecuteAction(option.Action) : ExecuteDefault();
        }

        private T ExecuteDefault()
        {
            return ExecuteAction(null);
        }

        private T ExecuteAction(Func<T> action)
        {
            return (action ?? DefaultAction ?? (() => default(T)))();
        }

        private bool IsInputValid(int input)
        {
            return input >= 1 && input <= _options.Count;
        }

        private static int GetInput()
        {
            Console.Write(@"Select: ");
            int index;
            if (!int.TryParse(Console.ReadLine(), out index)) return -1;
            return index;
        }

        private void RenderOptions()
        {
            var index = 1;
            foreach (var option in _options)
            {
                RenderOption(index++, option);
            }
        }

        private static void RenderOption(int index, Option option)
        {
            using (ConsoleColorClosure.ForegroundColor(GetOptionColor(option)))
            {
                Console.WriteLine(@"{0,3}: {1}", index, option.Text);
            }
        }

        private static ConsoleColor GetOptionColor(Option option)
        {
            return option.IsEnabled ? ConsoleColor.White : ConsoleColor.DarkGray;
        }

        private class Option
        {
            public string Text { get; set; }
            public bool IsEnabled { get; set; }
            public Func<T> Action { get; set; }
        }
    }
}