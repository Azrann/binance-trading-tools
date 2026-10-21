using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Binance.API.Csharp.Client;
using Binance.API.Csharp.Client.Models.Account;
using Binance.API.Csharp.Client.Models.Enums;
using Newtonsoft.Json;

namespace BinanceBot
{
    public class Binance
    {
        private static ApiClient apiClient = new ApiClient("REPLACE_WITH_YOUR_VALUE", "REPLACE_WITH_YOUR_VALUE");
        public static BinanceClient binanceClient = new BinanceClient(apiClient, false);

        public Stopwatch stop;

        public PriceResult FirstPriceResultUnsafe;
        public decimal _firstSellOrderPrice;
        public NewOrder firstNewOrder;
        public const decimal numBitcoin = 0.35m;
        public List<string> memoryLog;

        private string _ticker;
        private decimal _maxprice;
        private decimal _targetprice;
        public PriceResult bidding;

        private TickerRules _tickerRules;
        private readonly HttpClientHelper _httpClientHelper;

        public Binance(TickerRules tickerRules, HttpClientHelper httpClientHelper)
        {
            _tickerRules = tickerRules;
            _httpClientHelper = httpClientHelper;
            stop= new Stopwatch();
            memoryLog = new List<string>();
        }

        public async Task<NewOrder> BotBuyAsync()
        {
            int counter = 0;
            do
            {
                Console.Clear();
                Console.WriteLine($"-{numBitcoin}-");
                Console.Write($"{counter}-ticker:");
                var price = GetPriceUnsafe("ETHBTC");
                await Binance.binanceClient
                    .PostNewOrder("ETHBTC",0.1m, 0.02m, OrderSide.BUY, OrderType.LIMIT).ConfigureAwait(false);
                _ticker = (Console.ReadLine() + "btc").ToUpper();
                counter++;
            } while (_ticker == "BTC");

            Log($"Ticker added {_ticker}");
            
            var task2 = CreateOrderAtMarketPriceAsync(_ticker);
            var task1 = SetMaxPrice();
            await Task.WhenAll(task1, task2);

            firstNewOrder = task2.Result;
            return firstNewOrder;
        }

        public async Task<NewOrder> BotPlaceSellOrder()
        {
            // Se el precio de compra lo multiplico por 2 y escribo predefinidos los digitos que coinciden
            // Ej compra a 0.002458 el doble es 0.004916 como coincide 0.00 pues eso que meto predefinido.
            
            //string firstNumbers = GetMatchingNumbers(FirstPriceResultUnsafe.Price);

            return await CreateSellOrderAsync();
        }

        public Task<decimal> SetMaxPrice()
        {
            Console.Write($"maxprice:");
            _maxprice = Math.Round(Decimal.Parse($"{Console.ReadLine().Trim()}"), _tickerRules.Rules[_ticker].MinPriceDecimals);

            Log($"Maxprice added {_maxprice}");
            return Task.FromResult(_maxprice);
        }

        public async Task<NewOrder> CreateOrderAtMarketPriceAsync(string pair)
        {
            var stopwatch = Stopwatch.StartNew();

            bidding =  GetPriceUnsafe(pair);

            stopwatch.Stop();
            Log($"Milliseconds to get the price {stopwatch.ElapsedMilliseconds}");

            stopwatch.Reset();
            stopwatch.Start();

            var result = await Binance.binanceClient
                .PostNewOrder(pair, bidding.Quantity, 0m, OrderSide.BUY, OrderType.MARKET).ConfigureAwait(false);

            stopwatch.Stop();
            Log("Buy Order Created");
            Log($"Milliseconds to create buy order {stopwatch.ElapsedMilliseconds}");

            return result;
        }

        private string GetTimeNowAsString()
        {
            return DateTime.Now.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture);
        }

        private void Log(string logmessage)
        {
            memoryLog.Add($"{GetTimeNowAsString()} -> {logmessage}");
        }

        public PriceResult GetPriceUnsafe(string pair)
        {
            decimal firstSellPrice = _httpClientHelper.GetTickerPrice(pair);
            FirstPriceResultUnsafe = new PriceResult();

            var howManyCanIBuy = Math.Round(numBitcoin / firstSellPrice, _tickerRules.Rules[pair].MinAmountDecimals);

            FirstPriceResultUnsafe.Price = Math.Round(firstSellPrice, _tickerRules.Rules[pair].MinPriceDecimals);
            FirstPriceResultUnsafe.Quantity = howManyCanIBuy;
            return FirstPriceResultUnsafe;
        }

        public string GetMatchingNumbers(decimal price)
        {
            decimal matching = price * 2;
            var numdigits = 0;
            foreach (var number in price.ToString(CultureInfo.InvariantCulture))
            {
                if (price.ToString(CultureInfo.InvariantCulture)[numdigits] == matching.ToString(CultureInfo.InvariantCulture)[numdigits])
                {
                    numdigits++;
                }
                else
                {
                    return price.ToString(CultureInfo.InvariantCulture).Substring(0, numdigits);
                }
            }

            return price.ToString(CultureInfo.InvariantCulture).Substring(0, numdigits);
        }

