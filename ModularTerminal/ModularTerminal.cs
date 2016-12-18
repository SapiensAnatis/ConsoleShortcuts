using ConsoleShortcuts;
using System;

namespace ModularTerminal
{
    class ModularTerminal
    {
        static void Main(string[] args)
        {
            var first = new Input<string>("I'll let you enter the first item of the menu: ").Receive();
            
            var M = new HighlightMenu("Enter a thing", first, "Progress", "RelativeLength 333", "RelativeLength 4444") { ShowNumbers = false };
            Tuple<int, string> result = M.GetSelection();

            Console.WriteLine($"You have selected option {result.Item1+1}: {result.Item2}.\n");

            if (result.Item1 == 1)
            {
                var P = new ProgressBar(0);
                for (int i = 0; i <= 100; i++)
                {
                    P.Progress = i;
                    System.Threading.Thread.Sleep(100);
                }
            }

            new Exit();
        }
    }
}
