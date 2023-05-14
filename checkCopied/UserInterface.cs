using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace checkCopied
{
    internal class UserInterface
    {
        public class MenuOption
        {
            public string Name { get; set; } = "";
            public string Value { get; set; } = "";

            public Action Action { get; set; } = () => { };

        }

        public static void PrintMenuOptions(MenuOption[] menuOptions)
        {
            foreach (var option in menuOptions)
            {
                Console.WriteLine($"{option.Value}: {option.Name}");
            }
        }

        public static void PrintMenu(MenuOption[] menuOptions)
        {
            string selectedMenu = "";
            do //while the selected menu is not valid
            {
                PrintMenuOptions(menuOptions);
                selectedMenu = Console.ReadLine() ?? "";
            } while (!menuOptions.Any(option => option.Value == selectedMenu));

            var selectedOption = menuOptions.First(option => option.Value == selectedMenu);
            selectedOption.Action();
        }

        public static void WriteLine(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
}
