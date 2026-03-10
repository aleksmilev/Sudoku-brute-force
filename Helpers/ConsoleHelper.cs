using SudokuBruteForce.Core;
using SudokuBruteForce.Models;

namespace SudokuBruteForce.Helpers
{
    public static class ConsoleHelper
    {
        public static int SelectItem(List<string> items, string title, Action? displayPreviousAction = null)
        {
            if (items.Count == 0)
            {
                return -1;
            }

            int selectedIndex = 0;
            ConsoleKey key;

            do
            {
                Console.Clear();

                if (displayPreviousAction != null)
                {
                    displayPreviousAction();
                    Console.WriteLine();
                }

                Console.WriteLine(title);
                Console.WriteLine();

                for (int i = 0; i < items.Count; i++)
                {
                    if (i == selectedIndex)
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.Write("[●] ");
                        Console.ResetColor();
                        Console.WriteLine(items[i]);
                    }
                    else
                    {
                        Console.Write("[○] ");
                        Console.WriteLine(items[i]);
                    }
                }

                Console.WriteLine();
                Console.WriteLine("Use ↑/↓ arrow keys to navigate, Enter to select");

                key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        selectedIndex = (selectedIndex - 1 + items.Count) % items.Count;
                        break;
                    case ConsoleKey.DownArrow:
                        selectedIndex = (selectedIndex + 1) % items.Count;
                        break;
                }
            } while (key != ConsoleKey.Enter);

            Console.ResetColor();
            return selectedIndex;
        }

        public static Grid? SelectGrid()
        {
            List<string> gridNames = GridCollection.Grids.Keys.ToList();
            
            if (gridNames.Count == 0)
            {
                Console.WriteLine("No grids available.");
                Console.WriteLine("Press any key to return to the menu...");
                Console.ReadKey();
                return null;
            }

            List<string> gridDisplays = gridNames.Select(name =>
            {
                Grid grid = GridCollection.Grids[name];
                return $"{grid.Name} - {grid.Difficulty}";
            }).ToList();

            int selectedIndex = SelectItem(gridDisplays, "Select a grid to start:");

            if (selectedIndex == -1)
            {
                return null;
            }

            return GridCollection.Grids[gridNames[selectedIndex]];
        }

        public static int SelectMenuOption()
        {
            List<string> options = new List<string>
            {
                "Generate Game",
                "Start Game",
                "Exit"
            };

            return SelectItem(options, "Select an option:");
        }

        public static int SelectDifficulty()
        {
            List<string> difficulties = new List<string>
            {
                "Easy",
                "Medium",
                "Hard"
            };

            return SelectItem(difficulties, "Select difficulty:");
        }

        public static bool SelectYesNo(string question, Action? displayPreviousAction = null)
        {
            List<string> options = new List<string>
            {
                "Yes",
                "No"
            };

            int selectedIndex = SelectItem(options, question, displayPreviousAction);
            return selectedIndex == 0;
        }

        public static bool SelectYesNoInline(string question)
        {
            Console.WriteLine();
            Console.WriteLine(question + " (Y/N)");

            while (true)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                if (keyInfo.Key == ConsoleKey.Y)
                {
                    Console.WriteLine("Y");
                    return true;
                }
                if (keyInfo.Key == ConsoleKey.N)
                {
                    Console.WriteLine("N");
                    return false;
                }
            }
        }
    }
}

