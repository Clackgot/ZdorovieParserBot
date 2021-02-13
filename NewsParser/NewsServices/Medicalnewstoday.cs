using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Io;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
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
            List<Task<IDocument>> documents = new List<Task<IDocument>>();
            var documentRequest = DocumentRequest.Get(new Url(BaseUrl));
            var result = await context.OpenAsync(documentRequest);
            List<string> latestNewsLinks = new List<string>();
            var latestNews = result.QuerySelector("div[id='LATEST NEWS']");
            var newsLinks = latestNews.QuerySelectorAll("li p a");
            foreach (var link in newsLinks)
            {
                var fullLink = BaseUrl + link.GetAttribute("href");
                latestNewsLinks.Add(fullLink);
                var docRequest = DocumentRequest.Get(new Url(fullLink));
                var doc = context.OpenAsync(docRequest);
                documents.Add(doc);
            }
            Task.WaitAll(documents.ToArray());
            var completeDocuments = documents.ConvertAll(doc => doc.Result);
            Regex regex = new Regex("publishedDate\":\".*?\"", RegexOptions.Compiled | RegexOptions.IgnoreCase);

            foreach (var document in completeDocuments)
            {
                string dateString = regex.Match(document.Source.Text).Value.Split("\":\"")[1].Trim('"');
                if((DateTime.Parse(dateString)) == DateTime.Today)
                {
                    var title = document.QuerySelector("div.css-z468a2 h1").TextContent;
                    var teaser = document.QuerySelector("article div p").TextContent;
                    var url = document.Url;
                    allNews.Add(new News(title, teaser, url));
                }
                
            }
            

            return allNews;
        }
    }
}
