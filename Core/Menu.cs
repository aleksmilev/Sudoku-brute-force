using SudokuBruteForce.Models;
using SudokuBruteForce.Helpers;

namespace SudokuBruteForce.Core
{
    public class Menu
    {
        public Menu()
        {
            ApiHelper.GetDefaultGrid();
            this.SelectOption();
        }

        private void SelectOption()
        {
            int choice = ConsoleHelper.SelectMenuOption();

            switch (choice)
            {
                case 0:
                    this.generateGame();
                    break;
                case 1:
                    this.startGame();
                    break;
                case 2:
                default:
                    Environment.Exit(0);
                    break;
            }

            this.SelectOption();
        }

        private void startGame()
        {
            Grid? selectedGrid = ConsoleHelper.SelectGrid();

            if (selectedGrid == null)
            {
                return;
            }

            new Game(selectedGrid);
            
            Console.WriteLine("The game has been completed.");
            Console.WriteLine("Press any key to return to the menu...");
            Console.ReadKey();
        }

        private void generateGame()
        {
            Console.Clear();
            this.SelectOption();
        }
    }
}

