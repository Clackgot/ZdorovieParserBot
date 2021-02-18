﻿using ParserNews.NewsServices;
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
            var updater = new ChannelUpdater(ParserFactory.Test());
            updater.Run();
        }
    }
}
