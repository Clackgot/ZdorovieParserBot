using AngleSharp;
using AngleSharp.Io;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ParserNews.NewsServices
{
    class takiedela : NewsService
    {
        protected override string BaseUrl => "https://takiedela.ru";
        public takiedela()
        {
            Name = "takiedela.ru";
        }

        private bool IsCorrectDate(string date)
        {
            if (date.Contains(":") || date == "Только что")
                return true;
            else return false; 
        }

        public override async Task<IEnumerable<News>> GetAllNewsAsync()
        {
            allNews.Clear();
            for (int i = 1; ; i++)
            {
                var documentRequest = DocumentRequest.Get(new Url($"https://takiedela.ru/news/page/{i}/"));
                var document = await context.OpenAsync(documentRequest);
                if (document.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    Console.WriteLine($"{Name} {document.StatusCode}");
                    return new List<News>();
                }
                var tags = document.QuerySelectorAll("ul[class='b-news-list b-news-list_inside'] li");
                foreach (var item in tags)
                {
                    var title = item.QuerySelector("a[href]").TextContent.Trim();
                    var date = item.QuerySelector("time").TextContent.Trim();
                    var link = item.QuerySelector("a[href]").GetAttribute("href");


                    var documentSite = DocumentRequest.Get(new Url(link));
                    var doc = await context.OpenAsync(documentSite);

                    var teasers = doc.QuerySelector($"div.b-article__content");
                    if (teasers == null) continue;
                    string teaser = "";

                    foreach (var it in teasers.Children)
                    {
                        //var tmp = Regex.Replace(it.TextContent, @"[\r\n\t]", " ").Trim();

                        if (teaser.Length < 300)
                        {
                            if (!it.ToHtml().Contains("div"))
                            {
                                teaser += it.TextContent.Trim();
                            }
                        }
                        else break;
                    }

                    if (IsCorrectDate(date))
                    {
                        //Console.WriteLine($"{date}: {title}\n{teaser}\n{link}\n");

                        allNews.Add(new News(title, teaser, link));
                    }
                    else
                    {
                        Console.WriteLine($"{Name} {allNews.Count}");
                        return allNews;
                    }
                }
            }
        }
    }
}
