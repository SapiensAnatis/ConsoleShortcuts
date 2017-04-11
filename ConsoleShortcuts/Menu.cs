using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleShortcuts
{
    public struct Result
    {
        public int Index { get; set; }
        public string Text { get; set; }

        public Result(int index, string text)
        {
            Index = index; Text = text; 
        }
        // You could also extend this struct to make it even further better than a tuple
        // by including metrics such as the time taken to make that decision.
    }

    public abstract class BaseMenu
    {
        // --- Properties
        // Content
        public string Prompt { get; set; } // What the user will be 'asked'. This is public in case the prompt needs to be changed if the menu is being drawn multiple times.
        protected string[] Options { get; set; } // The GetSelection() methods will return a tuple with string and index, so this needs not to be exposed. Protected as inheritor classes use it.

        public BaseMenu(string prompt, params string[] options) // Params[] allows us to initialize a menu and call a large number of objects, without the overhead of first making an array to pass.
        {
            Prompt = prompt;
            Options = options;
            Console.CursorVisible = false;
        }
    }

    public class KeyboardMenu : BaseMenu
    {
        // Settings
        public bool ShowNumbers { get; set; } // Whether or not to show the numbers next to options. We may want to change this externally

        public KeyboardMenu(string prompt, params string[] options) : base(prompt, options)
        {
            if (Options.Length > 9) { throw new System.ArgumentOutOfRangeException("Keyboard menus can only support up to 9 elements!"); }
            /* It's true! There are only 9 numeric keys to press because we're excluding zeroes. */

            ShowNumbers = true; // ShowNumbers must be true for this type of menu as the user needs to know what to press.
            this.Display(); // Display menu, an important part of having the user make a decision.
        }

        private void Display() // Not in base menu class as both classes handle differently.
        {
            Console.WriteLine($"{Prompt}");
            /* Loop through the Options array. Did not use foreach because we need index numbers accessible for printing option numbers.
               I prefer to do this rather than use .IndexOf() as IndexOf doesn't always work ideally with duplicates; it returns the index of only the first occurence.
               While this is a menu and there shouldn't be duplicates for obvious reasons, it's not that much hassle to do it this way and add support for that. */
            for (int i = 0; i < Options.Length; i++)
            {
                if (ShowNumbers) { Console.Write($"\t{i + 1}: "); } // Write the number in front only if we're showing numbers. i+1 as arrays are zero indexed but 1-index is more user-friendly.

                Console.WriteLine(Options[i]); // Then write the content
                // Use WriteLine() so that we then proceed to a newline for the next option.
            }
        }

        public Tuple<int, string> GetSelection()
        {
            
            char c = Console.ReadKey(true).KeyChar; // true arg hides input which gives the menu a cleaner look
            while (!ValidateIsOption(c)) { c = Console.ReadKey(true).KeyChar; } // Get stuck in a while loop until the user enters a valid key.

            int cInt = (int)Char.GetNumericValue(c); // Once we've validated it's a number 1-9, get the number.

            Console.Clear();
            return Tuple.Create(cInt-1, Options[cInt - 1]); // Subtract one to give as indices of the array
        }

        private bool ValidateIsOption(char input)
        {
            /* The way we have to validate digits is interesting.
               Several built-in methods exist, but these are actually unsuitable:
               - Char.IsNumber(), which is the top-level numeric validation function for chars, would return true for Unicode characters such as ¼ and ², which we aren't considering valid here.
               - Char.IsDigit(), which is a subset of Char.IsNumber() that excludes the above examples, apparently includes digits from other languages such as Thai: ๖, ๑, ๙, etc.
               So the absolute way to determine it is 1-9, or what we want, is by simply whitelisting only those characters in a string.
            */
            if ("123456789".Contains(input)) 
            {
                int numChar = (int)Char.GetNumericValue(input); // This is nested, because only once we confirm that the character is a valid digit can we know that it's safe to convert to an integer and start doing int operations
                // It requires a cast to int because it's initially a double--those aforementioned Unicode fractions would return 0.25 for instance
                if (numChar > 0 && numChar <= Options.Length) { return true; } // Only if both selection criteria are met return true. This is the 'int operation'
            }

            return false; // Otherwise return false.
        }
    }

    public class HighlightMenu : BaseMenu // N.B. ShowNumbers is unsupported on this menu. Not likely to be implemented in future. Can't be bothered, it's somehow more difficult than it sounds.
    {
        // Options (can be overriden with field initializer, hence they are public)
        public int HighlightLength { get; set; } // Default highlight length..
        public int HighlightTrail { get; set; } = 10; // The amount of spaces to trail with highlight after an option. 

        private ConsoleColor HighlightBGColor { get; set; } = Console.ForegroundColor;
        private ConsoleColor HighlightFGColor { get; set; } = Console.BackgroundColor;


        // Private internal vars
        private int _SelectedOptionIndex = 0; // backing field to avoid recursion in custom get; set; statement
        private int SelectedOptionIndex // This will be returned by GetSelection() anyway; there's no need to expose it.
        {
            get { return _SelectedOptionIndex; }
            set // A bunch of logic for redrawing the selected option (not redrawing the whole menu)
            {
                // Once the selected option is changed, rather than overwrite the whole menu and redraw it with new highlights, we simply overwrite what we need to.
                // First, we must erase the highlight of the current selected option:

                Console.SetCursorPosition(0, OptionZeroLine + SelectedOptionIndex); // Go to the line of our current (old) selected option

                Console.Write($"\t  {Options[_SelectedOptionIndex]}"); // Overwrite the option 
                Console.WriteLine("".PadRight(Console.WindowWidth)); // then remove any highlight trail. PadRight will not cause newlines.

                // Now, we target our newly selected option and give it a highlight.
                Console.SetCursorPosition(0, value + OptionZeroLine);
                PrintHighlightedOption(Options[value]);

                _SelectedOptionIndex = value; // Finally update backing field to keep it accurate
                
            }
        }
        private string SelectedOption
        {
            get { return Options[SelectedOptionIndex]; }
        }


        private int OptionZeroLine { get; set; } // We can't always assume that our menu has the luxury of being at the top of the console.
        public int LastOccupiedLine
        {
            get { return OptionZeroLine + Options.Length - 1; } // minus one: Option zero, our ref point, also contributes to the length.
        }

        // Methods
        private void PrintHighlightedOption(string Option)
        {
            Console.Write("\t"); // We're writing the tab outside of the main print for stlystic reasons; we want the highlight to have a left margin a little bit.
            Console.BackgroundColor = HighlightBGColor;
            Console.ForegroundColor = HighlightFGColor;
            Console.WriteLine($"  {Option}".PadRight(HighlightLength)); // Print the option with highlighted (white) background, using this method.
            // PadRight provides some common trailing whitespace. Interestingly enough it creates a common margin, no matter the length of the preceding string.
            // Very smart function
            Console.ResetColor(); // clear up for whatever we're doing next
        }

        public void Display() // First time draw, draw out the whole menu. Don't use more than once.
        {
            Console.WriteLine(Prompt);

            foreach (string Option in Options)
            {
                if (Option == SelectedOption)
                    PrintHighlightedOption(Option);
                else
                    Console.WriteLine($"\t  {Option}");
            }

        }

        public Result GetSelection() // This method should be public as we need to return values back up to the code where we're instancing the menu.
        {
            var input = Console.ReadKey(true);
            while (input.Key != ConsoleKey.Enter) // I might want to get input in a slightly better way in future.
            {
                // Normal input handlers
                if (input.Key == ConsoleKey.UpArrow && SelectedOptionIndex > 0)
                    { SelectedOptionIndex--; }
                else if (input.Key == ConsoleKey.DownArrow && SelectedOptionIndex < Options.Length - 1)
                    { SelectedOptionIndex++; }

                // Handlers for input in the event that the user is at the top or bottom, in which case we must roll over:
                else if (input.Key == ConsoleKey.UpArrow && SelectedOptionIndex == 0)
                    { SelectedOptionIndex = Options.Length - 1; }
                else if (input.Key == ConsoleKey.DownArrow && SelectedOptionIndex == Options.Length - 1)
                    { SelectedOptionIndex = 0; }

                input = Console.ReadKey(true); // Get a new key and repeat the loop

            }
            // Once we have an enter, finish up and return:
            return new Result(SelectedOptionIndex, SelectedOption); // Return the string and the option number to minimize work done after result has been returned.
        }

        public HighlightMenu(string prompt, params string[] options) : base(prompt, options)
        {
            OptionZeroLine = Console.CursorTop+1; // We look at where we are on creation and take that as our base level for all editing.
                                                  // We add one to include the line of prompt that we'll be writing.
            // If HighlightLength isn't long enough for all of our options:
            string LongestOption = Options.OrderByDescending(s => s.Length).First();
            HighlightLength = LongestOption.Length + HighlightTrail;

            // We don't auto-display so that menus can be pre-created.
        }

       
    }
}
