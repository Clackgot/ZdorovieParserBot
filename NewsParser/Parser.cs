using ParserNews.NewsServices;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ParserNews
{
    public static class ParserFactory
    {
        public static Parser Parser()
        {
            return new Parser(new List<INewsService>() {
                //new Medicalnewstoday(),
                //new Medscape(),
                //new NewsMedical(),
                //new Bmj(),
                //new Nplus1(),
                //new Reuters(),
                //new Takiedela(),
                //new SpidСenter(),
                //new Medicalxpress(),
                //new Nature(),
                //new Sciencemag()
            });
        }
        public static Parser Test()
        {
            return new Parser(new List<INewsService>() {
                new Nature()
            });
        }

    }
    public class Parser
    {
        private List<INewsService> newsServices = new List<INewsService>();
        public List<News> News { get; private set; } = new List<News>();
        public Parser(List<INewsService> services)
        {
            newsServices = services;
        }
        public async Task Parse()
        {
            List<Task<IEnumerable<News>>> allnews = new List<Task<IEnumerable<News>>>();
            foreach (var service in newsServices)
            {
                allnews.Add(service.GetAllNewsAsync());
            }
            await Task.WhenAll(allnews.ToArray());

            //Console.Clear();
            foreach (var service in newsServices)
            {
                var serv = (NewsService)service;
            }
            foreach (var item in allnews)
            {
                News.AddRange(item.Result);
            }
        }
    }
}
