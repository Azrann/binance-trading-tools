using System;
using System.Collections.Generic;
using System.Globalization;

namespace BinanceBot.Common
{
    public class ConsoleSafe
    {
        private static string entry = "test";

        public static void WriteLine(string message, int? pos1 = null, int? pos2 = null)
        {
            if (pos1.HasValue && pos2.HasValue)
            {
                lock (entry)
                {
                    Console.SetCursorPosition(pos1.Value,pos2.Value);
                    Console.WriteLine(message);
                }
            }
            else
            {
                Console.WriteLine(message);
            }
        }

        public static void WriteLine(string message, List<string> log, int? pos1 = null, int? pos2 = null)
        {
            if (pos1.HasValue && pos2.HasValue)
            {
                lock (entry)
                {
                    Console.SetCursorPosition(pos1.Value, pos2.Value);
                    Console.WriteLine(message);
                }
            }
            else
            {
                Console.WriteLine(message);
            }

            log?.Add($"{GetTimeNowAsString()},{message}");
        }

        public static string GetTimeNowAsString()
        {
            return DateTime.Now.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture);
        }

    }
}
