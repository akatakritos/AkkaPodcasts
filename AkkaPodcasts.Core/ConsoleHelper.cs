using System;
using System.Collections.Generic;
using System.Linq;

namespace AkkaPodcasts.Core
{
    public static class ConsoleHelper
    {
        public static void Write(ConsoleColor color, string message)
        {
            var current = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ForegroundColor = current;
        }
    }
}
