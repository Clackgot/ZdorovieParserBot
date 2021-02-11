using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ChannelBot
{
    class Program
    {
        public static async Task<Message> sendNews(string title)
        {

        }
        static void Main(string[] args)
        {
            string token = "1656601649:AAG1d5BQ8GCdkt5QP6PRKxriPFSewixNKFQ";

            string channelName = "@newstestbot1";
            string message = @"[курсив](https://tlgrm.ru/docs/bots/api)

qwe";
            var bot = new TelegramBotClient(token);
            bot.SendTextMessageAsync(new ChatId(channelName), message, Telegram.Bot.Types.Enums.ParseMode.MarkdownV2, true).Wait();
        }
    }
}
