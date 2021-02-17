using ParserNews.NewsServices;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Telegram.Bot.Types.Enums;

namespace ParserNews
{

    class Program
    {
        static void Main(string[] args)
        {
            var updater = new ChannelUpdater(ParserFactory.Parser());
            updater.Run();
        }

        private static void medicalnewstoday()
        {
            Medicalnewstoday medicalnewstoday = new Medicalnewstoday();
            var result = medicalnewstoday.GetAllNewsAsync().Result;
            foreach (var news in result)
            {
                Console.WriteLine(news);
            }
        }
        private static void medscape()
        {
            MedscapeService medscape = new MedscapeService();
            var result = medscape.GetAllNewsAsync().Result;
            foreach (var news in result)
            {
                Console.WriteLine(news);
            }
        }
    }
}
