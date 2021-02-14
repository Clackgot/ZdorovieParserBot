using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Io;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ParserNews
{
    public class Medicalnewstoday : NewsService
    {
        protected override string BaseUrl => "https://www.medicalnewstoday.com";
        public Medicalnewstoday()
        {
            Name = "medicalnewstoday.com";
        }
        public override async Task<IEnumerable<News>> GetAllNewsAsync()
        {
            allNews.Clear();

            var handler = new HttpClientHandler()
            {
                //Proxy = new WebProxy("96.96.123.154:80", false),
                PreAuthenticate = false,
                UseDefaultCredentials = false,
                MaxConnectionsPerServer = 1,
                UseCookies = false,
            };
            var config = Configuration.Default
    .WithDefaultLoader().WithRequesters(handler);//Использовать стандартный загрузчик и использовать куки
            context = BrowsingContext.New(config);//Инициализация контекста отправки запросов(а-ля сессия)
            List<IDocument> documents = new List<IDocument>();
            var documentRequest = DocumentRequest.Get(new Url(BaseUrl));
            var result = await context.OpenAsync(documentRequest);
            if (result != null)
            if(result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                List<string> latestNewsLinks = new List<string>();
                var latestNews = result.QuerySelector("div[id='LATEST NEWS']");
                var newsLinks = latestNews.QuerySelectorAll("li p a");
                foreach (var link in newsLinks)
                {
                    var fullLink = BaseUrl + link.GetAttribute("href");
                    latestNewsLinks.Add(fullLink);
                    var docRequest = DocumentRequest.Get(new Url(fullLink));
                    var doc = context.OpenAsync(docRequest).Result;
                    if (doc.StatusCode == HttpStatusCode.OK) documents.Add(doc);
                    else
                    {
                        Console.WriteLine($"{Name} {result.StatusCode}");
                        return new List<News>();
                    }
                }
                Regex regex = new Regex("publishedDate\":\".*?\"", RegexOptions.Compiled | RegexOptions.IgnoreCase);

                foreach (var document in documents)
                {
                    string dateString = regex.Match(document.Source.Text).Value.Split("\":\"")[1].Trim('"');
                    if ((DateTime.Parse(dateString)) == DateTime.Today)
                    {
                        var title = document.QuerySelector("div.css-z468a2 h1").TextContent;
                        var teaser = document.QuerySelector("article div p").TextContent;
                        var url = document.Url;
                        allNews.Add(new News(title, teaser, url));
                    }

                }
                Console.WriteLine($"{Name} {allNews.Count}");
                
            }
            else
            {
                Console.WriteLine($"{Name} {result.StatusCode}");
            }
            return allNews;
        }
    }
}
