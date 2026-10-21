using System;
using System.Linq;
using System.Threading.Tasks;
using Mentor.telegram.client;
using Newtonsoft.Json;
using OpenTl.Schema;

namespace BinanceBot.Common
{
    public class TelegramReader
    {
        private long ChannelId;
        private string ticker;
        public async Task<string> ReadTicker(string channelName)
        {
            ClientFactory clientFactory = new ClientFactory();
            var client = await clientFactory.GetClient();
            var bot = new TelegramBot(client);
            var result = await bot.FindChatData(channelName);
            ChannelId = result.ChatId;

            client.UpdatesService.AutoReceiveUpdates += Updates;
            if (ChannelId == 0)
            {
                Console.WriteLine($"ERROR cannot find channel name {channelName}");
                throw new Exception();
            }

            using (client)
            {
                Console.WriteLine($"Leyendo Telegram {channelName}");
                while (string.IsNullOrEmpty(ticker))
                {
                }
            }

            return ticker;
        }

        private void Updates(IUpdates update)
        {
            Console.WriteLine("Update Received");
            switch (update)
            {
                case TUpdates updates:
                    if (updates.Updates.Any())
                    {
                        if (updates.Updates[0] is TUpdateNewChannelMessage)
                        {
                            var message = ((updates.Updates[0] as TUpdateNewChannelMessage).Message as TMessage);

                            var textMessage = message.Message;
                            var channelId = ((TPeerChannel) ((TMessage)message)?.ToId)?.ChannelId;
                            if (channelId == ChannelId)
                            {
                                var candidateticker = ProcessMessage(message.Message);
                                if (!string.IsNullOrEmpty(candidateticker))
                                {
                                    ticker = candidateticker;
                                }
                            }
                        }
                        if (updates.Updates[0] is TUpdateNewMessage)
                        {
                            var message = ((updates.Updates[0] as TUpdateNewMessage).Message as TMessage);
                            var textMessage = message.Message;
                            var channelId = ((TPeerChannel)((TMessage)message)?.ToId)?.ChannelId;
                            if (channelId == ChannelId)
                            {
                                var candidateticker = ProcessMessage(message.Message);
                                if (!string.IsNullOrEmpty(candidateticker))
                                {
                                    ticker = candidateticker;
                                }
                            }
                        }
                    }

                    
                    break;
                case TUpdatesCombined updatesCombined:
                    //Console.WriteLine(JsonConvert.SerializeObject(updatesCombined));
                    break;
                case TUpdateShort updateShort:
                   // Console.WriteLine(JsonConvert.SerializeObject(updateShort));
                    break;
                case TUpdateShortChatMessage updateShortChatMessage:
                   // Console.WriteLine(JsonConvert.SerializeObject(updateShortChatMessage));
                    break;
                case TUpdateShortMessage updateShortMessage:
                   // Console.WriteLine(JsonConvert.SerializeObject(updateShortMessage));
                    break;
                case TUpdateShortSentMessage updateShortSentMessage:
                   // Console.WriteLine(JsonConvert.SerializeObject(updateShortSentMessage));
                    break;
                case TUpdatesTooLong updatesTooLong:
                   // Console.WriteLine(JsonConvert.SerializeObject(updatesTooLong));
                    break;
                default:
                  //  Console.WriteLine(JsonConvert.SerializeObject(update));
                    break;
            }
        }

        private string ProcessMessage(string message)
        {
            return TelegramMessageExtractor.Extract(message);
        }

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }
    }
}
