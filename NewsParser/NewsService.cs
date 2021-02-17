using AngleSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ParserNews
{
    /// <summary>
    /// Модель новости
    /// </summary>
    public class News
    {
        public News(string title, string teaser, string url)
        {
            Title = title;
            Teaser = teaser;
            Url = url;
        }
        public News()
        {

        }
        public string Title { get; set; }

        public const int teaserMaxLength = 200;
        private string cutText(string text)
        {
            var tempText = text;
            var temp = tempText.Substring(0, Math.Min(tempText.Length, teaserMaxLength));
            return temp;
        }
        
        private string teaser;
        public string Teaser { get { return teaser; } set { teaser = cutText(value); } }

        private string url;

        public string Url
        {
            get { return url; }
            set { url = value; }
        }



        public override string ToString()
        {
            return "Title: " + Title + "\n"
                + "Teaser: " + Teaser + "\n" +
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
        public string Name { get; protected set; }
        public int NewsCount
        {
            get { return allNews.Count; }
        }


        protected List<News> allNews = new List<News>();
        protected static int getMonthNumber(string monthName)
        {
            return DateTime.ParseExact(monthName, "MMMM", CultureInfo.GetCultureInfo("en-us")).Month;
        }
        protected IBrowsingContext context;
        public NewsService()
        {

            var handler = new HttpClientHandler()
            {
                //Proxy = new WebProxy("96.96.123.154:80", false),
                PreAuthenticate = false,
                UseDefaultCredentials = false,
                MaxConnectionsPerServer = 1,
                UseCookies = true,
            };
            var config = Configuration.Default
                .WithDefaultCookies()
                .WithDefaultLoader()
                .WithRequesters(handler);//Использовать стандартный загрузчик и использовать куки
            context = BrowsingContext.New(config);//Инициализация контекста отправки запросов(а-ля сессия)
        }
        public abstract Task<IEnumerable<News>> GetAllNewsAsync();
    }


}
