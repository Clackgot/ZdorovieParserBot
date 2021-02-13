using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ParserNews
{

    public class ChannelUpdater
    {
        private Parser parser;
        private Bot bot = new Bot();
        private List<News> publishedNews = new List<News>();
        private List<News> parsedNews = new List<News>();
        public ChannelUpdater(Parser parser)
        {
            this.parser = parser;
            Console.CancelKeyPress += Console_CancelKeyPress;
            LoadPublishedNews();
        }

        private void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            SavePublishedNews();
        }
        public void Publish()
        {
            for (int i = 0; i < parsedNews.Count; i++)
            {
                if (!publishedNews.Exists(n => n.Url == parsedNews[i].Url))
                {
                    publishedNews.Add(parsedNews[i]);
                    bot.SendNews(parsedNews[i]).Wait();
                    Console.WriteLine($"{i + 1}/{parsedNews.Count}");
                    Thread.Sleep(TimeSpan.FromSeconds(3));
                }
            }
        }

        public async Task Parse()
        {
            await parser.Parse();
            parsedNews = parser.News;
        }

     
        public void LoadPublishedNews()
        {
            if (System.IO.File.Exists("news.json"))
            {
                var serialized = System.IO.File.ReadAllText("news.json");
                if (serialized.Length > 0) publishedNews = JsonConvert.DeserializeObject<List<News>>(serialized);
            }
            else
            {
                System.IO.File.Create("news.json");
            }
        }
        public void Run()
        {
            for (int i = 0;; i++)
            {
                Parse().Wait();
                Publish();
                Thread.Sleep(TimeSpan.FromMinutes(0.2));
            }

        }
        public void SavePublishedNews()
        {
            var serialized = JsonConvert.SerializeObject(publishedNews);
            System.IO.File.WriteAllText("news.json", serialized);
        }
        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            string resultString = "";
            foreach (var news in publishedNews)
            {
                resultString += news;
                resultString += "\n";
            }

            return resultString.ToString();
        }
    }
    public class Parser
    {
        private List<INewsService> newsServices = new List<INewsService>();
        public List<News> News { get; private set; } = new List<News>();
        public Parser(List<INewsService> services)
        {
            newsServices = services;
        }
        public async Task Parse()
        {
            List<Task<IEnumerable<News>>> allnews = new List<Task<IEnumerable<News>>>();
            foreach (var service in newsServices)
            {
                allnews.Add(service.GetAllNewsAsync());
            }
            await Task.WhenAll(allnews.ToArray());

            Console.Clear();
            foreach (var service in newsServices)
            {
                var serv = (NewsService)service;
                Console.WriteLine($"{serv.Name} {serv.NewsCount}");
            }
            foreach (var item in allnews)
            {
                News.AddRange(item.Result);
            }
        }
    }
    static class ParserFactory
    {
        public static Parser Parser()
        {
            return new Parser(new List<INewsService>() {
                //new Medicalnewstoday(),
                //new Medscape(),
                //new NewsMedical(),
                new Bmj() });
        }

    }


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

    class Program
    {
        static void Main(string[] args)
        {
            var updater = new ChannelUpdater(ParserFactory.Parser());
            //updater.Parse().Wait();
            Console.WriteLine(updater);
            updater.Run();
        }
    }
}
