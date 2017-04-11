using ConsoleShortcuts;
using System;

namespace ModularTerminal
{
    class ModularTerminal
    {
        static void Main(string[] args)
        {
            var first = new Input<string>("I'll let you enter the first item of the menu: ").Receive(); // Get the user's string input to use later
            
            var M = new HighlightMenu("The choice is yours:", first, "Progress", "RelativeLength 333", "RelativeLength 4444"); // Show a highlight menu
            M.Display();
            var Result = M.GetSelection(); // and get the user's choice from it

            Console.CursorTop = M.LastOccupiedLine + 2;
            Console.WriteLine($"You have selected option {Result.Index + 1}: {Result.Text}."); // First one: option number (humanized i.e. non zero-based); second: text

            new Exit(); // http://i.imgur.com/klYQmwL.jpg the text message sent by the Caddy, a corrupt officer working in anti-corruption, before the armed
                        // police officer guarding his room shot his fellow guard and brought him on a shooting spree around London.
                        // His crew was later shot by a high caliber sniper rifle - wielded by none other than the partner of the officer he implicated
                        // as being the Caddy in his place.
                        // rest in peace
                        // the TV show is Line of Duty Series 3 by the way
        }
    }
}
