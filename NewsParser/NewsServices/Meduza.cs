using AngleSharp;
using AngleSharp.Io;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParserNews.NewsServices
{
    class Meduza : NewsService
    {
        protected override string BaseUrl => "https://meduza.io";
        public Meduza()
        {
            Name = "meduza.io";
        }

        private bool IsCorrectDate(string date)
        {
            return true;
        }

        public override async Task<IEnumerable<News>> GetAllNewsAsync()
        {
            allNews.Clear();
            var documentRequest = DocumentRequest.Get(new Url("https://meduza.io/specials/coronavirus"));
            var document = await context.OpenAsync(documentRequest);
            if (document.StatusCode != System.Net.HttpStatusCode.OK)
            {
                Console.WriteLine($"{Name} {document.StatusCode}");
                return new List<News>();
            }
            //var tags = document.QuerySelectorAll("div.RichBlock-content, div.TopicBlock-featured, ul.TopicBlock-list li");
           // Console.WriteLine(tags.ToHtml());

            var tags = document.QuerySelectorAll("div.RichBlock-contentWrap div.RichBlock-content");

            foreach (var item in tags)
            {
                //Console.WriteLine(item.ToHtml());
                var title = item.QuerySelector("a").TextContent.Trim();
                Console.WriteLine(" * " + title);



                //Console.WriteLine(item.ToHtml());
                var date = item.QuerySelector("time").TextContent;
                if (IsCorrectDate(date))
                {
                    //var title = item.QuerySelector("a").TextContent.Trim();
                    var link = item.QuerySelector("a").GetAttribute("href");

                    //var teaser = tag.QuerySelector("p").TextContent.Trim();

                    var documentSite = DocumentRequest.Get(new Url(BaseUrl + link));
                    var doc = await context.OpenAsync(documentSite);

                    var teasers = doc.QuerySelectorAll($"figcaption div, p");
                    if (teasers == null) continue;

                    string teaser = "";

                    foreach (var it in teasers)
                    {
                        if (teaser.Length < 200)
                        {
                            if (!it.TextContent.Contains("/"))
                            {
                                teaser += it.TextContent.Trim() + " ";
                            }
                        }
                        else break;
                    }
                    if (!string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(teaser) && !string.IsNullOrEmpty(link))
                    {
                        //Console.WriteLine(date + ": " + title + "\n" + BaseUrl + link + "\n" + /*teaser +*/ "\n");
                        //allNews.Add(new News(title, teaser, BaseUrl + link));
                    }
                    Console.ReadKey();
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
