using AngleSharp;
using AngleSharp.Io;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ParserNews.NewsServices
{
    class vademec : NewsService
    {
        protected override string BaseUrl => "https://vademec.ru/news";
        public vademec()
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
            var middle_bg = document.QuerySelectorAll("div[class='b-news-middle-bg__wrap animate-3d__wrap']");
            foreach (var item in middle_bg)
            {
                var title = item.QuerySelector("div.b-news-middle-bg__title");
                Console.WriteLine(title.TextContent);
            }
            Console.ReadKey();
            return allNews;
        }
    }
}
