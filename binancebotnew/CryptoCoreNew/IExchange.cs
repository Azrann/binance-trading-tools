using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Binance.Net.Enums;
using Binance.Net.Interfaces;
using Binance.Net.Objects.Spot.MarketData;
using CryptoCoreNew.Model;

namespace CryptoCoreNew
{
    public interface IExchange
    {
        Dictionary<string, MarketData> GetMarketData();
        List<string> GetMainCurrencies();
        Task<Dictionary<string, MarketData>> RefreshMarketData(List<string> symbols);
        PriceData GetPriceMarketData();
        Task<PriceData> RefreshPriceMarketData();
        Task<List<string>> GetOpenTrades(List<string> symbols);
        Task<List<OrderDto>> GetOpenTrades(string symbol);
        Task<List<string>> GetOpenTransfers();
        Task<Dictionary<string, decimal>> GetBalances();
        Task CloseOrders(string simbol);
        Task<string> GetDepositAddress(string currency);
        Task<BuyOrderResult> CreateBuyLimitOrder(string symbol, decimal quantity, decimal price);
        Task<BuyOrderResult> CreateBuyMarketOrder(string symbol, decimal quantity, decimal quoteOrderQty=0);
        Task<string> CreateSellMarketOrder(string symbol, decimal quantity);
        Task<OrderDto> CreateSellLimitOrder(string symbol, decimal quantity, decimal price);
        Task<bool> CreateWithdraw(string currency, decimal amount, string address);
        Task<List<SymbolVolume>> GetMarketVolume();
        Task<TickerRules> GetTickerRules();
        Task<TickerRules> RefreshTickerRules();
        int GetMaxTasks();
        int GetResponseTime();
        string GetExchangeName();
        MarketFee GetMarketFee();
        Task<Dictionary<string, Currency>> GetCurrencies();
        Task<Dictionary<string, CryptoBalance>> GetFunds(bool force = false);
        Task<string> Transfer(string currency, decimal amount, string address);
        Task<IEnumerable<IBinanceKline>> GetKlinesAsync(string ticker, KlineInterval oneMinute, DateTime start);
        Task<string> GetBidAsk();
        Task<IEnumerable<IBinanceRecentTrade>> GetLastTrades(string ticker);
        Task<BinanceOrderBook> GetOrderBookAsync(string ticker, int limit = 1000);
    }
}
