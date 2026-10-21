using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using CryptoCoreNew;

namespace DataMonitorOnly
{
    class Program
    {
        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(EventHandler handler, bool add);

        private delegate bool EventHandler(CtrlType sig);
        static EventHandler _handler;
        static BinanceBot.Common.Binance bi;

        enum CtrlType
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT = 1,
            CTRL_CLOSE_EVENT = 2,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT = 6
        }

        private static bool Handler(CtrlType sig)
        {
            bi.PrintLogs();
            return true;
        }

        static async Task Main(string[] args)
        {
            string apiKey = "REPLACE_WITH_YOUR_VALUE";
            string apiSecret = "REPLACE_WITH_YOUR_VALUE";

            _handler += new EventHandler(Handler);
            SetConsoleCtrlHandler(_handler, true);

            Console.WriteLine("Buying Mode? Y/N");
            var res = Console.ReadKey();
            Console.Clear();

            BinanceExchange exchange;
            exchange = new BinanceExchange(apiKey, apiSecret);

            List<Tuple<decimal, decimal>> exitStrategy = new List<Tuple<decimal, decimal>>()
            {
                new Tuple<decimal, decimal>(0.33m,0.45m),
                new Tuple<decimal, decimal>(0.33m,0.47m),
                new Tuple<decimal, decimal>(0.32m,0.51m),
            };
            var limitBuyAtPercentage = 0.22m;
            var numBitcoinInAccount = 0.1m;
            var monitorUmbralPercentage = 0.01m;
            var monitorUmbralTotalPercentage = 0.062m;
            var monitorMillisecondsInterval = 500;
            var monitorStartMinute = 59;
            var monitorStartSecond = 59;

            ///* TODO INVESTIGATE sell orders on the market to try to find patterns on pump structures
            //   TODO Create fake pumps just before pump to try to get people to buy them */
            ///* TODO Option to trade also on ETH If I have enough funds in both check the one with lower %increase to buy*/
            ///* TODO create 2 console APP one with Monitoring the other one without and quick tiping the fastest one wins
            //   if I only have enough credit for one operation the fastes one will win */

            var tickerRules = await exchange.GetTickerRules();

            if (res.Key == ConsoleKey.N)
            {
                Console.WriteLine("MONITORING ONLY>");
                bi = new BinanceBot.Common.Binance(tickerRules, numBitcoinInAccount, monitorStartMinute, monitorStartSecond, exchange,apiKey,apiSecret);
                await bi.UpdateLowVolumeTickers();
                string ticker = await bi.MonitorPriceAndReturnTicket(monitorUmbralPercentage, monitorMillisecondsInterval,
                    monitorUmbralTotalPercentage);
                bi.MonitorVolume(monitorUmbralPercentage, monitorMillisecondsInterval, monitorUmbralTotalPercentage);
                bi.PrintLogs();
            }
            else
            {
                //Console.WriteLine("BUYING MODE>");
                //bi = new BinanceBot.Common.Binance(tickerRules, numBitcoinInAccount, monitorStartMinute, monitorStartSecond, exchange, apiKey, apiSecret);
                //bi.UpdateLowVolumeTickers();

                //string ticker = bi.MonitorPriceAndReturnTicket(monitorUmbralPercentage, monitorMillisecondsInterval,
                //    monitorUmbralTotalPercentage);

                //if (!string.IsNullOrEmpty(ticker))
                //{
                //    var order = bi
                //        .CreateOrderAtMarketPriceAsync(ticker, bi.initialPriceData.Data[ticker], limitBuyAtPercentage)
                //        .Result;
                //    if (order)
                //    {
                //        bi.CreateSellOrdersAsync(exitStrategy, ticker).Wait();
                //    }
                //}

                //bi.MonitorVolume(monitorUmbralPercentage, monitorMillisecondsInterval, monitorUmbralTotalPercentage);
                //bi.PrintLogs();
                //bi.PrintPriceLogs();
            }
        }
    }
}
