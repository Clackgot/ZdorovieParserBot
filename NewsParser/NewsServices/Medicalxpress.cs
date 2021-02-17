using AngleSharp;
using AngleSharp.Io;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ParserNews.NewsServices
{
    public class Medicalxpress : NewsService
    {
        protected override string BaseUrl => "https://medicalxpress.com/latest-news/";
        public Medicalxpress()
        {
            Name = "medicalxpress.com";
        }
        public override async Task<IEnumerable<News>> GetAllNewsAsync()
        {
            allNews.Clear();

            try
            {

            var documentRequest = DocumentRequest.Get(new Url(BaseUrl));
            var document = await context.OpenAsync(documentRequest);
            var articles = document.QuerySelectorAll("article.selection-article");

            foreach (var article in articles)
            {
                string title = article.QuerySelector("h3").TextContent;
                string teaser = article.QuerySelector("p").TextContent;
                string link = article.QuerySelector("a").GetAttribute("href");
                var news = new News(title, teaser, link);
                allNews.Add(news);
            }
            }
            catch(Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{Name} {e.Message}");
                Console.ForegroundColor = ConsoleColor.Gray;
                return new List<News>();
            }
            Console.WriteLine($"{Name} {allNews.Count}");
            return allNews;
        }
    }
}
