using System.Collections.Generic;

namespace BinanceBot.Common
{
    public class LowVolumeTickers
    {
        public static List<string> _tickers;

        public static List<string> Tickers => _tickers;

        public static void Initialize()
        {
            _tickers = new List<string>
            {
            };
        }
    }
}
