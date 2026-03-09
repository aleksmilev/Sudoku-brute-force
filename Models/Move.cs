namespace SudokuBruteForce.Models
{
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

