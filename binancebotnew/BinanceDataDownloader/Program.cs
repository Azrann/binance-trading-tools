using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Binance.Net.Enums;
using Binance.Net.Interfaces;
using Binance.Net.Objects.Spot.MarketData;
using BinanceBot.Common;
using CryptoCoreNew;
using Newtonsoft.Json;

namespace BinanceDataDownloader
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
                new Tuple<decimal, decimal>(0.33m,0.45m)
            };
            var limitBuyAtPercentage = 0.22m;
            var numBitcoinInAccount = 0.1m;
            var monitorUmbralPercentage = 0.01m;
            var monitorUmbralTotalPercentage = 0.062m;
            var monitorMillisecondsInterval = 500;
            var monitorStartMinute = 59;
            var monitorStartSecond = 59;
            
            var tickerRules = await exchange.GetTickerRules();
            bi = new BinanceBot.Common.Binance(tickerRules, numBitcoinInAccount, monitorStartMinute, monitorStartSecond, exchange,apiKey,apiSecret);
            await bi.UpdateLowVolumeTickers();

            Dictionary<DateTime, IBinanceKline> data = new Dictionary<DateTime, IBinanceKline>();
            foreach (var ticker in LowVolumeTickers._tickers)
            {
                //Dictionary<DateTime, IBinanceKline> response = await bi.GetKilines(ticker, DateTime.Now.AddDays(-3), DateTime.Now,KlineInterval.FifteenMinutes);
                //decimal average = response.Values.Select(x => x.QuoteVolume).Average();
                //var result = response.Where(x => (x.Value.QuoteVolume / average > 10m) && BetweenLimits(x.Key)).ToDictionary(x => x.Key, x => x.Value); ;
                //if (result.Count() > 1)
                //{
                //    bi.PrintCsvLines(result, $"{ticker}StrangeMovements.csv");
                //}
            }

            //var result2 = await exchange._api.Spot.Market.GetHistoricalSymbolTradesAsync("EVXBTC", 1000);
            //List<string> results3 = new List<string>();
            //results3.Add($"TradeTime,Price,QuoteQuantity,BuyerIsMaker");
            //foreach (var trade in result2.Data)
            //{
            //    results3.Add($"{trade.TradeTime},{trade.Price},{trade.QuoteQuantity},{trade.BuyerIsMaker}");
            //}

            //PrintCsvLines(results3,"EVXBTCGetLastTrades");

            //return;


            // Get ALL TICKETS BY VOLUME
            //var btcList1= priceData.Where(y => y.Symbol.EndsWith("BTC") && y.Volume > 80).OrderBy(x => x.Volume).Take(84).ToList();
            //var execPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            //using (StreamWriter w = File.AppendText($"{execPath}\\logs\\{DateTime.Now.ToShortDateString().Replace("/", "-")}-Volume.csv"))
            //{
            //    foreach (var symbolVolume in btcList1)
            //    {
            //        w.WriteLine($"{symbolVolume.Symbol},{symbolVolume.Volume.ToString("0.#")}");
            //    }
            //}


            // LowVolumeTickers._tickers = LowVolumeTickers._tickers.Union(ethList).ToList();

            Dictionary<string,BinanceOrderBook> orderBooks =new Dictionary<string, BinanceOrderBook>();
            int numTickets = LowVolumeTickers._tickers.Count;
            int i = 1;
            foreach (var ticker in LowVolumeTickers._tickers)
            {
                ConsoleSafe.WriteLine($"Procesando {i}/{numTickets} {ticker}");
                var orderBook = await bi.GetOrderBookInfo(ticker);
                orderBooks.Add(ticker,orderBook);
                Thread.Sleep(500);
                i++;
            }

            bi.WriteOrderBookInfo(orderBooks);
            bi.PrintLogs();
        }
        public static void PrintCsvLines(List<string> lines, string fileName)
        {
            var execPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Directory.CreateDirectory($"{execPath}\\logs");
            try
            {
                using (StreamWriter w = File.AppendText($"{execPath}\\logs\\{fileName}-{DateTime.Now.Ticks}.csv"))
                {
                   foreach (var line in lines)
                    {
                        w.WriteLine(line);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(DateTime.Now.ToShortTimeString() + JsonConvert.SerializeObject(ex));
            }
        }

        private static bool BetweenLimits(DateTime date)
        {
            if((date > new DateTime(2021,2,4,22,0,0)) && (date < new DateTime(2021, 2, 5, 1, 0, 0)))
            {
                return true;
            }
            if ((date > new DateTime(2021, 2, 5, 13, 0, 0)) && (date < DateTime.Now))
            { 
                return true;
            }
            return false;
        }
    }
}
