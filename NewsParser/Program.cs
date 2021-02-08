using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Io;
using System;
using System.Collections.Generic;
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
            return ">>" + Title + "\n"
                + Teaser + "\n" +
                Url + "\n";
        }
    }

    public interface INewsService
    {
        Task<IEnumerable<News>> GetAllNewsAsync(Url url);
    }
    public abstract class NewsService : INewsService
    {
        protected IBrowsingContext context;
        public NewsService()
        {
            var config = Configuration.Default
                .WithDefaultCookies()
                .WithDefaultLoader();//Использовать стандартный загрузчик и использовать куки
            context = BrowsingContext.New(config);//Инициализация конекста отправки запросов(а-ля сессия)
        }
        public abstract Task<IEnumerable<News>> GetAllNewsAsync(Url url);
    }

    public class MedscapeService : NewsService
    {
        private string baseUrl = "https://www.medscape.com/index/list_13470_";
        
        private int getMonthNumber(string monthName)
        {
            switch(monthName)
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
        private DateTime getDate(IElement dateElement)
        {
            string date = dateElement.TextContent;
            string month = date.Split(", ")[1].Split(' ')[0];
            string day = date.Split(", ")[1].Split(' ')[1];
            string year = date.Split(", ")[2];
            return new DateTime(int.Parse(year), getMonthNumber(month), int.Parse(day));
        }
        
        public override async Task<IEnumerable<News>> GetAllNewsAsync(Url url)
        {
            List<News> allNews = new List<News>();
            for (int i = 0;; i++)
            {
                var documentRequest = DocumentRequest.Get(new Url(baseUrl + i.ToString()));
                var result = await context.OpenAsync(documentRequest);//Получаем результат нашего запроса на отправку письма (при готовности)
                var liTags = result.QuerySelectorAll("div#archives li");
                bool validPage = false;
                foreach (var li in liTags)
                {
                    var date = li.QuerySelector("div.byline");
                    var title = li.QuerySelector("a.title").TextContent;
                    var teaser = li.QuerySelector("span.teaser").TextContent;
                    var newsUrl = li.QuerySelector("a.title").GetAttribute("href");
                    
                    if (getDate(date) == DateTime.Today)
                    {
                        Console.WriteLine(baseUrl + i.ToString() +  " " + date.TextContent);
                        allNews.Add(new News(title, teaser, new Url(url)));
                        validPage = true;
                    }
                }
                if (!validPage) break;
            }
            return allNews;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            MedscapeService medscape = new MedscapeService();
            Console.WriteLine(DateTime.Now);
            var result = medscape.GetAllNewsAsync(new Url("https://www.medscape.com/index/list_13470_0")).Result;
        }
    }
}
