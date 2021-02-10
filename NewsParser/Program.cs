using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Io;
using CodeHollow.FeedReader;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

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

        public const int teaserMaxLength = 100;
        private string cutText(string text)
        {
            var tempText = text;
            var temp = tempText.Substring(0, Math.Min(tempText.Length, teaserMaxLength - 3)) + "...";
            return temp;
        }
        private string teaser;
        public string Teaser { get { return teaser; } set { teaser = cutText(value); } }


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

    public class Medscape : NewsService
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
                    var url = document.Url;
                    allNews.Add(new News(title, teaser, new Url(url)));
                }
                
            }
            

            return allNews;
        }
    }

    public class NewsMedical : NewsService
    {
        protected override string BaseUrl => "https://www.news-medical.net/medical/news?page=";

        private async Task<List<IDocument>> getPages()
        {
            List<IDocument> documents = new List<IDocument>();

            for (int i = 1; i < 5; i++)
            {
                string url = BaseUrl + i.ToString();
                var document = context.OpenAsync(url);

                string date = DateTime.Now.ToString("dd MMM yyyy", CultureInfo.GetCultureInfo("en-us"));
                bool exsistTodayNews = false;
                foreach (var item in document.Result.QuerySelectorAll("span.article-meta-date"))
                {

                    if (item.TextContent.Trim() == date)
                    {
                        exsistTodayNews = true;
                        break;
                    }
                    
                }
                if (exsistTodayNews) documents.Add(await document);
                else return documents;
            }
            return documents;
        }

        public override async Task<IEnumerable<News>> GetAllNewsAsync()
        {
            var pages = await getPages();
            foreach (var page in pages)
            {
                string baseUrl = "https://www.news-medical.net/medical";
                var childNodesCount = page.QuerySelector("div.publishables-list-wrap").ChildElementCount-1;
                for (int i = 1; i < childNodesCount; i+=2)
                {
                    string date = DateTime.Now.ToString("dd MMM yyyy", CultureInfo.GetCultureInfo("en-us"));
                    var tagDate = page.QuerySelector("div.publishables-list-wrap").Children[i].QuerySelector("span.article-meta-date").TextContent.Trim();
                    if(tagDate == date)
                    {
                        var title = page.QuerySelector("div.publishables-list-wrap").Children[i - 1].QuerySelector("h3 a").TextContent.Trim();
                        var teaser = page.QuerySelector("div.publishables-list-wrap").Children[i - 1].QuerySelector("p.hidden-xs").TextContent.Trim();
                        var url = baseUrl + page.QuerySelector("div.publishables-list-wrap").Children[i - 1].QuerySelector("a").GetAttribute("href");
                        allNews.Add(new News(title, teaser, new Url(url)));
                    }
                }
            }
            
            return allNews;
        }
    }


    public class Bmj : NewsService
    {
        protected override string BaseUrl => "http://feeds.bmj.com/bmj/recent";

        public override async Task<IEnumerable<News>> GetAllNewsAsync()
        {
            var page = await context.OpenAsync("http://feeds.bmj.com/bmj/recent");
            var result = await FeedReader.ReadAsync("http://feeds.bmj.com/bmj/recent");
            XName name = XName.Get("title", "http://feeds.bmj.com/bmj/recent");
            for (int i = 0; i < result.Items.Count; i++)
            {
                var title = result.Items.AsEnumerable().ToArray()[i].Title;
                var teaser = result.Items.AsEnumerable().ToArray()[0].Description.Split(@"<div")[0];
                var url = result.Items.AsEnumerable().ToArray()[i].Link;
                allNews.Add(new News(title, teaser, new Url(url)));
            }
            return allNews;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Run();
        }

        private static void Run()
        {
            List<News> AllNews = new List<News>();
            List<INewsService> newsServices = new List<INewsService>();
            newsServices.Add(new Medicalnewstoday());//https://www.medicalnewstoday.com
            newsServices.Add(new Medscape()); //https://www.medscape.com/index/list_13470
            newsServices.Add(new NewsMedical());//https://www.news-medical.net/syndication.axd?news=lifesciences
            newsServices.Add(new Bmj());//

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

            // bot is working...

            StupidBot bot = new StupidBot();

            foreach (var news in AllNews)
            {
                bot.SendMessage(news.Title);
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
