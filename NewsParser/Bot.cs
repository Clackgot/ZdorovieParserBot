using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ParserNews
{
    public static class BotFactory
    {
        public static Bot ReleasedBot()
        {
            return new Bot("1527590284:AAFu8RGnAiiJh6qkdhmKUqkvs48oMmCi0ow", "newstestbot1");
        }
        public static Bot DebugBot()
        {
            return new Bot("1527590284:AAFu8RGnAiiJh6qkdhmKUqkvs48oMmCi0ow", "debugzdoroviebot");
        }
    }
    public class Bot
    {
        public string token;

        public string channelName;


        private TelegramBotClient bot;
        public Bot(string token, string channelName)
        {
            this.token = token;
            this.channelName = "@" + channelName;
            bot = new TelegramBotClient(this.token);
        }
        public Bot()
        {
            bot = new TelegramBotClient(this.token);
        }
        public async Task<Message> SendNews(News news)
        {
            string message = $"<b>{news.Title}</b>" +
                $"\n\n" +
                $"{news.Teaser}" +
                $"\n\n" +
                $"<a href=\"{news.Url}\">Источник</a>";
            var response = await bot.SendTextMessageAsync(new ChatId(channelName), message, Telegram.Bot.Types.Enums.ParseMode.Html, true);
            Thread.Sleep(3000);
            return response;
        }
    }
}
