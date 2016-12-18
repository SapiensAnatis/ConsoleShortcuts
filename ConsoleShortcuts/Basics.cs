﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleShortcuts
{
    public class Exit
    {
        private ConsoleKey exitKey { get; set; } = ConsoleKey.Enter; // The key, defaults to enter. Not included in constructor, can be specified in a collection initializer if so desired.

        public Exit(string text)
        {
            Console.WriteLine(text=$"Press {exitKey.ToString()} to exit..."); // Define a default standard text which should work for most cases.
            ActuallyExit(); // Once called, exit. Generally an instance is not created unless an exit is immediately required.
        }

        private void ActuallyExit()
        {
            while (Console.ReadKey().Key != exitKey) {  } // While they haven't pressed the exit key, do nothing else.
            System.Environment.Exit(0); // Provide an exit (works only for consoles, which is fine given the name of this project)
        }
    }

    public class Input<T> // Multi-type Input function. Will only work for types which can be converted to from a string.
    {
        public Input(string p, bool nl = false)
        {
            Console.Write($"{p} ");
            if (nl) { Console.Write("\n"); }
            // We do not call Receive() here as it returns types that need to be available to the scope in which this class was instanced.
            // as a result, it will have to be called there.
        }

        public T Receive()
        {
            string tmpResult = Console.ReadLine(); // Actually get input
            return (T)Convert.ChangeType(tmpResult, typeof(T));
        }
    }
}
