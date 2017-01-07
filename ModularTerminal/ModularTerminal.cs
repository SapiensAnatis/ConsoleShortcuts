using ConsoleShortcuts;
using System;

namespace ModularTerminal
{
    class ModularTerminal
    {
        static void Main(string[] args)
        {
            var first = new Input<string>("I'll let you enter the first item of the menu: ").Receive(); // Get the user's string input to use later
            
            var M = new HighlightMenu("Enter a thing", first, "Progress", "RelativeLength 333", "RelativeLength 4444") { ShowNumbers = true }; // Show a highlight menu
            Tuple<int, string> result = M.GetSelection(); // and get the user's choice from it

            Console.WriteLine($"You have selected option {result.Item1+1}: {result.Item2}.\n"); // First one: option number; second: text

            if (result.Item1 == 1) // If second option (zero-indexed) which is Progress
            {
                // Make 10 progress bars and make them all a bit slower than each other to make some sort of ripple thing
                ProgressBar[] wow = new ProgressBar[10];
                for (int x = 0; x <= 9; x++)
                {
                    wow[x] = new ProgressBar(3*(10-x), x + 2); // Not all progress bars are made equal
                }

                for (int i = 0; i <= 100; i++)
                {
                    foreach(ProgressBar p in wow)
                        p.Progress++;
                    
                    System.Threading.Thread.Sleep(20);
                }

                Console.Write("\n\nNice and fluent/smooth, right? ;)");

            }
            Console.CursorTop++;
            new Exit();
        }
    }
}
