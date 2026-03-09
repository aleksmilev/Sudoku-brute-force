namespace SudokuBruteForce.Models
{
    public class Position
    {
        public int X;
        public int Y;

        public Position(int X, int Y)
        {
            this.X = X - 1;
            this.Y = Y - 1;
        }
    }
}

