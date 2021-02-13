using AngleSharp;
using AngleSharp.Io;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ParserNews.NewsServices
{
    class Reuters : NewsService
    {
        protected override string BaseUrl => "https://www.reuters.com";
        public Reuters()
        {
            Name = "reuters.com";
        }

        private bool IsCorrectDate(string date)
        {
            bool IsBool = (date.Contains("EST") || (DateTime.Now.AddDays(-1).ToShortDateString() == DateTime.Parse(date).ToShortDateString()));
            return (IsBool) ? true : false;
        }

        public override async Task<IEnumerable<News>> GetAllNewsAsync()
        {
            allNews.Clear();
            for (int i = 1; ; i++)
            {
                var documentRequest = DocumentRequest.Get(new Url($"https://www.reuters.com/news/archive/healthnews?view=page&page={i}&pageSize=10"));
                var document = await context.OpenAsync(documentRequest);
                if(document.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    Console.WriteLine($"{Name} {document.StatusCode}");
                    return new List<News>();
                }
                var tags = document.QuerySelectorAll("div[class*='column1'] article.story > div.story-content");
                foreach (var tag in tags)
                {
                    var title = tag.QuerySelector("a[href] > h3.story-title").TextContent.Trim();
                    var teaser = tag.QuerySelector("p").TextContent.Trim();
                    var link = tag.QuerySelector("a").GetAttribute("href");
                    var date = tag.QuerySelector("time.article-time span.timestamp").TextContent;

                    //Console.WriteLine(" * " + title);
                    //Console.WriteLine(" * " + teaser);
                    //Console.WriteLine(" * " + date);
                    //Console.WriteLine(" * " + BaseUrl + link + "\n");

                    if (IsCorrectDate(date))
                    {
                        //Console.WriteLine($"{date}: {title}\n{BaseUrl+link}\n{teaser}\n");
                        allNews.Add(new News(title, teaser, BaseUrl + link));
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
