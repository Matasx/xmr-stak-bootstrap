using System;
using System.Collections.Generic;
using System.Linq;
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
            switch (_options.Count)
            {
                case 0:
                    return ExecuteDefault();

                case 1:
                    return ExecuteAction(_options.First().Action);

                default:
                    RenderOptions();
                    var input = GetInput();

                    if (!IsInputValid(input)) return ExecuteDefault();

                    var option = _options[input - 1];
                    return option.IsEnabled ? ExecuteAction(option.Action) : ExecuteDefault();
            }
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

        private int GetInput()
        {
            Console.Write(@"Select: ");

            var knownOptions = _options.Select((_, i) => (i + 1).ToString()).ToList();

            ConsoleKeyInfo key;
            var input = "";
            int index;
            do
            {
                key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Backspace && input.Length > 0)
                {
                    input = input.Substring(0, input.Length - 1);
                    // ReSharper disable once LocalizableElement
                    Console.Write("\b \b");
                }
                else if (char.IsNumber(key.KeyChar))
                {
                    input += key.KeyChar;
                    Console.Write(key.KeyChar);
                }

                if (int.TryParse(input, out index) && IsInputValid(index))
                {
                    var stringInput = index.ToString();
                    if (knownOptions.Count(x => x.StartsWith(stringInput)) == 1) return index;
                }
            }
            while (key.Key != ConsoleKey.Enter);
            return int.TryParse(input, out index) ? index : -1;
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