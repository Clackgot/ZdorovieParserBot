using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ParserNews
{
    public class ChannelUpdater
    {
        public ChannelUpdater(){}
        [XmlArray("CurrentNews"), XmlArrayItem("News")]
        public List<News> CurrentNews = new List<News>();
        [XmlArray("Distinction"), XmlArrayItem("News2")]
        private List<News> Distinction = new List<News>();


        public void Update(List<News> News)
        {
            foreach (var item in News)
            {
                if (!CurrentNews.Contains(item))
                {
                    CurrentNews.Add(item);
                    Distinction.Add(item);
                }
            }
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

            foreach (var news in allnews)
            {
                foreach (var newsItem in news.Result)
                {
                    Console.WriteLine(newsItem);
                }   
            }
            Console.WriteLine("--------------------------------------------------------------------------");
            foreach (var service in newsServices)
            {
                var serv = (NewsService)service;
                Console.WriteLine($"{serv.Name} {serv.NewsCount}");
            }
            Console.WriteLine("--------------------------------------------------------------------------");
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
                new Medicalnewstoday(),
                new Medscape(),
                new NewsMedical(),
                new Bmj() });
        }

    }

    public class Person
    {
        [XmlAttribute("Name")]
        public string Name;
        [XmlAttribute("Age")]
        public int Age;
    }
    public class Company
    {
        [XmlArray("CurrentNews"), XmlArrayItem("News")]
        public List<Person> People = new List<Person>();
        public Company()
        {
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
                $"<a href=\"{news.Url.Href}\">Источник</a>";
            var response = await bot.SendTextMessageAsync(new ChatId(channelName), message, Telegram.Bot.Types.Enums.ParseMode.Html, true);
            Thread.Sleep(3000);
            return response;
        }
    }
    
    class Program
    {
        static void Main(string[] args)
        {
            var parser = ParserFactory.Parser();
            parser.Parse().Wait();
            Bot bot = new Bot();
            foreach (var newsItem in parser.News)
            {
                bot.SendNews(newsItem).Wait();

            }
        }


        private static async Task Run()
        {
            List<News> AllNews = new List<News>();
            List<INewsService> newsServices = new List<INewsService>();
            newsServices.Add(new Medicalnewstoday());//https://www.medicalnewstoday.com
            newsServices.Add(new Medscape()); //https://www.medscape.com/index/list_13470
            newsServices.Add(new NewsMedical());//https://www.news-medical.net/syndication.axd?news=lifesciences
            newsServices.Add(new Bmj());//
            List<Task<IEnumerable<News>>> allnews = new List<Task<IEnumerable<News>>>();
            foreach (var service in newsServices)
            {
                allnews.Add(service.GetAllNewsAsync());
            }
            foreach (var news in allnews)
            {
                foreach (var item in news.Result)
                {
                    Console.WriteLine(item);
                }
            }

            // bot is working...

            StupidBot bot = new StupidBot();

            foreach (var news in AllNews)
            {
                //bot.SendMessage(news.Title);
            }

            Console.ReadKey();

        }

        private static void medicalnewstoday()
        {
            Medicalnewstoday medicalnewstoday = new Medicalnewstoday();
            var result = medicalnewstoday.GetAllNewsAsync().Result;
            foreach (var news in result)
            {
                Console.WriteLine(news);
            }
        }
        private static void medscape()
        {
            MedscapeService medscape = new MedscapeService();
            var result = medscape.GetAllNewsAsync().Result;
            foreach (var news in result)
            {
                Console.WriteLine(news);
            }
        }
    }
}
