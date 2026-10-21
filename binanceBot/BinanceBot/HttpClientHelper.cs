using System;
using System.Diagnostics;
using System.Net.Http;
using Newtonsoft.Json;

namespace BinanceBot
{
    public class HttpClientHelper
    {
        private static HttpClient _client;

        public HttpClientHelper()
        {
            _client = new HttpClient();
        }

        public decimal GetTickerPrice(string pair)
        {
            var response = _client.GetAsync($"https://api.binance.com/api/v1/ticker/price?symbol={pair.ToUpper()}").Result;
            var result = JsonConvert.DeserializeObject<SymbolPriceResult>(response.Content.ReadAsStringAsync().Result);
            return result.Price;
        }
    }
}