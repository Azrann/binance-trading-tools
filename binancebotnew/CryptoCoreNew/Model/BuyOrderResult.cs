using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoCoreNew.Model
{
    public class BuyOrderResult
    {
        public long OrderId { get; set; }
        public decimal QuoteQuantityFilled { get; set; }
        public decimal QuantityFilled { get; set; }
        public decimal Price { get; set; }
    }
}
