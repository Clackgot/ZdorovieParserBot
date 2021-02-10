using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace ParserNews
{
    class StupidBot
    {
        private string token = "1656601649:AAG1d5BQ8GCdkt5QP6PRKxriPFSewixNKFQ";

        private string UsernameChannel = "newstestbot1";

        public StupidBot(string text)
        {
            HttpClient client = new HttpClient();

            string message = "https://api.telegram.org/bot" + token + "/sendMessage?chat_id=@" + UsernameChannel + "&text=" + text;

            client.GetAsync(message);
        }
        
    }
}
