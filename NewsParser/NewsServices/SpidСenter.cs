using AngleSharp;
using AngleSharp.Io;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace ParserNews.NewsServices
{
    public class SpidСenter : NewsService
    {
        protected override string BaseUrl => "https://spid.center/ru/news";

        public SpidСenter()
        {
            Name = "spid.center";
        }
        public override async Task<IEnumerable<News>> GetAllNewsAsync()
        {
            allNews.Clear();
            try
            {
                var documentRequest = DocumentRequest.Get(new Url(BaseUrl));
                var document = await context.OpenAsync(documentRequest);

                var h3Tags = document.QuerySelectorAll("h3");
                foreach (var h3 in h3Tags) 
                {
                    if (h3.TextContent == "Сегодня")
                    {
                        var aTags = h3.ParentElement.QuerySelectorAll("li h4 a");
                        foreach (var a in aTags)
                        {
                            var link = "https://spid.center" + a.GetAttribute("href");
                            var request = DocumentRequest.Get(new Url(link));
                            var doc = await context.OpenAsync(request);
                            var title = doc.QuerySelector("h1").TextContent;
                            var teaser = "";
                            var pTags = doc.QuerySelectorAll("p");
                            foreach (var p in pTags)
                            {
                                if (teaser.Length < 200) teaser += p.TextContent.Trim();
                            }
                            teaser.Trim();
                            allNews.Add(new News(title, teaser, link));
                        }
                    }
                }

            }
            catch(Exception e)
            {
                Console.WriteLine($"{Name} {e.Message}");
                return new List<News>();
            }
            return allNews;
        }
    }
}
