using AngleSharp;
using AngleSharp.Io;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ParserNews.NewsServices
{
    class Vademec : NewsService
    {
        protected override string BaseUrl => "https://vademec.ru";
        public Vademec()
        {
            Name = "vademec.ru";
        }


        public override async Task<IEnumerable<News>> GetAllNewsAsync()
        {
            allNews.Clear();
            var documentRequest = DocumentRequest.Get(new Url($"https://vademec.ru/news/"));
            var document = await context.OpenAsync(documentRequest);
            if (document.StatusCode != System.Net.HttpStatusCode.OK)
            {
                Console.WriteLine($"{Name} {document.StatusCode}");
                return new List<News>();
            }
            var middle_bg = document.QuerySelectorAll("div.container div.masonry-layout div.masonry-layout__item");
            foreach (var item in middle_bg)
            {
                var link = BaseUrl + item.QuerySelector("a").GetAttribute("href");
                //var title = item.QuerySelector("div div a").GetAttribute("href");
                //var date = item.QuerySelector("div[class*='animate-3d__wrap'] span.b-date__date").TextContent;

                var documentSite = DocumentRequest.Get(new Url(link));
                var doc = await context.OpenAsync(documentSite);

                var teasers = doc.QuerySelector($"div.content div[class*='page-wrap__center'] div[class*='d-news__content']");
                if (teasers == null) continue;
                string teaser = "";

                foreach (var it in teasers.Children)
                {
                    //var tmp = Regex.Replace(it.TextContent, @"[\r\n\t]", " ").Trim();

                    if (teaser.Length < 200)
                    {
                        if (it.ToHtml().Contains("<p")&&!it.ToHtml().Contains("script"))
                        {
                            teaser += it.TextContent.Trim();
                        }
                    }
                    else break;
                }

                Console.WriteLine(teaser);
                Console.WriteLine(link + "\n");
            }
            Console.ReadKey();
            return allNews;
        }
    }
}
