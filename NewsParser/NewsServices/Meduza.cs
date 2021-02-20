using AngleSharp;
using AngleSharp.Io;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Text.RegularExpressions;

namespace ParserNews.NewsServices
{
    class Meduza : NewsService
    {
        protected override string BaseUrl => "https://meduza.io/";
        public Meduza()
        {
            Name = "meduza.io";
        }

        private bool IsCorrectDate(string date)
        {
            return true;
        }

        public override async Task<IEnumerable<News>> GetAllNewsAsync()
        {
            allNews.Clear();
            var documentRequest = DocumentRequest.Get(new Url("https://meduza.io/api/w5/screens/specials/coronavirus"));
            var document = await context.OpenAsync(documentRequest);

            if (document.StatusCode != System.Net.HttpStatusCode.OK)
            {
                Console.WriteLine($"{Name} {document.StatusCode}");
                return new List<News>();
            }
            string stringJson = document.QuerySelector("pre").TextContent;


            var documents = (JContainer)JObject.Parse(stringJson)["documents"];
            var results = documents.Descendants().OfType<JObject>().Where(x => x["url"] != null);

            var teaser = "См. источник ниже";
            foreach (var item in results)
            {
                var datetime = item["datetime"]?.ToString();
                if (UnixToDateTime(datetime))
                {
                    var title = item["title"]?.ToString();
                    var link = BaseUrl + item["url"]?.ToString();
                    //Console.WriteLine($"---: {datetime}\n1.{title}\n2.{link}\n3.{teaser}\n========================================================================================\n");
                    
                    if (!string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(teaser) && !string.IsNullOrEmpty(link))
                    {
                        allNews.Add(new News(title, teaser, link));
                    }
                }

                #region Запаска

                //var link = "https://meduza.io/api/w5/" + item["url"].ToString();

                //var docReq = DocumentRequest.Get(new Url(link));
                //var doc = await context.OpenAsync(docReq);
                //if (doc.StatusCode != HttpStatusCode.OK)
                //{
                //    Console.WriteLine($"{Name} {doc.StatusCode}");
                //    return new List<News>();
                //}

                //string strJson = doc.QuerySelector("pre").TextContent;
                //var root = (JContainer)JObject.Parse(strJson)["root"];
                //var title = root["title"];


                //string teaser = root["og"]["description"]?.ToString();
                //if (teaser == null || teaser.Length < 10)
                //{
                //    var Teasers = root.Descendants().OfType<JObject>().Where(x => x["caption"] != null);
                //    foreach (var it in Teasers)
                //    {
                //        var temp = it["caption"].ToString();
                //        if (temp!=null)
                //        {
                //            Console.WriteLine(temp);
                //        }
                //        //root["content"]["blocks"][$"{}"]
                //    }
                //}

                //Console.WriteLine(" * "+title+"\n + "+teaser+"\n");
                #endregion
                //Console.ReadKey();
            }
            return allNews;
        }


        private bool UnixToDateTime(string date)
        {
            DateTime Date = (new DateTime(1970, 1, 1, 0, 0, 0, 0)).AddSeconds(int.Parse(date));
            if (Date.Date == DateTime.Today) return true;
            else return false;
        }

    }


}