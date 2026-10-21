using System;
using System.Collections.Generic;
using System.Text;

namespace BinanceBot.Common
{
    public class TelegramMessageExtractor
    {
        public static string Extract(string message)
        {
            var minutes = DateTime.Now.Minute;
            var seconds = DateTime.Now.Second;

            if ((minutes == 59 && seconds > 55) || (minutes == 00 && seconds < 30))
            {
                int ini = message.IndexOf("$");
                if (ini > 0)
                { 
                    var submessage = message.Substring(ini, message.Length - 1 - ini);
                    var coin = submessage.Split(" ")[0].Replace("$", "") + "BTC";
                    if (LowVolumeTickers.Tickers.Contains(coin))
                    {
                        return coin.ToUpper();
                    }
                }

                foreach (var ticker in LowVolumeTickers.Tickers)
                {
                    if (message.ToUpper().Contains(ticker.Substring(0, ticker.Length - 3).ToUpper()))
                    {
                        return ticker;
                    }
                }

                return null;
            }

            return null;
        }
    }
}
