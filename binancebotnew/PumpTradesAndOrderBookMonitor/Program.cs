using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using CryptoCoreNew;

namespace PumpTradesAndOrderBookMonitor
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

            Console.WriteLine("* Life will never be the same ** Pump Trade and OrderBook Monitor");
            BinanceExchange exchange;
            exchange = new BinanceExchange(apiKey, apiSecret);

            var monitorUmbralPercentage = await GetBinance(exchange, apiKey, apiSecret);
            await bi.UpdateLowVolumeTickers();
            string ticker = await bi.MonitorPriceAndReturnTicket(monitorUmbralPercentage, 500,0.06m);
            await bi.MonitorPump(ticker);
            bi.PrintLogs();
        }

        private static async Task<decimal> GetBinance(BinanceExchange exchange, string apiKey, string apiSecret)
        {
            List<Tuple<decimal, decimal>> exitStrategy = new List<Tuple<decimal, decimal>>()
            {
                new Tuple<decimal, decimal>(0.33m, 0.45m),
                new Tuple<decimal, decimal>(0.33m, 0.47m),
                new Tuple<decimal, decimal>(0.32m, 0.51m),
            };
            var limitBuyAtPercentage = 0.22m;
            var monitorUmbralTotalPercentage = 0.062m;
            var monitorMillisecondsInterval = 500;

            var numBitcoinInAccount = 0.1m;
            var monitorUmbralPercentage = 0.01m;
            var monitorStartMinute = 59;
            var monitorStartSecond = 56;

            var tickerRules = await exchange.GetTickerRules();

            bi = new BinanceBot.Common.Binance(tickerRules, numBitcoinInAccount, monitorStartMinute, monitorStartSecond,
                exchange, apiKey, apiSecret);
            return monitorUmbralPercentage;
        }
    }
}
