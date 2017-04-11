using System;

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
                Update(delta: value - Progress); // Call Update() here so we can access the old value of progress and add delta in Update() to get the new.
                _Progress = value;
            }
        }

        private int ProgressChars
        {
            get
            {
                return (int)(Progress / CharWorth);
            }
        }

        private static int Space { get; set; } = Console.WindowWidth;
        private static double ProgressCharsAvail { get; set; } = Space - ($"[] 100%").Length; // the amount of fill we're able to give. subtract 1 and 1 for both braces, and 5 for space then 100% at the end
        private static double CharWorth { get; set; } = 100 / ProgressCharsAvail; // How much progress each character represents
        private static int InitialProgress { get; set; }

        public int Line { get; set; }

        public bool IsDone { get; set; } = false;

        public ProgressBar(int initialProgress, int line) // Constructor
        {
        #if DEBUG
             //Console.WriteLine($"We have {Space} characters available in this console.\n\t- This means that we have {ProgressCharsAvail} fills available.\n\t- Each character is worth {CharWorth}");
        #endif 
             // stats for those curious. Build in Release mode to remove
             // Yes, you need to subtract 2 for it to be accurate. No, I don't know why.
            Line = line;
            Display(initialProgress); // Show for the first time
            Progress = initialProgress; // Set initial progress. Do this after first show because Progress will force an update

        }

        private int GetCharValue(int progressValue)
        {
            return (int)(progressValue / CharWorth);
        }

        private double GetCharValueD(int progressValue)
        {
            return progressValue / CharWorth;
        }

        private void Display(int initialProgress) // First-time display function
        {
            Console.SetCursorPosition(0, Line); // Start of our chosen line to write brace
            Console.Write(BraceL);
            
            Console.Write(new string(Fill, GetCharValue(initialProgress)));
            Console.CursorLeft = Space - 6; // Go to the end and output the right brace,
            // Subtract five too as we need five chars at the end for possible ' 100%'
            Console.Write(BraceR);
            Console.Write($" {initialProgress}%");
        }

        private void Update(int delta)
        {
            Console.CursorTop = Line;
            if (IsDone)
            { return; }
                
            if (delta < 0) { throw new ArgumentException($"Progress change cannot be negative! Current progress {Progress}, delta calculated {delta}"); } // This program does not support going backwards
            // Can't be bothered to delete characters and such. Also what sort of evil progress bar goes backwards?

            int newProgress = Progress + delta;

            if (newProgress > 99) { // if finished (hacks)
                Console.CursorLeft = ProgressChars; // Go to where we left off
                Console.Write(new String(Fill, Space - Console.CursorLeft)); // Fill enough of the new line with the fill to ensure it's full at the end but -1 so no newline

                Console.SetCursorPosition(Line, Space - 6); // Go back to where the brace should be to overwrite excess
                                                            // Prevents possible newlines caused by wrapping from being an issue
                Console.Write($"{BraceR} {newProgress}%");  // Then cut off that last bit with the bracket and percentage value
                IsDone = true;
                return; // prevent the below block of code from executing
            }
            Console.CursorLeft = Space - 4;
            Console.Write($"{newProgress}%"); // Update end progress
            Console.CursorLeft = ProgressChars+1; // Go to the end of the current fill
            Console.Write(new string(Fill, GetCharValue(newProgress) - ProgressChars)); // And add in the required amount of progress. Don't use delta as it causes issues with rounding and can leave gaps.

        }
    }
}
