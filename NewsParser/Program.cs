using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Io;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ParserNews
{
    /// <summary>
    /// Модель новости
    /// </summary>
    public class News
    {
        public News(string title, string teaser, Url url)
        {
            Title = title;
            Teaser = teaser;
            Url = url;
        }
        public string Title { get; set; }
        public string Teaser { get; set; }
        public Url Url { get; set; }

        public override string ToString()
        {
            return "Title: " + Title + "\n"
                + "Teaser: " +  Teaser + "\n" +
                "Url: " + Url + "\n";
        }
    }

    public interface INewsService
    {
        Task<IEnumerable<News>> GetAllNewsAsync();
    }
    public abstract class NewsService : INewsService
    {
        protected abstract string BaseUrl { get;}

        protected List<News> allNews = new List<News>();
        protected static int getMonthNumber(string monthName)
        {
            switch (monthName)
            {
                case "January":
                    return 1;
                case "February":
                    return 2;
                case "March":
                    return 3;
                case "April":
                    return 4;
                case "May":
                    return 5;
                case "June":
                    return 6;
                case "July":
                    return 7;
                case "August":
                    return 8;
                case "September":
                    return 9;
                case "October":
                    return 10;
                case "November":
                    return 11;
                case "December":
                    return 12;
                default:
                    return 0;
            }
        }
        protected IBrowsingContext context;
        public NewsService()
        {
            var config = Configuration.Default
                .WithDefaultCookies()
                .WithDefaultLoader();//Использовать стандартный загрузчик и использовать куки
            context = BrowsingContext.New(config);//Инициализация конекста отправки запросов(а-ля сессия)
        }
        public abstract Task<IEnumerable<News>> GetAllNewsAsync();
    }

    public class MedscapeService : NewsService
    {
        protected override string BaseUrl => "https://www.medscape.com/index/list_13470_";

        
        private DateTime getDate(IElement dateElement)
        {
            string date = dateElement.TextContent;
            string month = date.Split(", ")[1].Split(' ')[0];
            string day = date.Split(", ")[1].Split(' ')[1];
            string year = date.Split(", ")[2];
            return new DateTime(int.Parse(year), getMonthNumber(month), int.Parse(day));
        }
        
        public override async Task<IEnumerable<News>> GetAllNewsAsync()
        {
            for (int i = 0;; i++)
            {
                var documentRequest = DocumentRequest.Get(new Url(BaseUrl + i.ToString()));
                var result = await context.OpenAsync(documentRequest);
                var liTags = result.QuerySelectorAll("div#archives li");
                bool validPage = false;
                foreach (var li in liTags)
                {
                    var date = li.QuerySelector("div.byline");
                    var title = li.QuerySelector("a.title").TextContent;
                    var teaser = li.QuerySelector("span.teaser").TextContent;
                    var newsUrl = li.QuerySelector("a.title").GetAttribute("href");
                    newsUrl = @"https:" + newsUrl;
                    
                    if (getDate(date) == DateTime.Today)
                    {
                        allNews.Add(new News(title, teaser, new Url(newsUrl)));
                        validPage = true;
                    }
                }
                if (!validPage) break;
            }
            return allNews;
        }
    }

    public class Medicalnewstoday : NewsService
    {
        protected override string BaseUrl => "https://www.medicalnewstoday.com";
        private int tiserMaxLength = 100;
        public override async Task<IEnumerable<News>> GetAllNewsAsync()
        {
            List<Task<IDocument>> documents = new List<Task<IDocument>>();
            var documentRequest = DocumentRequest.Get(new Url(BaseUrl));
            var result = await context.OpenAsync(documentRequest);
            List<string> latestNewsLinks = new List<string>();
            var latestNews = result.QuerySelector("div[id='LATEST NEWS']");
            var newsLinks = latestNews.QuerySelectorAll("li p a");
            foreach (var link in newsLinks)
            {
                var fullLink = BaseUrl + link.GetAttribute("href");
                latestNewsLinks.Add(fullLink);
                var docRequest = DocumentRequest.Get(new Url(fullLink));
                var doc = context.OpenAsync(docRequest);
                documents.Add(doc);
            }
            Task.WaitAll(documents.ToArray());
            var completeDocuments = documents.ConvertAll(doc => doc.Result);
            Regex regex = new Regex("publishedDate\":\".*?\"", RegexOptions.Compiled | RegexOptions.IgnoreCase);

            foreach (var document in completeDocuments)
            {
                string dateString = regex.Match(document.Source.Text).Value.Split("\":\"")[1].Trim('"');
                if((DateTime.Parse(dateString)) == DateTime.Today)
                {
                    var title = document.QuerySelector("div.css-z468a2 h1").TextContent;
                    var teaser = document.QuerySelector("article div p").TextContent;
                    bool needPoints = tiserMaxLength < teaser.Length;
                    teaser = teaser.Substring(0, Math.Min(teaser.Length, tiserMaxLength-3));
                    if(needPoints) teaser += "...";
                    var url = document.Url;
                    allNews.Add(new News(title, teaser, new Url(url)));
                }
                
            }
            

            return allNews;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            List<News> AllNews = new List<News>();
            List<INewsService> newsServices = new List<INewsService>();
            newsServices.Add(new Medicalnewstoday());
            newsServices.Add(new MedscapeService());

            foreach (var service in newsServices)
            {
                foreach (var item in service.GetAllNewsAsync().Result)
                {
                    AllNews.Add(item);
                }
            }
            foreach (var news in AllNews)
            {
                Console.WriteLine(news);
            }
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
