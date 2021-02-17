using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Io;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ParserNews.NewsServices
{
    public class Nplus1 : NewsService
    {
        protected override string BaseUrl => "https://nplus1.ru";
        public Nplus1()
        {
            Name = "nplus1.ru";
        }

        private static DateTime getDate(string date)
        {
            date = "2021-02-12";
            string year = date.Split('-')[0];
            string month = date.Split('-')[1];
            string day = date.Split('-')[2];
            return new DateTime(int.Parse(year), int.Parse(month), int.Parse(day));
        }

        public override async Task<IEnumerable<News>> GetAllNewsAsync()
        {
            IDocument document = null;
            try
            {
                allNews.Clear();
                //var documentRequest = DocumentRequest.Get(new Url("https://nplus1.ru/theme/coronavirus-history"));
                var documentRequest = DocumentRequest.Get(new Url("https://nplus1.ru/rubric/medicine"));
                document = await context.OpenAsync(documentRequest);
                if (document.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    Console.WriteLine($"{Name} {document.StatusCode}");
                    return new List<News>();
                }
                var tags = document.QuerySelectorAll("article[class*='item-news']");
                foreach (var item in tags)
                {
                    var title = item.QuerySelector("a > div.caption > h3").TextContent;
                    var date = item.QuerySelector("a > div.caption > div.date > span").GetAttribute("title");
                    var link = item.QuerySelector("a").GetAttribute("href");

                    var documentSite = DocumentRequest.Get(new Url(BaseUrl + link));
                    var doc = await context.OpenAsync(documentSite);

                    var teasers = doc.QuerySelector($"div[class*='body']");
                    if (teasers == null) continue;
                    string teaser = "";
                    foreach (var it in teasers.Children)
                    {
                        if (!it.ToHtml().Contains("class"))
                        {
                            var tmp = Regex.Replace(it.TextContent, @"[\r\n\t]", " ").Trim();
                            if (tmp.Length > 100)
                            {
                                teaser = tmp; break;
                            }
                        }
                    }

                    //Console.WriteLine($" * {title}\n * {teaser}\n * {BaseUrl + link}\n");
                    if (getDate(date) == DateTime.Today)
                    {
                        allNews.Add(new News(title, teaser, BaseUrl + link));
                    }
                }
                Console.WriteLine($"{Name} {allNews.Count}");
                return allNews;
            }
            catch
            {
                if (document.StatusCode != HttpStatusCode.OK)
                {
                    Console.WriteLine($"{Name} {document.StatusCode}");
                }
                return new List<News>();
            }
        }
    }
}
