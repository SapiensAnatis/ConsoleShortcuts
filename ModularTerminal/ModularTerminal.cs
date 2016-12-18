using ConsoleShortcuts;
using System;

namespace ModularTerminal
{
    class ModularTerminal
    {
        static void Main(string[] args)
        {
            var M = new HighlightMenu("Enter a thing", "RelativeLength 1", "RelativeLength 22", "RelativeLength 333", "RelativeLength 4444") { ShowNumbers = false };
            Tuple<int, string> result = M.GetSelection();

            Console.WriteLine($"You have selected option {result.Item1+1}: {result.Item2}.\n");

            new Exit("Press Enter to exit...");
        }
    }
}
