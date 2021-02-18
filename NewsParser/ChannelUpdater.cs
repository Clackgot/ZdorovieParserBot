using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ParserNews
{
    public class ChannelUpdater
    {
        private Parser parser;
        private Bot bot;
        private List<News> publishedNews = new List<News>();
        private List<News> parsedNews = new List<News>();
        public ChannelUpdater(Parser parser, Bot bot)
        {
            this.parser = parser;
            this.bot = bot;
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
            parsedNews.Clear();
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
            TimeSpan timeout = TimeSpan.FromMinutes(3);
            //TimeSpan timeout = TimeSpan.FromSeconds(180);
            //TimeSpan timeout = TimeSpan.FromHours(0.05);
            for (int i = 0;; i++)
            {
                Console.Clear();
                Parse().Wait();
                Publish();
                SavePublishedNews();
                Console.WriteLine($"Ожидание {timeout.TotalMinutes} минут для проверки новостей");
                Thread.Sleep(timeout);
            }

        }
        public void SavePublishedNews()
        {
            var serialized = JsonConvert.SerializeObject(publishedNews);
            System.IO.File.WriteAllText("news.json", serialized);
        }
    }
}
