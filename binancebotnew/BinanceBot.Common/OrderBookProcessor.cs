using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Binance.Net.Objects;
using Binance.Net.Objects.Spot.MarketData;

namespace BinanceBot.Common
{
    public static class OrderBookProcessor
    {
        public static OrderBookAskDto Aggregate(BinanceOrderBook orderBook, int rangeFrom, int rangeTo, DateTime date)
        {
            decimal bestAsk = orderBook.Asks.First().Price;
            decimal rangePriceFrom = bestAsk * (rangeFrom + 100) /100;
            decimal rangePriceTo = bestAsk * (rangeTo + 100) / 100; 

            var askList = orderBook.Asks.Where(x => x.Price > rangePriceFrom && x.Price < rangePriceTo);
            decimal sumAskVolum = askList.Sum(x => x.Price * x.Quantity);
            var askCount = askList.Count();
            
            return new OrderBookAskDto()
            {
                StartRangePercentage = rangeFrom,
                EndRangePercentage = rangeTo,
                Symbol = orderBook.Symbol,
                DateTime = date,
                SumAskVolum = sumAskVolum,
                AskVentaCount = askCount,
                AvgAskVolume = askCount > 0 ? sumAskVolum / askCount : 0
            };
        }

        public static List<OrderBookAskDto> Aggregates(BinanceOrderBook orderBook, DateTime date)
        {
            List <OrderBookAskDto> result = new List<OrderBookAskDto>();
            result.Add(Aggregate(orderBook,0,50,date));
            result.Add(Aggregate(orderBook, 50, 100, date));
            result.Add(Aggregate(orderBook, 100, 150, date));
            result.Add(Aggregate(orderBook, 150, 225, date));
            result.Add(Aggregate(orderBook, 225, 300, date));
            result.Add(Aggregate(orderBook, 300, 400, date));
            result.Add(Aggregate(orderBook, 400, 500, date));
            result.Add(Aggregate(orderBook, 500, 600, date));
            result.Add(Aggregate(orderBook, 600, 700, date));
            result.Add(Aggregate(orderBook, 700, 800, date));
            result.Add(Aggregate(orderBook, 800, 900, date));
            result.Add(Aggregate(orderBook, 900, 1000, date));
            return result;
        }
    }

    public class OrderBookAskDto
    {
        public int StartRangePercentage { get; set; }
        public int EndRangePercentage { get; set; }
        public string Symbol { get; set; }
        public DateTime DateTime { get; set; }
        public decimal SumAskVolum { get; set; }
        public int AskVentaCount { get; set; }
        public decimal AvgAskVolume { get; set; }

        public string ToCsvString()
        {
            return
                $"{DateTime},{Symbol},{AskVentaCount},{SumAskVolum.ToCsvString()},{AvgAskVolume.ToCsvString()},{StartRangePercentage},{EndRangePercentage}";
        }
    }

    public static class Extensions
    {
        public static string ToCsvString(this decimal value)
        {
            return $"\"{value.ToString("0.###").Replace(".",",")}\"";
        }
    }
}
