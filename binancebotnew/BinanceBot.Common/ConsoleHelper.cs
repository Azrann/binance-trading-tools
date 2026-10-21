using System;

namespace BinanceBot.Common
{
    public class ConsoleHelper
    {
        public static void ConsolePrintGlobalPercentage(string symbolPrice, decimal percentageChange)
        {
            if (percentageChange > 0.4m)
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
            } else if (percentageChange > 0.3m)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
            }
            else if (percentageChange > 0.2m)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
            }

            Console.WriteLine($"{symbolPrice} price up Global % {(percentageChange * 100):#.00}%");
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        public static void ConsolePrintPercentage(string symbolPrice, decimal percentageChange)
        {
            Console.WriteLine(
                $"{symbolPrice} price up {(percentageChange * 100):#.00}%");
        }
    }
}