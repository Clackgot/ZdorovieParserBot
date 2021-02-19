using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Io;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ParserNews.NewsServices
{
    class Takiedela : NewsService
    {
        protected override string BaseUrl => "https://takiedela.ru";
        public Takiedela()
        {
            Name = "takiedela.ru";
        }

        private DateTime CorrectDate(string date)
        {
            string day = date.Split(". ")[0];
            string month = date.Split(". ")[1];
            string year = date.Split(". ")[2];
            return new DateTime(int.Parse(year), int.Parse(month), int.Parse(day));
        }

        private bool IsCorrectDate(string date)
        {
            if (date.Contains(":") || date == "Только что") return true;
            else if (CorrectDate(date) == DateTime.Today) return true;
            else return false;
        }

        public override async Task<IEnumerable<News>> GetAllNewsAsync()
        {
            allNews.Clear();
            var documentRequest = DocumentRequest.Get(new Url($"https://takiedela.ru/plot/koronavirus-covid-19/"));
            var document = await context.OpenAsync(documentRequest);
            if (document.StatusCode != System.Net.HttpStatusCode.OK)
            {
                Console.WriteLine($"{Name} {document.StatusCode}");
                return new List<News>();
            }
            var tags = document.QuerySelectorAll("ul.b-plot__list li.b-plot__list__elem");
            foreach (var item in tags)
            {
                var title = item.QuerySelector("div.b-plot__list__elem__txt > a").TextContent.Trim();
                var date = item.QuerySelector("div.b-plot__list__elem__date").TextContent.Trim();
                var link = item.QuerySelector("div.b-plot__list__elem__txt a.b-plot__list__elem__txt__lnk").GetAttribute("href");

                var documentSite = DocumentRequest.Get(new Url(link));
                var doc = await context.OpenAsync(documentSite);

                //var teasers = doc.QuerySelector("div.b-article__content, div.b-single__content-720");
                var teasers = doc.QuerySelectorAll("div[data-io-article-url] p");
                if (teasers == null) continue;
                string teaser = "";

                foreach (var it in teasers)
                {
                    if (teaser.Length < 300) teaser += it.TextContent.Trim();
                    else break;
                }


                if (IsCorrectDate(date))
                {
                    //Console.WriteLine($"{date}: {title}\n{teaser}\n{link}\n\n\n");
                    allNews.Add(new News(title, teaser, link));
                }
                else
                {
                    Console.WriteLine($"{Name} {allNews.Count}");
                    return allNews;
                }
            }
            return new List<News>();
        }
    }
}
