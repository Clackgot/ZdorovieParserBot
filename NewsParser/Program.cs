using ParserNews.NewsServices;
using System.Collections.Generic;
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
    }
}
