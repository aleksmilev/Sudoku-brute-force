using Newtonsoft.Json;

namespace SudokuBruteForce
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Clear();

            new Game();
        }
    }

    class Game
    {
        public Grid LoadGrid()
        {
            // TODO Get Grid Form API

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

            // jsonGrid = @"
            // [
            //     [ 0, 0, 0, 0, 0, 0, 0, 0, 0 ],
            //     [ 0, 0, 0, 0, 0, 0, 0, 0, 0 ],
            //     [ 0, 0, 0, 0, 0, 0, 0, 0, 0 ],
            //     [ 0, 0, 0, 0, 0, 0, 0, 0, 0 ],
            //     [ 0, 0, 0, 0, 0, 0, 0, 0, 0 ],
            //     [ 0, 0, 0, 0, 0, 0, 0, 0, 0 ],
            //     [ 0, 0, 0, 0, 0, 0, 0, 0, 0 ],
            //     [ 0, 0, 0, 0, 0, 0, 0, 0, 0 ],
            //     [ 0, 0, 0, 0, 0, 0, 0, 0, 0 ]
            // ]";

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

            this.Solve(grid, 1000);

            Console.WriteLine();
        }

        public bool Solve(Grid grid, int sleep = 0)
        {
            Queue<(int RegionRow, int RegionCol)> skippedRegions = new Queue<(int, int)>();

            for (int regionRow = 0; regionRow < 3; regionRow++)
            {
                for (int regionCol = 0; regionCol < 3; regionCol++)
                {
                    if (!SolveRegion(grid, regionRow, regionCol, sleep, out bool solvedRegion))
                    {
                        skippedRegions.Enqueue((regionRow, regionCol));
                    }
                }
            }

            while (skippedRegions.Count > 0)
            {
                var (regionRow, regionCol) = skippedRegions.Dequeue();

                if (!SolveRegion(grid, regionRow, regionCol, sleep, out bool solvedRegion))
                {
                    skippedRegions.Enqueue((regionRow, regionCol));
                }

                if (skippedRegions.Count == 9)
                    return false;
            }

            return grid.GetUnusedNotes().Length == 0;
        }

        public bool SolveRegion(Grid grid, int regionRow, int regionCol, int sleep, out bool solvedRegion)
        {
            Position[] unusedPositions = grid.GetUnusedNotes(regionRow, regionCol);
            int failCount = 0;

            foreach (Position pos in unusedPositions)
            {
                for (int num = 1; num <= 9; num++)
                {
                    int[] sums = grid.GetSumsForNote(pos, num);
                    int[] usedNotes = grid.GetUsedNotes(pos);

                    if (!usedNotes.Contains(num))
                    {
                        grid.Add(num, pos);
                        grid.Display();
                        Thread.Sleep(sleep);

                        if (Solve(grid, sleep))
                        {
                            solvedRegion = true;
                            return true;
                        }

                        grid.RevertTransaction();
                        failCount++;

                        if (failCount >= unusedPositions.Length)
                        {
                            solvedRegion = false;
                            return false;
                        }
                    }
                }
            }

            solvedRegion = false;
            return false;
        }

    }


    class Grid
    {
        public static int MaxSum = 45;
        public Stack<List<Move>> Transactions = new Stack<List<Move>>();
        public List<List<Note>> grid;

        public Grid(string gridJson)
        {
            this.grid = ConvertToNoteGrid(JsonConvert.DeserializeObject<List<List<int>>>(gridJson) ?? new List<List<int>>());
            this.Transactions.Push(new List<Move>());
        }

        public static List<List<Note>> ConvertToNoteGrid(List<List<int>> grid)
        {
            List<List<Note>> noteGrid = new List<List<Note>>();

            for (int i = 0; i < grid.Count; i++)
            {
                List<Note> noteRow = new List<Note>();
                for (int j = 0; j < grid[i].Count; j++)
                    noteRow.Add(new Note(grid[i][j]));
                noteGrid.Add(noteRow);
            }

            return noteGrid;
        }

        public void Display(bool clearBefore = true)
        {
            if (clearBefore)
                Console.Clear();

            for (int i = 0; i < this.grid.Count; i++)
            {
                if (i > 0 && i % 3 == 0)
                    Console.WriteLine("──────╂───────╂──────");

                for (int j = 0; j < this.grid[i].Count; j++)
                {
                    if (j > 0 && j % 3 == 0)
                        Console.Write("│ ");
                    
                    if (this.grid[i][j].value == 0)
                    {
                        Console.ForegroundColor = this.grid[i][j].GetColor();
                        Console.Write("● ");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.ForegroundColor = this.grid[i][j].GetColor(); 
                        Console.Write(this.grid[i][j].value + " ");
                        Console.ResetColor();
                    }
                }

                Console.WriteLine();
            }
        }

        public void Add(int value, Position position)
        {
            if (!(value > 0 && value < 10))
                return;

            if (this.grid[position.Y][position.X].value != 0)
                return;

            int[] sums =  this.GetSumsForNote(position, value);
            int[] usedNotes = this.GetUsedNotes(position);

            this.grid[position.Y][position.X].ChangeValue(value, sums, usedNotes);   
            this.Transactions.Peek().Add(new Move(value, position));

            if (sums.Any(sum => sum > Grid.MaxSum) || usedNotes.Contains(value))
                this.RevertTransaction();

            if (sums.Sum() == Grid.MaxSum * 3)
                this.CommitTransaction();
            

        }

        // TODO Fix the RevertTransaction condition
        public void RevertTransaction()
        {
            List<Move> moves = this.Transactions.Peek();

            foreach (Move move in moves)
                this.grid[move.position.Y][move.position.X].Reset();

            this.Transactions.Pop();

            if (this.Transactions.Count == 0) 
                this.Transactions.Push(new List<Move>());
        }

        public void CommitTransaction()
        {
            List<Move> moves = this.Transactions.Peek();

            foreach (Move move in moves)
                this.grid[move.position.Y][move.position.X].SetAsInserted();

            this.Transactions.Push(new List<Move>());
        }

        public static bool Compare(Grid grid1, Grid grid2)
        {
            for (int i = 0; i < grid1.grid.Count; i++)
            {
                if (grid1.grid[i].Count != grid2.grid[i].Count)
                    return false;

                for (int j = 0; j < grid1.grid[i].Count; j++)
                    if (grid1.grid[i][j] != grid2.grid[i][j])
                        return false;
            }

            return true;
        }

        public int[] GetSumsForNote(Position position, int value)
        {
            int rowSum = value;
            int colSum = value;
            int regionSum = value;

            int gridSize = this.grid.Count;
            int regionStartRow = (position.Y / 3) * 3;
            int regionStartCol = (position.X / 3) * 3;

            for (int i = 0; i < gridSize; i++)
            {
                rowSum += this.grid[position.Y][i].value;
                colSum += this.grid[i][position.X].value;
            }

            for (int i = regionStartRow; i < regionStartRow + 3; i++)
                for (int j = regionStartCol; j < regionStartCol + 3; j++)
                    regionSum += this.grid[i][j].value;

            return new int[] { rowSum, colSum, regionSum };
        }

        public int[] GetUsedNotes(Position position)
        {
            List<int> usedNoes = new List<int>();

            int gridSize = this.grid.Count;
            int regionStartRow = (position.Y / 3) * 3;
            int regionStartCol = (position.X / 3) * 3;

            for (int i = 0; i < gridSize; i++)
            {
                if (this.grid[position.Y][i].value != 0)
                    usedNoes.Add(this.grid[position.Y][i].value);

                if (this.grid[i][position.X].value != 0)
                    usedNoes.Add(this.grid[i][position.X].value);                
            }

            for (int i = regionStartRow; i < regionStartRow + 3; i++)
                for (int j = regionStartCol; j < regionStartCol + 3; j++)
                    if (this.grid[i][j].value != 0) 
                        usedNoes.Add(this.grid[i][j].value);

            return usedNoes.ToArray();
        }

        public Position[] GetUnusedNotes(int regionRow = 0, int regionCol = 0)
        {
            List<Position> unusedNotes = new List<Position>();

            int startRow = regionRow * 3;
            int startCol = regionCol * 3;

            for (int i = startRow; i < startRow + 3; i++)
                for (int j = startCol; j < startCol + 3; j++)
                    if (this.grid[i][j].value == 0)
                        unusedNotes.Add(new Position(j + 1, i + 1));

            return unusedNotes.ToArray();
        }

    }

    class Position
    {
        public int X;
        public int Y;

        public Position(int X, int Y)
        {
            this.X = X - 1;
            this.Y = Y - 1;
        }
    }

    class Note
    {
        public int value;
        public string type;

        public static Dictionary<string, ConsoleColor> TypesToColors = new Dictionary<string, ConsoleColor>
        {
            {"static", ConsoleColor.White},
            {"null", ConsoleColor.Green},
            {"temp", ConsoleColor.Red},
            {"inserted", ConsoleColor.Blue}
        };

        public Note(int value)
        {
            this.value = value;
            this.type = value == 0 ? "null" : "static";
        }

        public ConsoleColor GetColor()
        {
            return Note.TypesToColors[this.type];
        }

        public void SetAsInserted()
        {
            this.type = "inserted";
        }

        public void ChangeValue(int value, int[] sums, int[] usedNotes)
        {
            if (this.value != 0 || value == 0 || usedNotes.Contains(value)) 
                return;

            this.value = value;

            if (sums.Any(sum => sum < Grid.MaxSum))
                this.type = "temp";
            else
            {
                this.type = "null";
                this.value = 0;
            }
        }

        public void Reset()
        {
            this.type = "null";
            this.value = 0;
        }
    }

    class Move
    {
        public int value;
        public Position position;

        public Move(int value, Position position)
        {
            this.value = value; 
            this.position = position;
        }
    }
}