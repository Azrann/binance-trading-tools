using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Binance.Net;
using Binance.Net.Enums;
using Binance.Net.Interfaces;
using Binance.Net.Objects.Spot;
using Binance.Net.Objects.Spot.MarketData;
using Binance.Net.Objects.Spot.SpotData;
using CryptoCoreNew.Model;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Objects;

namespace CryptoCoreNew
{
    public class BinanceExchange :IExchange
    {
        public readonly BinanceClient _api;
        private Dictionary<string, Currency> _currencies;
        private DateTime _lastGetFundRefresh;
        private Dictionary<string,CryptoBalance> _funds;
        private PriceData _marketPriceData;
        private Dictionary<string, MarketData> _marketsData;
        private TickerRules _tickerRules;

        public BinanceExchange(string apiKey, string apiSecret)
        {
            _api = new BinanceClient(new BinanceClientOptions
            {
                ApiCredentials = new ApiCredentials(apiKey, apiSecret)
            });
            _lastGetFundRefresh = DateTime.Now.AddDays(-1);
        }

        public Dictionary<string, MarketData> GetMarketData()
        {
            return _marketsData;
        }

        public List<string> GetMainCurrencies()
        {
            return new List<string>()
            {
                "BTC",
                "ETH",
                "BNB",
                "USDT"
            };
        }

        public async Task<Dictionary<string, MarketData>> RefreshMarketData(List<string> symbols)
        {
            
            var allMarketData = await _api.Spot.Market.GetAllBookPricesAsync();

            Dictionary<string, MarketData> result = new Dictionary<string, MarketData>();
            foreach (var market in allMarketData.Data)
            {
                result.Add(market.Symbol, new MarketData()
                {
                    AskPrice = market.BestAskPrice,
                    AskQty = market.BestAskQuantity,
                    BidPrice = market.BestBidPrice,
                    BidQty = market.BestBidQuantity,
                    Ticker = market.Symbol
                });
            }

            _marketsData = result;
            return result;
        }

        public PriceData GetPriceMarketData()
        {
            return _marketPriceData;
        }

        public async Task<PriceData> RefreshPriceMarketData()
        {
            try
            {
                var markets = await _api.Spot.Market.GetAllPricesAsync();

                PriceData result = new PriceData();
                foreach (var market in markets.Data)
                {
                    result.Data.Add(market.Symbol, market.Price);
                }

                _marketPriceData = result;
                return result;
            }
            catch (Exception ex)
            {
                return _marketPriceData;
            }
        }

        public async Task<List<string>> GetOpenTrades(List<string> symbols)
        {
            List < string > orderIds = new List<string>();
            foreach (var symbol in symbols)
            {
                var response = await _api.Spot.Order.GetOpenOrdersAsync(symbol);
                if (response != null && response.Success && response.Data.Any())
                {
                    orderIds.AddRange(response.Data.Select(x => x.OrderId.ToString()).ToList());
                }
                
            }

            return orderIds;
        }

        public async Task<List<OrderDto>> GetOpenTrades(string symbol)
        {
            List<OrderDto> orderDtos = new List<OrderDto>();
  
                var response = await _api.Spot.Order.GetOpenOrdersAsync(symbol);
                if (response != null && response.Success && response.Data.Any())
                {
                    foreach (var binanceOrder in response.Data)
                    {
                        orderDtos.Add(new OrderDto()
                        {
                            OrderId = binanceOrder.OrderId.ToString(),
                            Price = binanceOrder.Price,
                            Quantity = binanceOrder.Quantity,
                            Side = binanceOrder.Side.ToString(),
                            Status = binanceOrder.Status.ToString()
                        });
                    }
                }


            return orderDtos;
        }

        public async Task<List<string>> GetOpenTransfers()
        {
            List<string> orderIds = new List<string>();
            var response = await _api.WithdrawDeposit.GetWithdrawalHistoryAsync();
            if (response.Success && response.Data.Any())
            {
                orderIds.AddRange(response.Data.Where(x => x.Status == WithdrawalStatus.Processing ||
                                                                x.Status == WithdrawalStatus.AwaitingApproval).Select(y => y.Id).ToList());
            }

            return orderIds;
        }

        public async Task<Dictionary<string,decimal>> GetBalances()
        {
            var accountInfo = await _api.General.GetAccountInfoAsync();

            Dictionary<string, decimal> result = new Dictionary<string, decimal>();
            foreach (var balance in accountInfo.Data.Balances)
            {
                result.Add(balance.Asset, balance.Free);
            }
            return result;
        }

