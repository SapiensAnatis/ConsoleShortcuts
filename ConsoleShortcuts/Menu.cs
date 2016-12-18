using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleShortcuts
{
    public abstract class BaseMenu
    {
        // --- Properties
        // Content
        public string Prompt { get; set; } // What the user will be 'asked'. This is public in case the prompt needs to be changed if the menu is being drawn multiple times.
        protected string[] Options { get; set; } // The GetSelection() methods will return a tuple with string and index, so this needs not to be exposed. Protected as inheritor classes use it.
        // Settings
        public bool ShowNumbers { get; set; } // Whether or not to show the numbers next to options. Again we may want to change this outside of this code, so it's not private.

        public BaseMenu(string prompt, params string[] options) // Params[] allows us to initialize a menu and call a large number of objects, without the overhead of first making an array to pass.
        {
            Prompt = prompt;
            Options = options;
            Console.CursorVisible = false;
            
        }

        protected virtual void Display() // The HighlightMenu will need to override this.
        {
            Console.WriteLine($"{Prompt}");
            /* Loop through the Options array. Did not use foreach because we need index numbers accessible for printing option numbers.
               I prefer to do this rather than use .IndexOf() as IndexOf doesn't always work ideally with duplicates; it returns the index of only the first occurence.
               While this is a menu and there shouldn't be duplicates for obvious reasons, it's not that much hassle to do it this way and add support for that. */
            for (int i = 0; i < Options.Length; i++) 
            {                                                                       
                if (ShowNumbers) { Console.Write($"\t{i+1}: "); } // Write the number in front only if we're showing numbers. i+1 as arrays are zero indexed but 1-index is more user-friendly.
               
                Console.WriteLine(Options[i]); // Then write the content
                // Use WriteLine() so that we then proceed to a newline for the next option.
            }
        } 
    }

    public class KeyboardMenu : BaseMenu
    {
        public KeyboardMenu(string prompt, params string[] options) : base(prompt, options)
        {
            if (Options.Length > 9) { throw new System.ArgumentOutOfRangeException("Keyboard menus can only support up to 9 elements!"); }
            /* It's true! There are only 9 numeric keys to press because we're excluding zeroes.
               In my mind, not excluding zeroes makes it harder to understand for the user.
               Including the option of whether or not to exclude zero would extend the code by a lot.
               In any case, this code is mostly for my own personal use so it's what I think is best.
               And I can't imagine any case scenario where I'd need more than 9 elements in this type of menu. */

            ShowNumbers = true; // ShowNumbers must be true for this type of menu as the user needs to know what to press.
            this.Display(); // Display menu, an important part of having the user make a decision.
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

    public class HighlightMenu : BaseMenu
    {
        private int _selectedOption = 0; // backing field to avoid recursion in custom get; set; statement
        private int SelectedOption
        {
            get { return _selectedOption; }
            set
            {
                // Once the selected option is changed, rather than overwrite the whole menu and redraw it with new highlights, we simply overwrite what we need to.
                // First, we must erase the highlight of the current selected option:
                Console.SetCursorPosition(0, SelectedOption + offset); // line number = index+1 due to prompt.
                int ToEndOfWin = Console.WindowWidth; // It should be impossible for this to run over to new lines. I've not been able to make it happen.
                Console.WriteLine($"\t  {Options[_selectedOption]}".PadRight(ToEndOfWin)); // Write over but without highlight
                // This is different to writing a normal non-highlighted option as we need to clear the trailing whitespace to the right that would've previously been added.

                // Now, we target our newly selected option and give it a highlight.
                Console.SetCursorPosition(0, value + offset);
                PrintHighlightedOption(Options[value]);

                _selectedOption = value; // Finally update backing field to keep it accurate
                
            }
        }
        // This will be returned by GetSelection() anyway; there's no need to expose it.
        public int HighlightLength { get; set; } = 50; // Default highlight length. Can be changed in a collection initializer if so desired.
        public bool ClearAfterSelection { get; set; } = true; // Whether or not to clear the console after the user presses Enter.
        private int offset { get; set; }

        public HighlightMenu(string prompt, params string[] options) : base(prompt, options)
        {
            offset = Console.CursorTop+1;
            // No idea why, but Console.CursorTop reports a number that is equal to the line the cursor is on +5.
            // We add one to include the prompt that we'll be writing.

            // If HighlightLength isn't long enough for all of our options:
            int max = HighlightLength;
            foreach (string Option in Options) {
                if (Option.Length > max) { max = Option.Length; }
            }
            HighlightLength = max+4;

            Display(); // This was taken out of the BaseMenu class as we need to display after we work out the highlight length.
        }

        private void PrintHighlightedOption(string Option)
        {
            Console.Write("\t"); // We're writing the tab outside of the main print for stlystic reasons; we want the highlight to have a left margin a little bit.
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine($"  {Option}".PadRight(HighlightLength)); // Print the option with highlighted (white) background, using this method.
            // PadRight provides some common trailing whitespace. Interestingly enough it creates a common margin, no matter the length of the preceding string.                                                         
            Console.ResetColor();
        }

        protected override void Display()
        {
            Console.WriteLine(Prompt);
            for (int i = 0; i < Options.Length; i++)
            {
                if (i == 0) // First option
                {
                    PrintHighlightedOption(Options[i]);
                }
                else { Console.WriteLine($"\t  {Options[i]}"); }
                // Otherwise just write the option normally. No trailing whitespace is needed as there's no highlight.
            }

            

        }

        public Tuple<int, string> GetSelection() // This method should be public as we need to return values back up to the code where we're instancing the menu.
        {
            var input = Console.ReadKey();
            while (input.Key != ConsoleKey.Enter)
            {
                // Normal input handlers
                if (input.Key == ConsoleKey.UpArrow && SelectedOption > 0)
                    { SelectedOption--; }
                else if (input.Key == ConsoleKey.DownArrow && SelectedOption < Options.Length-1)
                    { SelectedOption++; }

                // Handlers for input in the event that the user is at the top or bottom, in which case we must roll over.
                else if (input.Key == ConsoleKey.UpArrow && SelectedOption == 0)
                    { SelectedOption = Options.Length - 1; }
                else if (input.Key == ConsoleKey.DownArrow && SelectedOption == Options.Length-1)
                    { SelectedOption = 0; }

                input = Console.ReadKey(); // Get a new key and repeat the loop
                
            }

            if (ClearAfterSelection) { Console.Clear(); }
            return Tuple.Create(SelectedOption, Options[SelectedOption]); // Return the string and the option number to minimize work done after result has been returned.
        }
    }
}
