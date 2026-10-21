using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using BinanceBot.Common;
using CryptoCoreNew;
using CryptoExchange.Net.Logging;

namespace BinanceBotNew
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

            var limitBuyAtPercentage = 0.55m;
            var numBitcoinInAccount = 0.0015m;
            var monitorUmbralPercentage = 0.01m;
            var useUmbral = false;
            var monitorUmbralTotalPercentage = 0.40m;
            var monitorMillisecondsInterval = 500;
            var monitorStartMinute = 59;
            var monitorStartSecond = 54;
           // var channelName = "Big Pump Signal"; // REVISAR
            var channelName = "[B] ForexTraders"; // REVISAR

            _handler += new EventHandler(Handler);
            SetConsoleCtrlHandler(_handler, true);

            Console.WriteLine("A: Automatico Telegram");
            Console.WriteLine("M: Manual metes la coin");
            var res = Console.ReadKey();
            Console.Clear();

           IExchange exchange;
           exchange = new BinanceExchange(apiKey,apiSecret);
           Console.WriteLine($"Life will never be the same!!");
           Console.WriteLine($"TIME:{DateTime.Now}");

            List<Tuple<decimal, decimal>> exitStrategy = new List<Tuple<decimal, decimal>>()
            {
                new Tuple<decimal, decimal>(0.33m,2.97m),
                new Tuple<decimal, decimal>(0.33m,2.92m),
                new Tuple<decimal, decimal>(0.32m,3.2m)
            };

            var tickerRules = await exchange.GetTickerRules();
            bi = new BinanceBot.Common.Binance(tickerRules, numBitcoinInAccount, monitorStartMinute, monitorStartSecond, exchange, apiKey, apiSecret);
            await bi.UpdateLowVolumeTickers(1000);
            string ticker = "";
            var priceMarketData = await exchange.RefreshPriceMarketData();
            if (res.Key == ConsoleKey.A)
            {
                TelegramReader reader = new TelegramReader();
                ticker = await reader.ReadTicker(channelName);
            }

            if (res.Key == ConsoleKey.M)
            {
                ticker = bi.GetBtcTickerAsync().Result;
            }

            Console.WriteLine($"Ticker found {ticker}");
            return;
            var success = await bi.CreateLimitOrderLikMarketPriceAsync(ticker, exchange.GetPriceMarketData().Data[ticker],
                    limitBuyAtPercentage);

                if (success)
                {
                    await bi.CreateSellOrdersAsync(exitStrategy, ticker);
                }

                bi.SubscribeToBookTickerUpdates(ticker);
                bi.PrintOrders();
                var key = Console.ReadKey();

                while (key.Key != ConsoleKey.Escape)
                {
                    if (key.Key == ConsoleKey.Q)
                    {
                        var orders = await bi.GetOpenOrders(ticker);
                        await bi.CancelAllOrders(ticker);
                        decimal increment=0;
                        foreach (var orderDto in orders)
                        {
                            await bi.CreateSellOrderAsync(orderDto.Quantity, bi.GetLastPrice(), ticker, 1.2m+increment);
                            increment += 0.05m;
                        }
                    }
                    if (key.Key == ConsoleKey.A)
                    {
                        var orders = await bi.GetOpenOrders(ticker);
                        await bi.CancelAllOrders(ticker);
                        decimal increment = 0;
                        foreach (var orderDto in orders)
                        {
                            await bi.CreateSellOrderAsync(orderDto.Quantity, bi.GetLastPrice(), ticker, 0.95m - increment);
                            increment += 0.05m;
                        }
                    }
                    key = Console.ReadKey();
                }
            
            bi.PrintLogs();
        }
    }
}
