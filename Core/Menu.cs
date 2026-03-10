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

            Console.Clear();

            switch (choice)
            {
                case 0:
                    this.startGame();
                    break;
                case 1:
                    this.generateGame();                    
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

            Grid playableGrid = selectedGrid.Clone($"{selectedGrid.Name} (play)", selectedGrid.Difficulty);
            new Game(playableGrid);
            
            Console.WriteLine("The game has been completed.");
            Console.WriteLine("Press any key to return to the menu...");
            Console.ReadKey();
        }

        private void generateGame()
        {
            Console.Clear();
            Console.Write("Enter grid name: ");
            string? gridName = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(gridName))
            {
                Console.WriteLine("Invalid name. Returning to menu...");
                Console.ReadKey();
                return;
            }

            int difficultyIndex = ConsoleHelper.SelectDifficulty();
            string[] difficulties = { "easy", "medium", "hard" };
            string difficulty = difficulties[difficultyIndex];

            Console.Clear();
            Console.WriteLine("Fetching grid from API...");
            Grid? grid = ApiHelper.GetRandomGrid(difficulty).Result;

            if (grid == null)
            {
                Console.WriteLine("Failed to fetch grid from API.");
                Console.WriteLine("Press any key to return to the menu...");
                Console.ReadKey();
                return;
            }

            grid.Name = gridName;
            grid.Difficulty = difficulty.Substring(0, 1).ToUpper() + difficulty.Substring(1);
            bool saveGrid = ConsoleHelper.SelectYesNo("Do you want to save this grid?", () => {this.displayPreview(grid);});

            if (saveGrid)
            {
                GridCollection.AddGrid(grid);
                Console.Clear();
                Console.WriteLine("Grid saved successfully!");
            }
            else
            {
                Console.Clear();
                Console.WriteLine("Grid not saved.");
            }

            Console.WriteLine("Press any key to return to the menu...");
            Console.ReadKey();
        }

        private void displayPreview(Grid grid)
        {
            Console.WriteLine($"Grid: {grid.Name} - {grid.Difficulty}");
            Console.WriteLine();
            grid.Display(false);
        }
    }
}

