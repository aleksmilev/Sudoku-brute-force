using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SudokuBruteForce
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Clear();

            string jsonGrid = @"
            [
                [ 2, 0, 0, 0, 1, 0, 0, 6, 0 ],
                [ 0, 0, 0, 0, 0, 7, 0, 0, 9 ],
                [ 0, 1, 0, 0, 0, 0, 0, 0, 0 ],
                [ 0, 7, 0, 0, 0, 3, 0, 2, 0 ],
                [ 0, 3, 0, 0, 0, 0, 0, 0, 0 ],
                [ 0, 0, 0, 7, 0, 0, 9, 0, 3 ],
                [ 7, 0, 4, 0, 0, 0, 1, 0, 0 ],
                [ 9, 2, 0, 0, 0, 6, 0, 0, 0 ],
                [ 0, 8, 1, 0, 5, 9, 0, 0, 7 ]
            ]";

            jsonGrid = @"
            [
                [ 0, 0, 0, 0, 0, 0, 0, 0, 0 ],
                [ 0, 0, 0, 0, 0, 0, 0, 0, 0 ],
                [ 0, 0, 0, 0, 0, 0, 0, 0, 0 ],
                [ 0, 0, 0, 0, 0, 0, 0, 0, 0 ],
                [ 0, 0, 0, 0, 0, 0, 0, 0, 0 ],
                [ 0, 0, 0, 0, 0, 0, 0, 0, 0 ],
                [ 0, 0, 0, 0, 0, 0, 0, 0, 0 ],
                [ 0, 0, 0, 0, 0, 0, 0, 0, 0 ],
                [ 0, 0, 0, 0, 0, 0, 0, 0, 0 ]
            ]";

            Grid test = new Grid(jsonGrid);

            test.Display();

            test.Add(3, new Position(2, 3));

            Console.WriteLine();

            Console.WriteLine("");

            test.Display();

        }
    }

    class Grid
    {
        public List<List<Note>> grid;

        public Grid(string gridJson)
        {
            this.grid = ConvertToNoteGrid(JsonConvert.DeserializeObject<List<List<int>>>(gridJson));
        }

        public static List<List<Note>> ConvertToNoteGrid(List<List<int>> grid)
        {
            // TODO fix this

            List<List<Note>> noteGrid = new List<List<Note>>();

            foreach (var (row, i) in grid.Select((row, i) => new { row, i }))
            {
                List<Note> noteRow = new List<Note>();

                foreach (var (value, j) in row.Select((value, j) => new { value, j }))
                {
                    noteRow.Add(new Note(value, new Position(j, i)));
                }

                noteGrid.Add(noteRow);
            }

            return noteGrid;
        }


        public void Display()
        {
            for (int i = 0; i < this.grid.Count; i++)
            {
                if (i > 0 && i % 3 == 0)
                {
                    Console.WriteLine("──────╂───────╂──────");
                }

                for (int j = 0; j < this.grid[i].Count; j++)
                {
                    if (j > 0 && j % 3 == 0)
                    {
                        Console.Write("│ ");
                    }
                    
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
            if (!(value > 0 && value < 10)) {
                return;
            }

            if (this.grid[position.Y][position.X].value != 0) {
                return;
            }

            int[] sums =  this.GetSumsForNote(position);

            // TODO overflow logic

            this.grid[position.Y][position.X].ChangeValue(value, sums);
        }

        public static bool Compare(Grid grid1, Grid grid2)
        {
            for (int i = 0; i < grid1.grid.Count; i++)
            {
                if (grid1.grid[i].Count != grid2.grid[i].Count)
                    return false;

                for (int j = 0; j < grid1.grid[i].Count; j++)
                {
                    if (grid1.grid[i][j] != grid2.grid[i][j])
                        return false;
                }
            }

            return true;
        }

        public int[] GetSumsForNote(Position position)
        {
            int rowSum = 0;
            int colSum = 0;
            int regionSum = 0;

            return new int[] { rowSum, colSum, regionSum };
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

        public Note(int value, Position position)
        {
            this.value = value;
            this.type = value == 0 ? "null" : "static";
        }

        public ConsoleColor GetColor()
        {
            return Note.TypesToColors[this.type];
        }

        public void ChangeValue(int value, int[] sums)
        {
            if (this.value != 0) 
            {
                return;
            }

            this.value = value;

            int totalSums = sums.Sum();

            if (totalSums < 30)
            {
                this.type = "temp";

            }
            else if (totalSums == 0)
            {
                this.type = "inserted";
            }
            else
            {
                this.type = "null";
                this.value = 0;
            }        
        }
    }
}