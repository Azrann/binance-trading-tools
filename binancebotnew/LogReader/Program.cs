using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LogReader
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Ticker:");
            var ticker = Console.ReadLine();
            Console.Write("Filename:");
            var filename = Console.ReadLine();
            Console.Write("Starting Price:");
            var price = Decimal.Parse(Console.ReadLine());

            string allText = File.ReadAllText(filename);

            bool finish = false;
            int startingTime = 0;
            int startingTicker = 0;
            while (!finish)
            {
                startingTime = allText.IndexOf(">", startingTime) -14;
                if (startingTicker == -1)
                {
                    return;
                }
                string Time = allText.Substring(startingTime, 12);
                startingTicker = allText.IndexOf(ticker, startingTicker) + ticker.Length +2;
                string priceString = allText.Substring(startingTicker, 10);
                startingTime += 25;
                startingTicker += 25;
                var percentage = (Decimal.Parse(priceString) - price) / price;
                LogWrite($"{Time}:{priceString}   {(percentage*100):#.00}%",ticker);
            }
        }

        private static void LogWrite(string logMessage, string ticker)
        {
            Console.WriteLine(logMessage);
            var execPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Directory.CreateDirectory($"{execPath}\\logs");
            try
            {
                using (StreamWriter w = File.AppendText($"{execPath}\\logs\\{DateTime.Now.ToShortDateString().Replace("/", "-")}-log-{ticker}.txt"))
                {
                    w.WriteLine(logMessage);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(DateTime.Now.ToShortTimeString());
            }
        }
    }
}
