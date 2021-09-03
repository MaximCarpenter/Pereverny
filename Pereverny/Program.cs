using System;
using System.IO;
using System.Net;
using System.Threading;

namespace Pereverny
{
    class Program
    {
        private static string _host = "3september.ru";
        private static volatile bool _break = false;
        private static volatile int _counter = 0;
        static void Main(string[] args)
        {
            Console.WriteLine("Auto-prerevorachivatel calendarya na " + _host);
            var thread1 = new Thread(DoWork);
            thread1.Start();
            Console.ReadLine();
            _break = true;
            thread1.Join();
            Console.WriteLine("Total requests: {0} were added to " + _host, _counter);
            Console.ReadLine();
        }

        public static void DoWork()
        {
            while (!_break)
            {
                _counter++;
                Console.WriteLine("Perevorachivayu. Response: {0}", Do());
                Thread.Sleep(100);
            }
        }

        private static string Do()
        {
            try
            {
                var httpWebRequest = (HttpWebRequest) WebRequest.Create("https://" + _host + "/update/");
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Headers.Add("Origin", "https://" + _host);
                httpWebRequest.Headers.Add("Sec-Fetch-Site", "same-origin");
                httpWebRequest.Headers.Add("Sec-Fetch-Mode", "cors");
                httpWebRequest.Headers.Add("Sec-Fetch-Dest", "empty");
                httpWebRequest.Headers.Add("Accept-Encoding", "gzip, deflate, br");
                httpWebRequest.Host = "3september.ru";
                httpWebRequest.Method = "POST";

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    var json = "{\"data\":\"counter\"}";
                    streamWriter.Write(json);
                }

                var httpResponse = (HttpWebResponse) httpWebRequest.GetResponse();
                if (httpResponse.StatusCode == HttpStatusCode.OK)
                    return "3 Sentyabrya";
                return httpResponse.StatusCode.ToString();
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
    }
}