        public async Task CloseOrders(string simbol)
        {
            try
            {
                var result = await _api.Spot.Order.CancelAllOpenOrdersAsync(simbol);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public async Task<string> GetDepositAddress(string currency)
        {
            var result = await _api.WithdrawDeposit.GetDepositAddressAsync(currency);
            return result.Data.Address;
        }

        public async Task<BuyOrderResult> CreateBuyLimitOrder(string symbol, decimal quantity, decimal price)
        {
            try
            {
                var tickerRules = await GetTickerRules();
                var safePrice = Math.Round(price, tickerRules.Rules[symbol].MinPriceDecimals);
                var safeQuantity = Math.Round(quantity, tickerRules.Rules[symbol].MinAmountDecimals);
                var response = await _api.Spot.Order.PlaceOrderAsync(symbol, OrderSide.Buy, OrderType.Limit, safeQuantity, price: safePrice, timeInForce: TimeInForce.GoodTillCancel);
                return new BuyOrderResult()
                {
                    OrderId = response.Data.OrderId,
                    QuantityFilled = response.Data.QuantityFilled,
                    QuoteQuantityFilled = response.Data.QuoteQuantityFilled,
                    Price = response.Data.Fills.First().Price
                };
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<BuyOrderResult> CreateBuyMarketOrder(string symbol, decimal quantity, decimal quoteOrderQty = 0)
        {
            try
            {
                WebCallResult<BinancePlacedOrder> response;
                var tickerRules = await GetTickerRules();
                if (quoteOrderQty == 0)
                {
                    var safeQuantity = Math.Round(quantity, tickerRules.Rules[symbol].MinAmountDecimals);
                    response = await _api.Spot.Order.PlaceOrderAsync(symbol, OrderSide.Buy, OrderType.Market, safeQuantity).ConfigureAwait(false);
                }
                else
                {
                   response = await _api.Spot.Order.PlaceOrderAsync(symbol, OrderSide.Buy, OrderType.Market, quoteOrderQuantity: quoteOrderQty).ConfigureAwait(false);
                }

                return new BuyOrderResult()
                {
                    OrderId = response.Data.OrderId,
                    QuantityFilled = response.Data.QuantityFilled,
                    QuoteQuantityFilled = response.Data.QuoteQuantityFilled,
                    Price = response.Data.Fills.First().Price
                };
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<string> CreateSellMarketOrder(string symbol, decimal quantity)
        {
            try
            {
                var tickerRules = await GetTickerRules();
                var safeQuantity = Math.Round(quantity, tickerRules.Rules[symbol].MinAmountDecimals);
                var response = await _api.Spot.Order.PlaceOrderAsync(symbol, OrderSide.Sell, OrderType.Market, safeQuantity).ConfigureAwait(false);
                return response.Data.OrderId.ToString();
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public async Task<OrderDto> CreateSellLimitOrder(string symbol, decimal quantity, decimal price)
        {
            try
            {
                var tickerRules = await GetTickerRules();
                var safePrice = Math.Round(price, tickerRules.Rules[symbol].MinPriceDecimals);
                var safeQuantity = Math.Round(quantity, tickerRules.Rules[symbol].MinAmountDecimals);
                var response = await _api.Spot.Order.PlaceOrderAsync(symbol, OrderSide.Sell, OrderType.Limit, safeQuantity, price: safePrice, timeInForce: TimeInForce.GoodTillCancel);
                return new OrderDto()
                {
                    OrderId = response.Data.OrderId.ToString(),
                    Quantity = response.Data.Quantity,
                    Status = response.Data.Status.ToString(),
                    Side = response.Data.Side.ToString(),
                    Price = response.Data.Price
                };
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> CreateWithdraw(string currency, decimal amount, string address)
        {
            try
            {
                var tickerRules = await GetTickerRules();
                var safeQuantity = Math.Round(amount, tickerRules.Rules[$"{currency}BTC"].MinAmountDecimals);
                var response = await _api.WithdrawDeposit.WithdrawAsync(currency, address, safeQuantity);
                return response.Success;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<string> Transfer(string currency, decimal amount, string address)
        {
            try
            {
                var tickerRules = await GetTickerRules();
                var safeQuantity = Math.Round(amount, tickerRules.Rules[$"{currency}BTC"].MinAmountDecimals);
                var response = await _api.WithdrawDeposit.WithdrawAsync(currency, address, safeQuantity);
                return response.Data.Id;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public async Task<IEnumerable<IBinanceKline>> GetKlinesAsync(string ticker, KlineInterval oneMinute, DateTime start)
        {
            var result = await _api.Spot.Market.GetKlinesAsync(ticker, KlineInterval.OneMinute, start);
            return result.Data;
        }

        public async Task<List<SymbolVolume>> GetMarketVolume()
        {
            List<SymbolVolume> result = new List<SymbolVolume>();
            var response = await _api.Spot.Market.Get24HPricesAsync();
            if (!response.Success)
            {
                return null;
            }

            foreach (var market in response.Data)
            {
                result.Add(new SymbolVolume()
                {
                    Symbol = market.Symbol,
                    Volume = market.QuoteVolume
                });
            }
            return result;
        }

        #region tickerRules
        public async Task<TickerRules> GetTickerRules()
        {
            if (_tickerRules == null)
            {
                return await RefreshTickerRules();
            }

            return _tickerRules;
        }

        public async Task<TickerRules> RefreshTickerRules()
        {
            var result = new TickerRules { Rules = new Dictionary<string, Rule>() };

            var response = await _api.Spot.System.GetExchangeInfoAsync();
            foreach (var market in response.Data.Symbols)
            {
                    result.Rules.Add(
                    market.Name,
                    new Rule(GetNumDecimalPlaces(market.LotSizeFilter.MinQuantity.ToString()),
                        GetNumDecimalPlaces(market.PriceFilter.MinPrice.ToString())));
            }

            _tickerRules = result;
            return result;
        }

        #endregion

        public int GetMaxTasks()
        {
            return 5;
        }

        public int GetResponseTime()
        {
            return 450;
        }

        public string GetExchangeName()
        {
            return "Binance";
        }

        public MarketFee GetMarketFee()
        {
            return new MarketFee()
            {
                BuyingFee = 0.001m,
                SellingFee = 0.001m
            };
        }

        public Task<Dictionary<string, Currency>> GetCurrencies()
        {
            if (_currencies != null)
            {
                return Task.FromResult(_currencies);
            }

            BinanceFees serviceFees = new BinanceFees();
            var currencies = serviceFees.FetchWebSite();
            _currencies = new Dictionary<string, Currency>();

            foreach (var currency in currencies)
            {
                _currencies.Add(currency.assetCode, new Currency()
                {
                    MinWithdraw = decimal.Parse(currency.minProductWithdraw),
                    Symbol = currency.assetCode,
                    Name = currency.assetName,
                    WithdrawFee = System.Convert.ToDecimal(currency.transactionFee)
                });
            }

            return Task.FromResult(_currencies);
        }

        public async Task<Dictionary<string, CryptoBalance>> GetFunds(bool force = false)
        {
            if (force || _lastGetFundRefresh.AddMinutes(1) < DateTime.Now)
            {
                Console.WriteLine($"Get funds force = {force}");
                var result = new Dictionary<string, CryptoBalance>();

                var response = await _api.General.GetAccountInfoAsync();
                foreach (var balance in response.Data.Balances)
                {
                    result.Add(balance.Asset, new CryptoBalance()
                    {
                        Symbol = balance.Asset,
                        Available = balance.Free,
                        CanTradeOrDeposit = response.Data.CanDeposit && response.Data.CanTrade && response.Data.CanWithdraw
                    });
                }

                _lastGetFundRefresh = DateTime.Now;
                _funds = result;
            }

            return _funds;
        }

        private int GetNumDecimalPlaces(string argument)
        {
            decimal number = decimal.Parse(Remove0FromRight(argument));
            return BitConverter.GetBytes(decimal.GetBits(number)[3])[2];
        }

        private string Remove0FromRight(string argument)
        {
            if (argument.EndsWith("0"))
            {
                return Remove0FromRight(argument.Substring(0, argument.Length - 1));
            }
            return argument;
        }

        public async Task<string> GetBidAsk()
        {
            var result = await _api.Spot.Market.GetOrderBookAsync("ASTBTC",1000);
            var result2 = await _api.Spot.Market.GetSymbolTradesAsync("ASTBTC");
            return "";
        }

        public async Task<BinanceOrderBook> GetOrderBookAsync(string ticker, int limit = 1000)
        {
            try
            {
                var result = await _api.Spot.Market.GetOrderBookAsync(ticker, limit);
                if (result.Data != null)
                {
                    return result.Data;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public async Task<IEnumerable<IBinanceRecentTrade>> GetLastTrades(string ticker)
        {
            try
            {
                var result = await _api.Spot.Market.GetSymbolTradesAsync(ticker);
                return result.Data;
            }
            catch (Exception ex)
            {
                return new List<IBinanceRecentTrade>();
            }

        }
    }
}
