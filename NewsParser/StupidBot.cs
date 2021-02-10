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

        private string message = "";

        public StupidBot()
        {

        }

        public void SendMessage(string text)
        {
            string message = "https://api.telegram.org/bot" + token + "/sendMessage?chat_id=@" + UsernameChannel + "&text=" + text;

            HttpClient client = new HttpClient();
            client.GetAsync(message);
        }
        
    }
}
https://api.telegram.org/bot1656601649:AAG1d5BQ8GCdkt5QP6PRKxriPFSewixNKFQ/sendMessage?chat_id=@newstestbot1&text=" + text;
