using SudokuBruteForce.Models;

namespace SudokuBruteForce.Core
{
    public class Game
    {
        public Game(Grid grid)
        {
            grid.Display();

            Console.WriteLine();
            Console.Write("Press any key to start:");
            Console.Read();

            Thread.Sleep(1000);

            bool solved = this.Solve(grid, 10);
            
            if (solved)
            {
                grid.CommitAllTransactions();
                grid.Display();
            }

            Console.WriteLine();
        }

        private bool Solve(Grid grid, int sleep = 0)
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

