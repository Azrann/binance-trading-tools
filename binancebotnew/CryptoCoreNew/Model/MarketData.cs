using System.Collections.Generic;

namespace CryptoCoreNew.Model
{
    public class MarketData
    {
        public string Ticker { get; set; }
        public decimal AskPrice { get; set; }
        public decimal AskQty { get; set; }
        public decimal BidPrice { get; set; }
        public decimal BidQty { get; set; }
    }

    public class MarketAllInfo
    {
        public string Ticker { get; set; }
        public List<PriceQuantity> Ask { get; set; }
        public List<PriceQuantity> Bid { get; set; }
    }

    public class PriceQuantity
    {
        public decimal Price { get; set; }
        public decimal Quantity { get; set; }
    }
}
