using CodeHollow.FeedReader;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ParserNews.NewsServices
{
    public class Sciencemag : NewsService
    {
        protected override string BaseUrl => "https://www.sciencemag.org/";

        public Sciencemag()
        {
            Name = "sciencemag.org";
        }

        private static string normalizeText(string text)
        {
            var temp = text;
            temp = temp.Replace("<p>", "");
            temp = temp.Replace("</p>", "");
            return temp;
        }
        public override async Task<IEnumerable<News>> GetAllNewsAsync()
        {
            allNews.Clear();
            int publishedNewsCount = 0;
            string coronavirusLink = BaseUrl + "coronavirus-news-api.xml";
            string immunologyLink = @"https://immunology.sciencemag.org/rss/current.xml";
            string medicineLink = @"https://stm.sciencemag.org/rss/current.xml";

            try
            {
                var coronavirus = await FeedReader.ReadAsync(coronavirusLink);
                var immunology = await FeedReader.ReadAsync(immunologyLink);
                var medicine = await FeedReader.ReadAsync(medicineLink);

                Regex regex = new Regex(@"(, ).*?( -)");

                foreach (var item in coronavirus.Items)
                {
                    string date = item.PublishingDateString;
                    date = regex.Match(date).Value;
                    date = date.Substring(2, date.Length - 4);
                    DateTime publishDate = DateTime.Parse(date);
                    var title = item.Title.Trim();
                    title = normalizeText(title);
                    var teaser = item.Description.Trim();
                    teaser = normalizeText(teaser);
                    var news = new News(title, teaser, item.Link);
                    if (publishDate == DateTime.Today)
                    {
                        publishedNewsCount++;
                        allNews.Add(news);
                    }
                }
                foreach (var item in immunology.Items)
                {
                    string date = item.PublishingDateString;
                    DateTime publishDate = DateTime.Parse(date);
                    var title = item.Title.Trim();
                    title = normalizeText(title);
                    var teaser = item.Description.Trim();
                    teaser = normalizeText(teaser);
                    var news = new News(title, teaser, item.Link);
                    if (publishDate == DateTime.Today)
                    {
                        publishedNewsCount++;
                        allNews.Add(news);
                    }
                }
                foreach (var item in medicine.Items)
                {
                    string date = item.PublishingDateString;
                    DateTime publishDate = DateTime.Parse(date);

                    var title = item.Title.Trim();
                    title = normalizeText(title);
                    var teaser = item.Description.Trim();
                    teaser = normalizeText(teaser);
                    var news = new News(title, teaser, item.Link);

                    if (publishDate == DateTime.Today)
                    {
                        publishedNewsCount++;
                        allNews.Add(new News(title, teaser, item.Link));
                    }
                }
            }
            catch(Exception e)
            {
                Console.WriteLine($"{Name} {e.Message}");
                return new List<News>();
            }

            Console.WriteLine($"{Name} {publishedNewsCount}");
            return allNews;
        }
    }
}
