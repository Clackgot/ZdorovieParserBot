using AngleSharp;
using AngleSharp.Io;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ParserNews.NewsServices
{
    class Nature : NewsService
    {
        protected override string BaseUrl => "https://www.nature.com";
        public Nature()
        {
            Name = "nature.com";
        }


        private DateTime CorrectDate(string date)
        {
            string year = date.Split('-')[0];
            string month = date.Split('-')[1];
            string day = date.Split('-')[2];
            return new DateTime(int.Parse(year), int.Parse(month), int.Parse(day));
        }

        public override async Task<IEnumerable<News>> GetAllNewsAsync()
        {
            allNews.Clear();
            for (int i = 1; ; i++)
            {
                var documentRequest = DocumentRequest.Get(new Url($"https://www.nature.com/nature/articles?searchType=journalSearch&sort=PubDate&type=news&page={i}"));
                var document = await context.OpenAsync(documentRequest);
                if (document.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    Console.WriteLine($"{Name} {document.StatusCode}");
                    return new List<News>();
                }
                var tags = document.QuerySelectorAll("div#content ul[class*='grid-auto-fill'] > li");
                foreach (var item in tags)
                {
                    //Console.WriteLine(item.ToHtml());
                    //Console.WriteLine("******************************************************************************");

                    var date = item.QuerySelector("time").GetAttribute("datetime");
                    Console.WriteLine(date);
                    if (CorrectDate(date) == DateTime.Today)
                    {

                        var title = item.QuerySelector("a.text-gray").TextContent.Trim();
                        var link = item.QuerySelector("a.text-gray").GetAttribute("href");


                        var documentSite = DocumentRequest.Get(new Url(BaseUrl + link));
                        var doc = await context.OpenAsync(documentSite);

                        var teasers = doc.QuerySelectorAll($"div.align-left div.cleared > p");
                        if (teasers == null) continue;
                        string teaser = "";

                        foreach (var it in teasers)
                        {
                            if (teaser.Length < 300) teaser += it.TextContent.Trim();
                            else break;
                        }
                        if (title != null && teaser != null && link != null)
                        {
                            allNews.Add(new News(title, teaser, link));
                        }
                        //Console.WriteLine(date + " " + title + "\n" + BaseUrl + link + "\n"+teaser+"\n");
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