        public async Task<NewOrder> CreateSellOrderAsync()
        {
            var stopwatch = Stopwatch.StartNew();

            _firstSellOrderPrice = _maxprice - ((_maxprice - FirstPriceResultUnsafe.Price) * 0.05m);
            _firstSellOrderPrice = Math.Round(_firstSellOrderPrice, _tickerRules.Rules[_ticker].MinPriceDecimals);
            NewOrder result;
            try
            {
                result = await Binance.binanceClient
                    .PostNewOrder(_ticker, bidding.Quantity, _firstSellOrderPrice, OrderSide.SELL,
                        OrderType.LIMIT).ConfigureAwait(false);
                stopwatch.Stop();
                Log($"Milliseconds to create sell order {stopwatch.ElapsedMilliseconds}");
                Log("Sell order created");
            }
            catch (Exception e)
            {
                Log("Sell order failed creating in the first time");
                result = await Binance.binanceClient
                    .PostNewOrder(_ticker, bidding.Quantity - GetSmallestQuantity(), _firstSellOrderPrice, OrderSide.SELL,
                        OrderType.LIMIT).ConfigureAwait(false);
                Log($"Milliseconds to create sell order second attempt {stopwatch.ElapsedMilliseconds}");
                Log("Sell order created second attempt");
            }
            return result;
        }

        private decimal GetSmallestQuantity()
        {
            var result = 1m;
            int i = 0;
            int numDecimals = _tickerRules.Rules[_ticker].MinAmountDecimals;
            while (i < numDecimals)
            {
                result = result / 10;
                i++;
            }
            return result;
        }

        public decimal GetMaxPriceInLast5Minutes(string ticker)
        {
            var result = 0m;
            var candlestick = binanceClient.GetCandleSticks(ticker, TimeInterval.Minutes_1, DateTime.Now.AddMinutes(-5), DateTime.Now).Result;
            foreach (var candle in candlestick)
            {
                if (candle.High > result)
                {
                    result = candle.High;
                }
            }

            return result;
        }

        public async Task BotManagePosition(NewOrder sellOrder)
        {
            Order order = new Order();
            var checkStatus = 1000;
            var exit = false;
            do
            {
                while (!Console.KeyAvailable)
                {
                    checkStatus++;
                    var currentPrice = _httpClientHelper.GetTickerPrice(_ticker);
                    Log($"Current price: {currentPrice}");
                    if (checkStatus > 2)
                    {
                        checkStatus = 0;
                        order = Binance.binanceClient.GetOrder(_ticker, sellOrder.OrderId).Result;
                        Log($"Order status: {order.Status}");
                    }
                    Console.Clear();
                    Console.WriteLine($"TIME: {DateTime.Now.ToLongTimeString()}");
                    Console.WriteLine($"Target: {_firstSellOrderPrice:#.000000}");
                    Console.WriteLine($"Actual: {currentPrice}");
                    Console.WriteLine($"Maximu Gain: {((_maxprice - bidding.Price) / bidding.Price) * 100:#.0}%");
                    Console.WriteLine($"Actual Gain: {((currentPrice - bidding.Price) / bidding.Price) * 100:#.0}%");
                    Console.WriteLine();
                    Console.WriteLine($"Order: {order.OrderId} Quantity: {order.OrigQty} ExecutedQty : {order.ExecutedQty}");
                    Console.WriteLine($"STATUS: {order.Status}");

                    Thread.Sleep(200);
                }

                var key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.Escape)
                {
                    var result = Binance.binanceClient.CancelOrder(_ticker, sellOrder.OrderId).Result;
                    var order1 = Binance.binanceClient.PostNewOrder(_ticker, order.OrigQty - order.ExecutedQty, 0m, OrderSide.SELL, OrderType.MARKET).Result;

                    exit = true;
                }

                if (key == ConsoleKey.Enter)
                {
                    exit = true;
                }

            } while (!exit);
        }
        
        private static void LogWrite(string logMessage)
        {
            Console.WriteLine(logMessage);
            var execPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Directory.CreateDirectory($"{execPath}\\logs");
            try
            {
                using (StreamWriter w = File.AppendText($"{execPath}\\logs\\{DateTime.Now.ToShortDateString().Replace("/", "-")}-log.txt"))
                {
                    w.WriteLine(logMessage);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(DateTime.Now.ToShortTimeString() + JsonConvert.SerializeObject(ex));
            }
        }

        public void PrintLogs()
        {
            var execPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Directory.CreateDirectory($"{execPath}\\logs");
            try
            {
                using (StreamWriter w = File.AppendText($"{execPath}\\logs\\{DateTime.Now.ToShortDateString().Replace("/", "-")}-log.txt"))
                {
                    foreach (var logline in memoryLog)
                    {
                        w.WriteLine(logline);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(DateTime.Now.ToShortTimeString() + JsonConvert.SerializeObject(ex));
            }
        }
    }
}