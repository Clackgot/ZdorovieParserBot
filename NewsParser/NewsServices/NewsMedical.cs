using AngleSharp;
using AngleSharp.Dom;
using CodeHollow.FeedReader;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace ParserNews
{
    public class NewsMedical : NewsService
    {
        public NewsMedical()
        {
            Name = "news-medical.net";
        }
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
                        allNews.Add(new News(title, teaser, url));
                    }
                }
            }
            return allNews;
        }
    }
}
