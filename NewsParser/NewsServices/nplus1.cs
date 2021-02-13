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
    public class Nplus1 : NewsService
    {
        protected override string BaseUrl => "https://nplus1.ru/";
        public Nplus1()
        {
            Name = "https://nplus1.ru/";
        }

        private DateTime getDate(string date)
        {
            date = "2021-02-12";
            string year = date.Split('-')[0];
            string month = date.Split('-')[1];
            string day = date.Split('-')[2];
            return new DateTime(int.Parse(year), int.Parse(month), int.Parse(day));
        }

        public override async Task<IEnumerable<News>> GetAllNewsAsync()
        {
            //var documentRequest = DocumentRequest.Get(new Url("https://nplus1.ru/theme/coronavirus-history"));
            var documentRequest = DocumentRequest.Get(new Url("https://nplus1.ru/rubric/medicine"));
            var document = await context.OpenAsync(documentRequest);

            var tags = document.QuerySelectorAll("article[class*='item-news']");
            foreach (var item in tags)
            {
                var title = item.QuerySelector("a > div.caption > h3").TextContent;
                var date = item.QuerySelector("a > div.caption > div.date > span").GetAttribute("title");
                var link = item.QuerySelector("a").GetAttribute("href");
                // var time = item.QuerySelector("a > div.caption > div.date > span").TextContent;

                var documentSite = DocumentRequest.Get(new Url(BaseUrl + link));
                var doc = await context.OpenAsync(documentSite);
                //Console.WriteLine(doc.ToHtml());

                var temp = doc.QuerySelector($"div[class*='body']");
                string teaser = "";
                for (int i = 1; i < 10;)
                {
                    if (string.IsNullOrEmpty(temp.Children[i].TextContent) || temp.Children[i].TextContent.Length < 15)
                    {
                        i++;
                    }
                    else
                    {
                        teaser = temp.Children[i].TextContent; break;
                    }
                }
                teaser = Regex.Replace(teaser, @"[\r\n\t]", "");

                if (getDate(date) == DateTime.Today)
                {
                    allNews.Add(new News(title, teaser, BaseUrl + link));
                }
            }
            return allNews;
        }
    }
}
