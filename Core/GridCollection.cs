using SudokuBruteForce.Models;

namespace SudokuBruteForce.Core
{
    public static class GridCollection
    {
        public static Dictionary<string, Grid> Grids = new Dictionary<string, Grid>();

        public static Grid GetGrid(string name)
        {
            return Grids.FirstOrDefault(grid => grid.Key == name).Value ?? throw new Exception($"Grid {name} not found");
        }

        public static void AddGrid(Grid grid)
        {
            Grids.Add(grid.Name, grid);
        }

        public static List<string> ListAllGrids()
        {
            return Grids.Values.Select(grid => $"{grid.Name} - {grid.Difficulty}").ToList();
        }
    }
}

