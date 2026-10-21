using System.Collections.Generic;

namespace CryptoCoreNew.Model
{
    public class PriceData
    {
        public PriceData()
        {
            Data = new Dictionary<string, decimal>();
        }

        public Dictionary<string, decimal> Data { get; set; }
        public string ExchangeName { get; set; }
    }
}
