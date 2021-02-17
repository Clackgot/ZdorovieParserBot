using CodeHollow.FeedReader;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ParserNews.NewsServices
{
    public class Sciencemag : NewsService
    {
        protected override string BaseUrl => "https://www.sciencemag.org/";

        public override async Task<IEnumerable<News>> GetAllNewsAsync()
        {
            Console.WriteLine("Hello");
            string coronavirusLink = BaseUrl + "coronavirus-news-api.xml";
            string immunologyLink = @"https://immunology.sciencemag.org/rss/current.xml";
            string medicineLink = @"https://stm.sciencemag.org/rss/current.xml";

            var coronavirus = await FeedReader.ReadAsync(coronavirusLink);
            var immunology = await FeedReader.GetFeedUrlsFromUrlAsync(immunologyLink);
            var medicine = await FeedReader.GetFeedUrlsFromUrlAsync(medicineLink);

            Regex regex = new Regex(@"(, ).*?( -)");


            foreach (var item in coronavirus.Items)
            {
                //Console.WriteLine(new News(item.Title.Trim(), item.Description.Trim(), item.Link));
                string date = item.PublishingDateString;
                date = regex.Match(date).Value;
                date = date.Substring(2, date.Length - 4);
                Console.WriteLine(DateTime.Parse(date));
            }

            

            return allNews;
        }
    }
}
