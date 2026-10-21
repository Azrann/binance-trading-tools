using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Binance.Net;
using Binance.Net.Enums;
using Binance.Net.Interfaces;
using Binance.Net.Objects.Spot;
using Binance.Net.Objects.Spot.MarketData;
using CryptoCoreNew;
using CryptoCoreNew.Model;
using CryptoExchange.Net.Authentication;
using Newtonsoft.Json;

namespace BinanceBot.Common
{
    public class Binance
    {
        public PriceQuantityResult FirstPriceQuantityResultUnsafe;
        public decimal _firstSellOrderPrice;
        public decimal _numBitcoin;
        public Dictionary<string, List<string>> memoryLog;
        public Dictionary<string, OrderDto> _orders;
        
        private string _ticker;
        private decimal _maxprice;
        private decimal _targetprice;
        public PriceQuantityResult bidding;
        public PriceData initialPriceData;
        public PriceData currentData;
        
        private TickerRules _tickerRules;

        private readonly int _startingMinute;
        private readonly int _startingSecond;
        private readonly IExchange _exchange;
        private readonly string _apiKey;
        private readonly string _apiSecret;

		private ConsoleAutoComplete autoComplete;
        private List<string> _excludingTickets;
        private Queue<decimal> _lastPriceQueue;
        private Queue<decimal> _lastPriceAverageQueue;

        public const string PriceMessagesLog = "PriceMessagesLog";
        public const string PriceCsvLog = "PriceCsvLog";
        public const string PriceJsonLog = "PriceJsonLog";
        public const string BotEventsLog = "BotEventsLog";
        public const string StatsCsvLog = "StatsCsvLog";
        public const string OrderBookStatsCsvLog = "OrderBookStatsCsvLog";
        public Binance(TickerRules tickerRules, decimal numBitcoin, int monitorStartMinute, int monitorStartSecond, IExchange exchange, string apiKey, string apiSecret)
        {
            _tickerRules = tickerRules;
            _lastPriceQueue = new Queue<decimal>();
            _lastPriceAverageQueue = new Queue<decimal>();
            _orders = new Dictionary<string, OrderDto>();
            _numBitcoin = numBitcoin;
            LowVolumeTickers.Initialize();
            _startingMinute = monitorStartMinute;
            _startingSecond = monitorStartSecond;
            _exchange = exchange;
            _excludingTickets = new List<string>();
            _apiKey = apiKey;
            _apiSecret = apiSecret;

            memoryLog = new Dictionary<string, List<string>>()
            {
                {PriceMessagesLog,new List<string>()},
                {PriceCsvLog,new List<string>()},
                {PriceJsonLog,new List<string>()},
                {BotEventsLog,new List<string>()},
                {StatsCsvLog,new List<string>()},
                {OrderBookStatsCsvLog,new List<string>()}
            };
        }
        public async Task<string> GetBtcTickerAsync()
        {
            autoComplete = new ConsoleAutoComplete(LowVolumeTickers.Tickers.ToArray());

            int counter = 0;
            do
            {
                Console.Clear();
                Console.Write($"{counter}-ticker:");
                currentData = await _exchange.RefreshPriceMarketData();
                autoComplete.Reset();
                _ticker = (autoComplete.Start() + "btc").ToUpper();
                counter++;
            } while (_ticker == "BTC" || _ticker == "RRRBTC");

            Log(BotEventsLog,$"Ticker added {_ticker}");
            /* Removet this return null*/
            return _ticker;
        }
        
        public async Task UpdateLowVolumeTickers(int numTickers = 120,int minVolume = 40)
        {
            var priceData = await _exchange.GetMarketVolume();
            var btcList = priceData.Where(y => y.Symbol.EndsWith("BTC") && y.Volume > minVolume).OrderBy(x => x.Volume).Select(z => z.Symbol).Take(numTickers).ToList();
            LowVolumeTickers._tickers = LowVolumeTickers._tickers.Union(btcList).ToList();
        }

