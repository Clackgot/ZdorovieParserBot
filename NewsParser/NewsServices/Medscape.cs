using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Io;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ParserNews
{
    public class Medscape : NewsService
    {
        protected override string BaseUrl => "https://www.medscape.com/index/list_13470_";
        public Medscape()
        {
            Name = "medscape.com";
        }
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
}
