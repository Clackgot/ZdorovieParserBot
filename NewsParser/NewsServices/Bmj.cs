using AngleSharp;
using CodeHollow.FeedReader;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ParserNews
{
    public class Bmj : NewsService
    {
        protected override string BaseUrl => "http://feeds.bmj.com/bmj/recent";
        public Bmj()
        {
            Name = "bmj.com";
        }
        public override async Task<IEnumerable<News>> GetAllNewsAsync()
        {
            var page = await context.OpenAsync("http://feeds.bmj.com/bmj/recent");
            var result = await FeedReader.ReadAsync("http://feeds.bmj.com/bmj/recent");
            for (int i = 0; i < result.Items.Count; i++)
            {
                var title = result.Items.AsEnumerable().ToArray()[i].Title;
                var teaser = result.Items.AsEnumerable().ToArray()[0].Description.Split(@"<div")[0];
                var url = result.Items.AsEnumerable().ToArray()[i].Link;
                allNews.Add(new News(title, teaser, url));
            }
            return allNews;
        }
    }
}