        public async Task<bool> CreateOrderAtMarketPriceAsync(string pair, decimal initialPrice,
            decimal limitBuyAtPercentage)
        {
            bidding = GetQuantityPriceResult(pair, initialPrice, limitBuyAtPercentage);

            BuyOrderResult order;
            try
            {
                var maxPrice = Math.Round((_numBitcoin / bidding.Quantity), _tickerRules.Rules[pair].MinPriceDecimals);
                var quantity = Math.Round(bidding.Quantity, _tickerRules.Rules[pair].MinAmountDecimals);
                order = await _exchange.CreateBuyMarketOrder(pair, bidding.Quantity).ConfigureAwait(false);

                Console.WriteLine($"Buy Order Created for {order.QuoteQuantityFilled} BTC and we bought {order.QuantityFilled} {pair} at {order.Price}");
                Log(BotEventsLog,$"Buy Order Created for {order.QuoteQuantityFilled} BTC and we bought {order.QuantityFilled} {pair} at {order.Price}");
                FirstPriceQuantityResultUnsafe.Quantity = order.QuantityFilled;
                FirstPriceQuantityResultUnsafe.Price = order.Price;
            }
            catch (Exception ex)
            {
                Log(BotEventsLog, $"Buy Order Exceed limit percentage of {limitBuyAtPercentage * 100}%");
                Console.WriteLine($"Buy Order Exceed limit percentage of {limitBuyAtPercentage * 100}%");
                Log(BotEventsLog, $"Exception {JsonConvert.SerializeObject(ex)}");
                return false;
            }

            return true;
        }

        public async Task<bool> CreateLimitOrderLikMarketPriceAsync(string pair, decimal initialPrice,
            decimal limitBuyAtPercentage)
        {
            bidding = GetQuantityPriceResult(pair, initialPrice, limitBuyAtPercentage);

            BuyOrderResult order;
            try
            {
                var maxPrice = Math.Round((_numBitcoin / bidding.Quantity), _tickerRules.Rules[pair].MinPriceDecimals);
                var quantity = Math.Round(bidding.Quantity, _tickerRules.Rules[pair].MinAmountDecimals);
                order = await _exchange.CreateBuyLimitOrder(pair, bidding.Quantity, maxPrice).ConfigureAwait(false);

                Console.WriteLine($"Buy Order Created for {order.QuoteQuantityFilled} BTC and we bought {order.QuantityFilled} {pair} at {order.Price}");
                Log(BotEventsLog, $"Buy Order Created for {order.QuoteQuantityFilled} BTC and we bought {order.QuantityFilled} {pair} at {order.Price}");
                FirstPriceQuantityResultUnsafe.Quantity = order.QuantityFilled;
                FirstPriceQuantityResultUnsafe.Price = order.Price;
            }
            catch (Exception ex)
            {
                Log(BotEventsLog, $"Buy Order Exceed limit percentage of {limitBuyAtPercentage * 100}%");
                Console.WriteLine($"Buy Order Exceed limit percentage of {limitBuyAtPercentage * 100}%");
                Log(BotEventsLog, $"Exception {JsonConvert.SerializeObject(ex)}");
                return false;
            }

            return true;
        }

        private string GetTimeNowAsString()
        {
            return DateTime.Now.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture);
        }

        private void Log(string logbucket, string logmessage)
        {
            memoryLog[logbucket].Add($"{GetTimeNowAsString()},{logmessage}");
        }

        public PriceQuantityResult GetQuantityPriceResult(string pair, decimal firstSellPrice, decimal limitBuyAtPercentage)
        {
            FirstPriceQuantityResultUnsafe = new PriceQuantityResult();

            var howManyCanIBuy = _numBitcoin / (firstSellPrice * (1+ limitBuyAtPercentage));
            var howManyCanIBuyRounded = Math.Round(howManyCanIBuy, _tickerRules.Rules[pair].MinAmountDecimals);

            FirstPriceQuantityResultUnsafe.Price = Math.Round(firstSellPrice, _tickerRules.Rules[pair].MinPriceDecimals);
            FirstPriceQuantityResultUnsafe.Quantity = howManyCanIBuyRounded;
            return FirstPriceQuantityResultUnsafe;
        }

