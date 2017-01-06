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

                ProgressChars = (int)Math.Floor(value / CharWorth);
            }
        }

        private int ProgressChars { get; set; }

        private static int Space { get; set; } = Console.WindowWidth;
        private static double ProgressCharsAvail { get; set; } = Space - (1 + 1 + 5); // the amount of fill we're able to give. subtract 1 and 1 for both braces, and 5 for space then 100% at the end
        private static double CharWorth { get; set; } = 100 / ProgressCharsAvail; // How much progress each character represents

        
        public ProgressBar(int initialProgress) // Constructor
        {
            #if DEBUG
            Console.WriteLine($"We have {Space} characters available in this console.\n\t- This means that we have {ProgressCharsAvail} fills available.\n\t- Each character is worth {CharWorth}");
            #endif // stats for those curious. Build in Release mode to remove

            Progress = initialProgress; // Set initial progress
            Display(); // and show for the first time
        }

        private void Display() // First-time display function
        {
            Console.CursorLeft = 0;
            Console.Write(BraceL);
            Console.Write(new string(Fill, ProgressChars));
            Console.CursorLeft = Space - 6; // Go to the end and output the right brace,
            // Subtract five too as we need five chars at the end for possible ' 100%'
            Console.Write(BraceR);
            Console.Write($" {Progress}%");
        }

        private void Update(int delta)
        {
            if (delta < 0) { throw new ArgumentException("Progress change cannot be negative!"); } // This program does not support going backwards

            int newProgress = Progress + delta;

            if (newProgress > 99) { // if finished (hacks)
                Console.Write(Fill);
                Console.Write($"{BraceR} {newProgress}%");
                return;
            }
            Console.CursorLeft = Space - 4;
            Console.Write($"{newProgress}%");
            Console.CursorLeft = ProgressChars + 1; // to account for left brace
            Console.Write(new string(Fill, delta));
        }
    }
}
