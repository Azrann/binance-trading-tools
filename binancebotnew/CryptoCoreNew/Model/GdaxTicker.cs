using System;

namespace CryptoCoreNew.Model
{
    public class GdaxTicker
    {
        public string type { get; set; }
        public long sequence { get; set; }
        public string product_id { get; set; }
        public string price { get; set; }
        public string open_24h { get; set; }
        public string volume_24h { get; set; }
        public string low_24h { get; set; }
        public string high_24h { get; set; }
        public string volume_30d { get; set; }
        public string best_bid { get; set; }
        public string best_ask { get; set; }
        public string side { get; set; }
        public DateTime time { get; set; }
        public int trade_id { get; set; }
        public string last_size { get; set; }
    }
}
