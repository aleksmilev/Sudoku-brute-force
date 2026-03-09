using SudokuBruteForce.Models;

namespace SudokuBruteForce.Core
{
    public class Menu
    {
        public Menu()
        {
            Api.GetDefaultGrid();
            this.newGame();
        }

        private void newGame()
        {
            Console.Clear();
            Grid grid = GridCollection.GetGrid("Default");
            new Game(grid);
        }
    }
}

