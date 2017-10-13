using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using XamarinWebServer;

namespace XamarinWebServerSimpleTest
{
    class Program
    {
        #region Global Variables
        static HttpListener listener;
        #endregion

        static void Main(string[] args)
        {
            //XamarinWebServer.WebServer server = new WebServer();
            //server.Start();
            //Start();
            StartWeb();
            //Console.ReadLine();
            Process.GetCurrentProcess().WaitForExit();
        }

        private static void StartWeb()
        {
            XamarinWebServer.WebServer se = new WebServer("hostServer");
            //string myFolder = "hostServer";
            //XamarinWebServer.WebServer myServer = new WebServer(myFolder, 1235);
            //Console.WriteLine("Server is running on this port: " + myServer.Port.ToString());
        }

        //private static void Start()
        //{
        //    try
        //    {
        //        listener = new HttpListener();
        //        listener.Prefixes.Add("http://localhost:1300/webserver/");
        //        listener.Start();

        //        while (true)
        //        {
        //            Debug.WriteLine("Waiting...");
        //            Console.WriteLine("Waiting...");
        //            HttpListenerContext context = listener.GetContext();
        //            string msg = "Hello";
        //            context.Response.ContentLength64 = Encoding.UTF8.GetByteCount(msg);
        //            context.Response.StatusCode = (int)HttpStatusCode.OK;

        //            using (Stream stream = context.Response.OutputStream)
        //            {
        //                using (StreamWriter writer = new StreamWriter(stream))
        //                {
        //                    writer.Write(msg);
        //                }
        //            }
        //            Console.WriteLine("Message sent...");
        //            Debug.WriteLine("Message sent...");
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine(ex.StackTrace);
        //    }
        //}
    }
}
