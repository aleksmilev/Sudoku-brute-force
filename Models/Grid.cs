using Newtonsoft.Json;

namespace SudokuBruteForce.Models
{
    public class Grid
    {
        public string Name;
        public string Difficulty;
        
        public const int MaxSum = 45;
        public Stack<List<Move>> Transactions = new Stack<List<Move>>();
        private List<List<Note>> grid;

        public Grid(string gridJson, string name = "Default", string difficulty = "Easy", bool isTemporary = true)
        {
            this.grid = ConvertToNoteGrid(JsonConvert.DeserializeObject<List<List<int>>>(gridJson) ?? new List<List<int>>());
            this.Transactions.Push(new List<Move>());

            this.Name = name;
            this.Difficulty = difficulty;

            if (!isTemporary)
                GridCollection.AddGrid(this);
        }

        public void Save()
        {
            GridCollection.AddGrid(this);
        }

        public Grid Clone(string? name = null, string? difficulty = null)
        {
            List<List<int>> gridData = new List<List<int>>();

            for (int i = 0; i < this.grid.Count; i++)
            {
                List<int> row = new List<int>();
                for (int j = 0; j < this.grid[i].Count; j++)
                {
                    Note note = this.grid[i][j];
                    int value = note.type == "static" ? note.value : 0;
                    row.Add(value);
                }
                gridData.Add(row);
            }

            string gridJson = JsonConvert.SerializeObject(gridData);
            return new Grid(
                gridJson,
                name ?? this.Name,
                difficulty ?? this.Difficulty,
                isTemporary: true
            );
        }

        private static List<List<Note>> ConvertToNoteGrid(List<List<int>> grid)
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

            int[] sums = this.GetSumsForNote(position, value);
            int[] usedNotes = this.GetUsedNotes(position);

            if (sums.Any(sum => sum > Grid.MaxSum) || usedNotes.Contains(value))
                return;

            if (this.Transactions.Count == 0)
                this.Transactions.Push(new List<Move>());

            this.grid[position.Y][position.X].ChangeValue(value, sums, usedNotes);   
            this.Transactions.Peek().Add(new Move(value, position));
        }

        public void RevertTransaction()
        {
            if (this.Transactions.Count == 0)
                return;

            List<Move> moves = this.Transactions.Peek();

            int moveCount = moves.Count;
            for (int i = moveCount - 1; i >= 0; i--)
            {
                Move move = moves[i];
                if (this.grid[move.position.Y][move.position.X].type != "inserted" && 
                    this.grid[move.position.Y][move.position.X].type != "static")
                {
                    this.grid[move.position.Y][move.position.X].Reset();
                }
            }

            moves.Clear();
        }

        private void CommitTransaction()
        {
            if (this.Transactions.Count == 0)
                return;

            List<Move> moves = this.Transactions.Peek();

            foreach (Move move in moves)
                this.grid[move.position.Y][move.position.X].SetAsInserted();

            moves.Clear();
        }

        public void CommitAllTransactions()
        {
            while (this.Transactions.Count > 0)
            {
                List<Move> moves = this.Transactions.Pop();
                
                foreach (Move move in moves)
                {
                    if (this.grid[move.position.Y][move.position.X].type != "static")
                    {
                        this.grid[move.position.Y][move.position.X].SetAsInserted();
                    }
                }
            }
            
            this.Transactions.Push(new List<Move>());
        }

        private static bool Compare(Grid grid1, Grid grid2)
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
            int rowSum = 0;
            int colSum = 0;
            int regionSum = 0;

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

            int currentValue = this.grid[position.Y][position.X].value;
            rowSum = rowSum - currentValue + value;
            colSum = colSum - currentValue + value;
            regionSum = regionSum - currentValue + value;

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

        public Position[] GetUnusedNotes(int? regionRow = null, int? regionCol = null)
        {
            List<Position> unusedNotes = new List<Position>();

            if (regionRow.HasValue && regionCol.HasValue)
            {
                int startRow = regionRow.Value * 3;
                int startCol = regionCol.Value * 3;

                for (int i = startRow; i < startRow + 3; i++)
                    for (int j = startCol; j < startCol + 3; j++)
                        if (this.grid[i][j].value == 0)
                            unusedNotes.Add(new Position(j + 1, i + 1));
            }
            else
            {
                for (int i = 0; i < this.grid.Count; i++)
                    for (int j = 0; j < this.grid[i].Count; j++)
                        if (this.grid[i][j].value == 0)
                            unusedNotes.Add(new Position(j + 1, i + 1));
            }

            return unusedNotes.ToArray();
        }

    }
}

