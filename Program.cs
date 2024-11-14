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
                [0, 0, 0, 0, 0, 0, 0, 0, 0],
                [0, 0, 0, 0, 0, 0, 0, 0, 0],
                [0, 0, 0, 0, 0, 0, 0, 0, 0],
                [0, 0, 0, 0, 0, 0, 0, 0, 0],
                [0, 0, 0, 0, 0, 0, 0, 0, 0],
                [0, 0, 0, 0, 0, 0, 0, 0, 0],
                [0, 0, 0, 0, 0, 0, 0, 0, 0],
                [0, 0, 0, 0, 0, 0, 0, 0, 0],
                [0, 0, 0, 0, 0, 0, 0, 0, 0]
            ]";
            Grid test = new Grid(jsonGrid);

            test.Display();

            Console.WriteLine(test.Add(3, 2, 5));

            Console.WriteLine("");

            test.Display();

        }
    }

    class Grid
    {
        public List<List<int>> grid;

        public Grid(string gridJson)
        {
            this.grid = JsonConvert.DeserializeObject<List<List<int>>>(gridJson);
        }

        public void Display()
        {
            for (int i = 0; i < grid.Count; i++)
            {
                if (i > 0 && i % 3 == 0)
                {
                    Console.WriteLine("──────╂───────╂──────");
                }

                for (int j = 0; j < grid[i].Count; j++)
                {
                    if (j > 0 && j % 3 == 0)
                    {
                        Console.Write("│ ");
                    }
                    
                    if (grid[i][j] == 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("● ");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.ResetColor(); 
                        Console.Write(grid[i][j] + " ");
                    }
                }

                Console.WriteLine();
            }
        }

        public string Add(int value, int X, int Y)
        {
            if (!(value > 0 && value < 10)) {
                return "false1";
            }

            if (this.grid[Y-1][X-1] != 0) {
                return "false2";
            }

            this.grid[Y-1][X-1] = value;
            return "true3";
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
    }
}