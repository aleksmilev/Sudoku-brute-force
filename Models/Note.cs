namespace SudokuBruteForce.Models
{
    public class Note
    {
        public int value;
        public string type;

        public static readonly Dictionary<string, ConsoleColor> TypesToColors = new Dictionary<string, ConsoleColor>
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

            if (sums.Any(sum => sum > Grid.MaxSum))
            {
                this.type = "null";
                this.value = 0;
                return;
            }

            if (sums.All(sum => sum == Grid.MaxSum))
            {
                this.type = "temp";
            }
            else if (sums.Any(sum => sum < Grid.MaxSum))
            {
                this.type = "temp";
            }
        }

        public void Reset()
        {
            this.type = "null";
            this.value = 0;
        }
    }
}

