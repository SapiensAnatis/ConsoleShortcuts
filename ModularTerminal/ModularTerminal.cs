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
                
                ProgressBar[] wow = new ProgressBar[10];
                for (int x = 0; x <= 9; x++)
                {
                    wow[x] = new ProgressBar(10-x, x + 2);
                }

                for (int i = 0; i <= 100; i++)
                {
                    foreach(ProgressBar p in wow)
                    {
                        /*if (i % (p.Line) == 0)
                            p.Progress = i;*/
                        p.Progress++;
                    }
                    System.Threading.Thread.Sleep(100);
                }

            }
            Console.CursorTop++;
            new Exit();
        }
    }
}
