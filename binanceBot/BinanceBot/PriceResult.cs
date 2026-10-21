namespace BinanceBot
{
    public class PriceResult
    {
        public decimal Price { get; set; }
        public decimal Quantity { get; set; }
    }

    public class SymbolPriceResult
    {
        public decimal Price { get; set; }
        public string Symbol { get; set; }
    }
}