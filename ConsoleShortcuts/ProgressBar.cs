using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleShortcuts
{
    public class ProgressBar
    {
        public string Braces
        {
            set
            {
                BraceL = value[0]; // Upon setting braces, split the two characters into L and R
                BraceR = value[1];
            }
        }

        private char BraceL { get; set; } = '[';
        private char BraceR { get; set; } = ']';

        public char Fill { get; set; } = '=';

        private int _Progress = 0; // backing field for use with custom accessor
        public int Progress
        {
            get { return _Progress; }
            set
            { 
                Update(value - Progress); // Call Update() here so we can access the old value of progress and add delta in Update() to get the new.
                _Progress = value;

                ProgressChars = value / CharWorth;

            }
        }

        private int ProgressChars { get; set; }

        private static int Space { get; set; } = Console.WindowWidth;
        private static int ProgressCharsAvail { get; set; } = Space - (1 + 1 + 5); // the amount of fill we're able to give. subtract 1 and 1 for both braces, and 5 for space then 100% at the end
        private static int CharWorth { get; set; } = 100 / ProgressCharsAvail; // How much progress each character represents

        public ProgressBar(int initialProgress) // Constructor
        {
            Progress = initialProgress;
            Display();
        }

        private void Display() // First-time display function
        {
            Console.Write(BraceL);
            Console.Write(new string(Fill, ProgressChars));
            Console.CursorLeft = Space - 5; // Go to the end and output the right brace,
            // Subtract five too as we need five chars at the end for possible ' 100%'
            Console.Write(BraceR);
            Console.Write($" {Progress}%");

        }

        private void Update(int delta)
        {
            if (delta < 0) { throw new ArgumentException("Progress change cannot be negative!"); } // This program does not support going backwards
            if (delta + Progress > 101) { // if finished (hacks)
                Console.Write(Fill);
                Console.WriteLine(BraceR);
                Console.WriteLine();
                return;
            }
            Console.CursorLeft = 103;
            Console.Write($"{Progress+delta}%");
            Console.CursorLeft = Progress + 1; // to account for left brace
            Console.Write(new string(Fill, delta));

        }


    }
}