        public async Task CreateSellOrdersAsync(List<Tuple<decimal, decimal>> exitStrategy, string ticker)
        {
            List<OrderDto> results = new List<OrderDto>();

            foreach (var tupple in exitStrategy)
            {
                var sellQuantity = Math.Round(FirstPriceQuantityResultUnsafe.Quantity * tupple.Item1,
                    _tickerRules.Rules[ticker].MinAmountDecimals);
                var result = await CreateSellWithQuantityAndTargetPercentageAsync(sellQuantity, tupple.Item2, ticker);
                if (result != null)
                {
                    results.Add(result);
                }
            }

            foreach (var orderDto in results)
            {
                UpdateOrder(orderDto);
            }
        }

        public async Task CreateSellOrderAsync(decimal quantity, decimal price, string ticker, decimal percentage)
        {
             var sellQuantity = Math.Round(quantity,_tickerRules.Rules[ticker].MinAmountDecimals);
             var orderPrice = price * (percentage);
             orderPrice = Math.Round(orderPrice, _tickerRules.Rules[ticker].MinPriceDecimals);
             var  result = await _exchange.CreateSellLimitOrder(ticker, sellQuantity, orderPrice).ConfigureAwait(false);
        }

        public async Task<OrderDto> CreateSellWithQuantityAndTargetPercentageAsync(decimal quantity, decimal percentage,
            string ticker)
        {
            var stopwatch = Stopwatch.StartNew();

            _firstSellOrderPrice = _exchange.GetPriceMarketData().Data[ticker] * (1 + percentage);
            _firstSellOrderPrice = Math.Round(_firstSellOrderPrice, _tickerRules.Rules[ticker].MinPriceDecimals);

            if (_firstSellOrderPrice < FirstPriceQuantityResultUnsafe.Price)
            {
                _firstSellOrderPrice = FirstPriceQuantityResultUnsafe.Price * (1.5m);
                _firstSellOrderPrice = Math.Round(_firstSellOrderPrice, _tickerRules.Rules[ticker].MinPriceDecimals);
            }

            OrderDto result = null;
            try
            {
                result = await _exchange.CreateSellLimitOrder(ticker, quantity, _firstSellOrderPrice)
                    .ConfigureAwait(false);
                stopwatch.Stop();

                Log(BotEventsLog,$"Milliseconds to create sell order {stopwatch.ElapsedMilliseconds}");
                Log(BotEventsLog, $"Sell order created quantity>{quantity} price>{_firstSellOrderPrice}");
            }
            catch (Exception ex)
            {
                Log(BotEventsLog, $"Sell order failed creating in the first time quantity>{quantity} price>{_firstSellOrderPrice}");
                Log(BotEventsLog, JsonConvert.SerializeObject(ex));
            }

            return result;
        }

