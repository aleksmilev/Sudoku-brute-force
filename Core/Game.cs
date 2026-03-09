using SudokuBruteForce.Models;

namespace SudokuBruteForce.Core
{
    class Game
    {
        public Grid LoadGrid()
        {
            string jsonGrid = @"
            [
                [ 5, 6, 1, 0, 4, 0, 7, 0, 0 ],
                [ 0, 9, 0, 0, 2, 0, 1, 0, 8 ],
                [ 0, 0, 0, 0, 0, 0, 0, 6, 0 ],
                [ 8, 0, 0, 2, 0, 6, 0, 0, 0 ],
                [ 6, 0, 0, 1, 0, 9, 2, 0, 4 ],
                [ 9, 0, 2, 7, 0, 4, 5, 0, 6 ],
                [ 1, 2, 0, 0, 0, 0, 8, 0, 7 ],
                [ 0, 5, 6, 8, 7, 2, 0, 0, 9 ],
                [ 4, 0, 0, 5, 0, 1, 6, 2, 0 ]
            ]";

            Grid grid = new Grid(jsonGrid);

            grid.Display();

            Console.WriteLine();
            Console.Write("Press any key to start:");
            Console.Read();

            return grid;
        }

        public Game()
        {
            Grid grid = this.LoadGrid();

            grid.Display();
            Thread.Sleep(1000);

            bool solved = this.Solve(grid, 100);
            
            if (solved)
            {
                grid.CommitAllTransactions();
                grid.Display();
            }

            Console.WriteLine();
        }

        public bool Solve(Grid grid, int sleep = 0)
        {
            Position[] unusedPositions = grid.GetUnusedNotes();
            if (unusedPositions.Length == 0)
                return true;

            Position nextPos = unusedPositions[0];

            grid.Transactions.Push(new List<Move>());

            for (int num = 1; num <= 9; num++)
            {
                int[] sums = grid.GetSumsForNote(nextPos, num);
                int[] usedNotes = grid.GetUsedNotes(nextPos);

                if (!usedNotes.Contains(num) && !sums.Any(sum => sum > Grid.MaxSum))
                {
                    grid.Add(num, nextPos);
                    grid.Display();
                    Thread.Sleep(sleep);

                    if (Solve(grid, sleep))
                    {
                        return true;
                    }

                    grid.RevertTransaction();
                }
            }

            if (grid.Transactions.Count > 0)
                grid.Transactions.Pop();
            return false;
        }

    }
}

