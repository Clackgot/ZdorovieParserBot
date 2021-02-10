using AngleSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using System.Xml.Serialization;

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
        public News()
        {

        }
        [XmlAttribute("Title")]
        public string Title { get; set; }

        public const int teaserMaxLength = 100;
        private string cutText(string text)
        {
            var tempText = text;
            var temp = tempText.Substring(0, Math.Min(tempText.Length, teaserMaxLength - 3)) + "...";
            return temp;
        }
        
        private string teaser;
        [XmlAttribute("Teaser")]
        public string Teaser { get { return teaser; } set { teaser = cutText(value); } }

        [XmlIgnore]
        public Url Url { get; set; }
        [XmlAttribute("Url")]
        public string UrlAdapter { get { return this.Url.Href; } }

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

            //switch (monthName)
            //{
            //    case "January":
            //        return 1;
            //    case "February":
            //        return 2;
            //    case "March":
            //        return 3;
            //    case "April":
            //        return 4;
            //    case "May":
            //        return 5;
            //    case "June":
            //        return 6;
            //    case "July":
            //        return 7;
            //    case "August":
            //        return 8;
            //    case "September":
            //        return 9;
            //    case "October":
            //        return 10;
            //    case "November":
            //        return 11;
            //    case "December":
            //        return 12;
            //    default:
            //        return 0;
            //}
        }
        protected IBrowsingContext context;
        public NewsService()
        {
            var config = Configuration.Default
                .WithDefaultCookies()
                .WithDefaultLoader();//Использовать стандартный загрузчик и использовать куки
            context = BrowsingContext.New(config);//Инициализация контекста отправки запросов(а-ля сессия)
        }
        public abstract Task<IEnumerable<News>> GetAllNewsAsync();
    }


}