        public async Task MonitorVolume(decimal umbral, int intervalMilliseconds, decimal umbralTotal)
        {
            currentData = new PriceData();
            initialPriceData = new PriceData();

            var exit = false;
            var monitorGlobalPercentage = false;
            do
            {
                while (!Console.KeyAvailable)
                {
                    monitorGlobalPercentage = CheckStartMonitoringForGlobalPercentage(monitorGlobalPercentage);
                    Log(PriceMessagesLog,$"Monitoring Global {monitorGlobalPercentage}");
                    Console.WriteLine($"{GetTimeNowAsString()} -> {monitorGlobalPercentage}");
                    
                    var priceData = await _exchange.RefreshPriceMarketData();
                    Log(PriceJsonLog,JsonConvert.SerializeObject(priceData));

                    foreach (var symbolPrice in priceData.Data)
                    {
                        if (symbolPrice.Key.EndsWith("BTC") && symbolPrice.Value > 0)
                        {
                            
                            if (currentData.Data.ContainsKey(symbolPrice.Key))
                            {
                                var oldprice = currentData.Data[symbolPrice.Key];
                                var percentageChange = (symbolPrice.Value - oldprice) / oldprice;
                                if (percentageChange > 0 && percentageChange > umbral)
                                {
                                    Log(PriceMessagesLog,$"{symbolPrice.Key} price up {(percentageChange * 100):#.00}% with price {symbolPrice.Value}");
                                    Log(PriceCsvLog, $"{symbolPrice.Key},\"{(percentageChange * 100):#,00}\", {symbolPrice.Value}, ComparePrevious");
                                    ConsoleHelper.ConsolePrintPercentage(symbolPrice.Key, percentageChange);
                                }

                            }
                            currentData.Data[symbolPrice.Key] = symbolPrice.Value;

                            if (monitorGlobalPercentage)
                            {
                                if (initialPriceData.Data.ContainsKey(symbolPrice.Key))
                                {
                                    var initialPrice = initialPriceData.Data[symbolPrice.Key];
                                    var percentageChange = (symbolPrice.Value - initialPrice) / initialPrice;
                                    if (percentageChange > 0 && percentageChange > umbralTotal)
                                    {
                                        Log(PriceMessagesLog,$"{symbolPrice.Key} price up Global % {(percentageChange * 100):#.00}% with price {symbolPrice.Value}");
                                        Log(PriceCsvLog, $"{symbolPrice.Key},\"{(percentageChange * 100):#,00}\", {symbolPrice.Value}, CompareTotal");
                                        ConsoleHelper.ConsolePrintGlobalPercentage(symbolPrice.Key, percentageChange);
                                    }
                                }
                                else
                                {
                                    initialPriceData.Data[symbolPrice.Key] = symbolPrice.Value;
                                }
                            }
                        }
                    }
                    Thread.Sleep(intervalMilliseconds - 500);
                }

                var key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.Escape)
                {
                    exit = true;
                }

                if (key == ConsoleKey.Enter)
                {
                    initialPriceData = new PriceData();
                    monitorGlobalPercentage = true;
                }
            } while (!exit);
               
        }

        public async Task MonitorPump(string ticker)
        {
            currentData = new PriceData();
            initialPriceData = new PriceData();
            Dictionary<long,IBinanceRecentTrade> trades = new Dictionary<long, IBinanceRecentTrade>();
            Dictionary<DateTime, BinanceOrderBook> orderBooks = new Dictionary<DateTime, BinanceOrderBook>();
            var exit = false;
            BinanceOrderBook currentOrderBook = null;
            do
            {
                while (!Console.KeyAvailable)
                {
                    var lastTrades = await _exchange.GetLastTrades(ticker);
                    if (currentOrderBook == null || (currentOrderBook.Asks.Count() < 1000 && currentOrderBook.Bids.Count() < 1000))
                    {
                        currentOrderBook = await _exchange.GetOrderBookAsync(ticker);
                    }
                    else
                    {
                        currentOrderBook = await _exchange.GetOrderBookAsync(ticker,5000);
                        Thread.Sleep(300);
                    }
                    if (orderBooks != null)
                    { 
                        orderBooks.Add(DateTime.Now, currentOrderBook);
                    }

                    foreach (var trade in lastTrades)
                    {
                        if (!trades.ContainsKey(trade.OrderId))
                        {
                            trades.Add(trade.OrderId,trade);
                        }
                    }
                    ConsoleSafe.WriteLine($"{trades.Count} trades.       ",null, 0,1);
                    ConsoleSafe.WriteLine($"{orderBooks.Count} orderBooks.       ", null, 0, 2);
                    Thread.Sleep(200);
                }

                var key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.Escape)
                {
                    foreach (var trade in trades.Values)
                    {
                        Log(StatsCsvLog, $"LastTrades,{trade.TradeTime}, {ticker}, \"{trade.QuoteQuantity.ToString("0.###").Replace(".",",")}\", {trade.Price},{trade.BuyerIsMaker},{trade.OrderId}");
                    }

                    var top95Count = Convert.ToInt32(trades.Values.Count() * 0.05);
                    var top95 = trades.Values.OrderByDescending(x => x.QuoteQuantity).Take(top95Count);

                    foreach (var trade in top95)
                    {
                        Log(StatsCsvLog, $"Top95QuantityTrades,{trade.TradeTime}, {ticker}, \"{trade.QuoteQuantity.ToString("0.###").Replace(".", ",")}\", {trade.Price},{trade.BuyerIsMaker},{trade.OrderId}");
                    }

                    foreach (var orderBook in orderBooks)
                    {
                        var oBook = orderBook.Value;
                        var oBookDate = orderBook.Key;
                        decimal SumBidVolum = oBook.Bids.Sum(x => x.Price * x.Quantity);
                        decimal SumAskVolum = oBook.Asks.Sum(x => x.Price * x.Quantity);
                        int bidCompraCount = oBook.Bids.Count();
                        int askVentaCount = oBook.Bids.Count();
                        decimal ventasVsCompras = SumAskVolum / SumBidVolum;
                        Log(OrderBookStatsCsvLog, $"OrderBookAgregated,{ticker},\"{bidCompraCount}\",\"{askVentaCount}\",\"{SumBidVolum.ToString("0.##")}\",\"{SumAskVolum.ToString("0.##")}\",\"{ventasVsCompras.ToString("0.##")}\"");

                        foreach (var bidPositions in oBook.Bids.Union(oBook.Asks))
                        {
                            Log(OrderBookStatsCsvLog, $"OrderBook,{orderBook.Key}, {ticker}, \"{(bidPositions.Quantity* bidPositions.Price).ToString("0.###").Replace(".", ",")}\",{bidPositions.Price}");
                        }

                        foreach (var askAgregate in OrderBookProcessor.Aggregates(oBook, DateTime.Now))
                        {
                            Log(OrderBookStatsCsvLog, $"OrderBookAskAgregated,{askAgregate.ToCsvString()}");
                        }
                    }

                    exit = true;
                }
            } while (!exit);
        }

        public async Task<BinanceOrderBook> GetOrderBookInfo(string ticker)
        {
            BinanceOrderBook currentOrderBook = await _exchange.GetOrderBookAsync(ticker);

            if (currentOrderBook.Asks.Count() < 1000 && currentOrderBook.Bids.Count() < 1000)
            {
                currentOrderBook = await _exchange.GetOrderBookAsync(ticker, 5000);
                Thread.Sleep(5000);
            }

            return currentOrderBook;
        }

        public void WriteOrderBookInfo(Dictionary<string, BinanceOrderBook> orderBooks)
        {
            foreach (var orderBook in orderBooks)
            {
                var oBook = orderBook.Value;
                var ticker = orderBook.Key;
                decimal SumBidVolum = oBook.Bids.Sum(x => x.Price * x.Quantity);
                decimal SumAskVolum = oBook.Asks.Sum(x => x.Price * x.Quantity);
                int bidCompraCount = oBook.Bids.Count();
                int askVentaCount = oBook.Bids.Count();
                decimal ventasVsCompras = SumAskVolum / SumBidVolum;
                Log(OrderBookStatsCsvLog, $"OrderBookAgregated,{ticker},\"{bidCompraCount}\",\"{askVentaCount}\",\"{SumBidVolum.ToString("0.##")}\",\"{SumAskVolum.ToString("0.##")}\",\"{ventasVsCompras.ToString("0.##")}\"");

                foreach (var bidPositions in oBook.Bids.Union(oBook.Asks))
                {
                    Log(OrderBookStatsCsvLog, $"OrderBook,{orderBook.Key}, {ticker}, \"{(bidPositions.Quantity * bidPositions.Price).ToString("0.###").Replace(".", ",")}\",{bidPositions.Price}");
                }

                foreach (var askAgregate in OrderBookProcessor.Aggregates(oBook, DateTime.Now))
                {
                    Log(OrderBookStatsCsvLog, $"OrderBookAskAgregated,{askAgregate.ToCsvString()}");
                }
            }
}
        public async Task<string> MonitorPriceAndReturnTicket(decimal umbral, int intervalMilliseconds, decimal umbralTotal)
        {
            currentData = new PriceData();
            initialPriceData = new PriceData();

            var exit = false;
            var monitorGlobalPercentage = false;

            Console.WriteLine($"Creating tasks:{DateTime.Now.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture)}");
            Log(BotEventsLog, $"Creating tasks:{DateTime.Now.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture)}");
            
            var getPriceDataTasks = GetPriceDataTasks();

            DateTime previousDateTime = DateTime.Now;
            DateTime currentDateTime;
            
            do
            {
                while (!Console.KeyAvailable && getPriceDataTasks.Count > 0)
                {
                    monitorGlobalPercentage = CheckStartMonitoringForGlobalPercentage(monitorGlobalPercentage);
                    Log(PriceMessagesLog,$"Monitoring Global {monitorGlobalPercentage}");
                    Console.WriteLine($"{GetTimeNowAsString()} -> {monitorGlobalPercentage}");
                   
                    Task<PriceData> firstFinishedTask = await Task.WhenAny<PriceData>(getPriceDataTasks.ToArray());
                    getPriceDataTasks.Remove(firstFinishedTask);

                    PriceData priceData = firstFinishedTask.Result;
                    Log(PriceJsonLog,JsonConvert.SerializeObject(priceData));

                    if (FindTickerOfPump(umbral, umbralTotal, priceData, monitorGlobalPercentage,
                        out var tickerOfPump))
                    {
                        exit = true; 
                        return tickerOfPump;
                    }

                    var priceDataTask = PriceDataTask(ref previousDateTime);
                    getPriceDataTasks.Add(priceDataTask);
                }

                var key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.Escape)
                {
                    exit = true;
                }

                if (key == ConsoleKey.Enter)
                {
                    initialPriceData = new PriceData();
                    monitorGlobalPercentage = true;
                }

            } 
            while (!exit);
            
            return "";
        }

        private Task<PriceData> PriceDataTask(ref DateTime previousDateTime)
        {
            DateTime currentDateTime;
            int maxTasks = _exchange.GetMaxTasks();
            int delay = _exchange.GetResponseTime() / maxTasks;
            currentDateTime = DateTime.Now;
            var milliseconds = (currentDateTime - previousDateTime).Milliseconds;
            previousDateTime = currentDateTime;
            var adjustedDelay = (milliseconds > delay) || milliseconds < 0 ? 0 : delay - milliseconds;
            var priceDataTask = GetPriceDataWithDelay((maxTasks * delay) + adjustedDelay);
            return priceDataTask;
        }

        private List<Task<PriceData>> GetPriceDataTasks()
        {
            List<Task<PriceData>> getPriceDataTasks = new List<Task<PriceData>>();
            int maxTasks = _exchange.GetMaxTasks();
            int delay = _exchange.GetResponseTime() / maxTasks;
            for (int i = 0; i < maxTasks; i++)
            {
                getPriceDataTasks.Add(GetPriceDataWithDelay(i * delay));
            }

            return getPriceDataTasks;
        }

        public async Task<PriceData> GetPriceDataWithDelay(int delay)
        {
            await Task.Delay(delay);
            return await _exchange.RefreshPriceMarketData();
        }

        private bool CheckStartMonitoringForGlobalPercentage(bool monitorGlobalPercentage)
        {
            if (!monitorGlobalPercentage)
            { 
                var dateNow = DateTime.Now;
                if (dateNow.Minute >= _startingMinute && dateNow.Second >= _startingSecond)
                {
                    initialPriceData = new PriceData();
                    monitorGlobalPercentage = true;
                }
            }
            return monitorGlobalPercentage;
        }

        private bool FindTickerOfPump(decimal umbral, decimal umbralTotal, PriceData priceData, bool monitorGlobalPercentage,
            out string symbolPriceSymbol)
        {
            foreach (var symbolPrice in LowVolumeTickers._tickers)
            {
               string symbol = symbolPrice;
               if (currentData.Data.ContainsKey(symbol))
               {
                   var oldprice = currentData.Data[symbol];
                   var percentageChange = (priceData.Data[symbol] - oldprice) / oldprice;
                   if (percentageChange > 0 && percentageChange > umbral)
                   {
                       Log(PriceMessagesLog, $"{symbolPrice} price up {(percentageChange * 100):#.00}% with price {priceData.Data[symbol]}");
                       Log(PriceCsvLog, $"{symbolPrice},\"{(percentageChange * 100):#,00}\", {priceData.Data[symbol]}, ComparePrevious");
                       ConsoleHelper.ConsolePrintPercentage(symbol, percentageChange);
                       
                   }

                   if (Math.Abs(percentageChange) > umbral && !monitorGlobalPercentage)
                   {
                       _excludingTickets.Add(symbol);
                       Log(BotEventsLog,$"{symbol} excluded");
                   }
               }

               if (monitorGlobalPercentage)
               {
                   if (initialPriceData.Data.ContainsKey(symbol))
                   {
                       var initialPrice = initialPriceData.Data[symbol];
                       var percentageChange = (priceData.Data[symbol] - initialPrice) / initialPrice;
                       if (percentageChange > 0 && percentageChange > umbralTotal)
                       {
                           Log(PriceMessagesLog, $"{symbol} price up Global % {(percentageChange * 100):#.00}% with price {priceData.Data[symbol]}");
                           Log(PriceCsvLog, $"{symbol},\"{(percentageChange * 100):#,00}\", {priceData.Data[symbol]}, CompareTotal");
                           ConsoleHelper.ConsolePrintGlobalPercentage(symbol, percentageChange);
                           if (percentageChange > umbralTotal)
                           {
                               symbolPriceSymbol = symbol; 
                               return true;
                           }
                       }
                   }
                   else
                   {
                       initialPriceData.Data[symbol] = priceData.Data[symbol];
                   }
               }

               currentData.Data[symbol] = priceData.Data[symbol];
            }
            symbolPriceSymbol = string.Empty;
            return false;
        }

        public async Task<Dictionary<DateTime, IBinanceKline>> GetKilines(string ticker, DateTime ini, DateTime end, KlineInterval interval = KlineInterval.OneMinute)
        {
            DateTime start = ini;
            end = new DateTime(end.Year,end.Month,end.Day,end.Hour,end.Minute,0);
            Dictionary<DateTime, IBinanceKline> result = new Dictionary<DateTime, IBinanceKline>();
            while (start < end)
            {
                var klines = await _exchange.GetKlinesAsync(ticker, interval, start);
                foreach (var binanceKline in klines)
                {
                    if (!result.ContainsKey(binanceKline.OpenTime))
                    {
                        result.Add(binanceKline.OpenTime,binanceKline);
                    }
                }
                start = klines.Last().OpenTime;
                Thread.Sleep(300);
            }

            return result;
        }

        public void PrintLogs()
        {
            var execPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Directory.CreateDirectory($"{execPath}\\logs");
            try
            {
                foreach (var logBucketName in memoryLog.Keys)
                {
                    if (memoryLog[logBucketName].Any())
                    { using (StreamWriter w = File.AppendText($"{execPath}\\logs\\{logBucketName}-{DateTime.Now.Ticks}-.csv"))
                        {
                            foreach (var logline in memoryLog[logBucketName])
                            {
                                w.WriteLine(logline);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(DateTime.Now.ToShortTimeString() + JsonConvert.SerializeObject(ex));
            }

            try
            {
                using (StreamWriter w = File.AppendText($"{execPath}\\logs\\{DateTime.Now.ToShortDateString().Replace("/", "-")}-ExcludingTickets.txt"))
                {
                    w.WriteLine($"ExcludingList: {JsonConvert.SerializeObject(_excludingTickets)}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(DateTime.Now.ToShortTimeString() + JsonConvert.SerializeObject(ex));
            }
        }

        public void PrintCsvLines(Dictionary<DateTime, IBinanceKline> data, string ticker)
        {
            var execPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Directory.CreateDirectory($"{execPath}\\logs");
            try
            {
                using (StreamWriter w = File.AppendText($"{execPath}\\logs\\{DateTime.Now.ToShortDateString().Replace("/", "-")}-{ticker}.csv"))
                {
                    w.WriteLine("OpenTime,CloseTime,BaseVolume,Open,Close,High,Low,TradeCount,QuoteVolume,TakerBuyBaseVolume,TakerBuyQuoteVolume,CommonOpen,CommonClose,CommonHigh,CommonLow,CommonVolume,CommonOpenTime");
                    foreach (var kline in data.Values)
                    {
                        w.WriteLine(CsvSerialize(kline));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(DateTime.Now.ToShortTimeString() + JsonConvert.SerializeObject(ex));
            }
        }

        private string CsvSerialize(IBinanceKline kline)
        {
            return
                $"{kline.OpenTime},{kline.CloseTime},{kline.BaseVolume},{kline.Open},{kline.Close},{kline.High},{kline.Low},{kline.TradeCount},{kline.QuoteVolume},{kline.TakerBuyBaseVolume},{kline.TakerBuyQuoteVolume},{kline.CommonOpen},{kline.CommonClose},{kline.CommonHigh},{kline.CommonLow},{kline.CommonVolume},{kline.CommonOpenTime}";
        }

        public void UpdateOrder(OrderDto order)
        {
            if (_orders.ContainsKey(order.OrderId))
            {
                _orders[order.OrderId] = order;
            }
            else
            {
                _orders.Add(order.OrderId, order);
            }
        }

        public void PrintOrders()
        {
            int pos = 7;
            foreach (var orderDto in _orders.Values)
            {
                ConsoleSafe.WriteLine($"{orderDto.OrderId}: {orderDto.Side} {orderDto.Status} {orderDto.Price} {orderDto.Quantity}", memoryLog[BotEventsLog], 0, pos);
                pos++;
            }
        }

        public async Task SellTrader(string ticker)
        {
            var openTrades = await _exchange.GetOpenTrades(new List<string>() { ticker });

        }

        // This one will not work for the pump because it takes long time to connect
        public void SubscribeToBookTickerUpdates(string ticker)
        {
            var client = new BinanceSocketClient(new BinanceSocketClientOptions
            {
                ApiCredentials = new ApiCredentials(_apiKey, _apiSecret)
            });
            
            client.Spot.SubscribeToTradeUpdates(ticker, data =>
            {
                var date = DateTime.Now;
                _lastPriceQueue.Enqueue(data.Price);
                var percentage = ((data.Price - FirstPriceQuantityResultUnsafe.Price) /
                                  FirstPriceQuantityResultUnsafe.Price) * 100;
                ConsoleSafe.WriteLine($"[{date.Minute}:{date.Second}] Price:{data.Price} - Quantity:{data.Quantity} {data.TradeTime}", memoryLog[BotEventsLog], 0, 0);
                ConsoleSafe.WriteLine($"PriceIncrease:{percentage:000.0}%              ", memoryLog[BotEventsLog], 0, 2);

                /* Show average price of last 5 updates */
                if (_lastPriceQueue.Count > 20)
                {
                    _lastPriceQueue.Dequeue();
                }

                if (_lastPriceQueue.Count > 20)
                {
                    _lastPriceAverageQueue.Dequeue();
                }

                decimal average = _lastPriceQueue.Average();
                _lastPriceAverageQueue.Enqueue(average);

                /* Compare growth */
                decimal pendiente = (_lastPriceQueue.Last() - _lastPriceQueue.First()) / _lastPriceQueue.First() * 100;
                decimal pendienteAverage = (_lastPriceAverageQueue.Last() - _lastPriceAverageQueue.First()) /
                                           _lastPriceAverageQueue.First() * 100;

                ConsoleSafe.WriteLine($"Pendiente:{pendiente:000.0}%   PendienteAverage:{pendienteAverage:000.0}%           ", memoryLog[BotEventsLog], 0, 4);

            });
        }

        public async Task<List<OrderDto>> GetOpenOrders(string ticker)
        {
            return await _exchange.GetOpenTrades(ticker);
        }

        public async Task CancelAllOrders(string ticker)
        {
            await _exchange.CloseOrders(ticker);
        }

        public decimal GetLastPrice()
        {
            return _lastPriceQueue.Last();
        }
    }
}