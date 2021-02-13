using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ParserNews
{
    class Bot
    {
        public string token = "1656601649:AAG1d5BQ8GCdkt5QP6PRKxriPFSewixNKFQ";

        public string channelName = "@newstestbot1";


        private TelegramBotClient bot;
        public Bot(string token)
        {
            this.token = token;
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
