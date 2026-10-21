using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Binance.Net.Enums;
using Binance.Net.Interfaces;
using CryptoCoreNew.Model;
using CryptoCoreNew;

namespace VolumeMonitorNew
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

            BinanceExchange exchange;
            exchange = new BinanceExchange(apiKey, apiSecret);

            List<Tuple<decimal, decimal>> exitStrategy = new List<Tuple<decimal, decimal>>()
            {
                new Tuple<decimal, decimal>(0.25m,0.92m),
                new Tuple<decimal, decimal>(0.25m,1.1m),
                new Tuple<decimal, decimal>(0.25m,1.8m),
                new Tuple<decimal, decimal>(0.24m,2.1m),
            };
            var limitBuyAtPercentage = 0.35m;
            var numBitcoinInAccount = 0.0015m;
            var monitorUmbralPercentage = 0.01m;
            var monitorUmbralTotalPercentage = 0.08m;
            var monitorMillisecondsInterval = 500;
            var monitorStartMinute = 59;
            var monitorStartSecond = 58;

            Console.WriteLine("Life will never be the same.");
            var tickerRules = await exchange.GetTickerRules();
            bi = new BinanceBot.Common.Binance(tickerRules, numBitcoinInAccount, monitorStartMinute, monitorStartSecond, exchange, apiKey,apiSecret);

            await bi.UpdateLowVolumeTickers();

            string ticker = await bi.MonitorPriceAndReturnTicket(monitorUmbralPercentage, monitorMillisecondsInterval,
                    monitorUmbralTotalPercentage);

            if (!string.IsNullOrEmpty(ticker))
            {
                var order = await bi.CreateOrderAtMarketPriceAsync(ticker, bi.initialPriceData.Data[ticker], limitBuyAtPercentage);
                if (order)
                {
                    await bi.CreateSellOrdersAsync(exitStrategy, ticker);
                }
            }

            Console.Clear();
            Console.WriteLine($"estamos dentro con {ticker}");
            bi.PrintOrders();
                var key = Console.ReadKey();

                while (key.Key != ConsoleKey.Escape)
                {
                    if (key.Key == ConsoleKey.Q)
                    {
                        var orders = await bi.GetOpenOrders(ticker);
                        await bi.CancelAllOrders(ticker);
                        decimal increment = 0;
                        foreach (var orderDto in orders)
                        {
                            await bi.CreateSellOrderAsync(orderDto.Quantity, bi.GetLastPrice(), ticker, 1.2m + increment);
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

        }
    }
}
