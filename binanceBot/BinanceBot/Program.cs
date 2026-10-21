using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Runtime.InteropServices;
using Binance.API.Csharp.Client.Models.WebSocket;

namespace BinanceBot
{
    class Program
    {
        static void  Main(string[] args)
        {
            Binance bi = new Binance(new TickerRules(), new HttpClientHelper());

            var stopwatch = Stopwatch.StartNew();

            var buyOrder = bi.BotBuyAsync().Result;
            var sellOrder = bi.BotPlaceSellOrder().Result;
            
            bi.BotManagePosition(sellOrder).Wait();

            bi.PrintLogs();
        }
    }
}
