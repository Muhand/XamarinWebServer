//using System;
//using System.Diagnostics;
//using System.IO;
//using System.Net;
//using System.Text;
//using System.Web;


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Web;

namespace XamarinWebServer
{
    #region SERVER 1
    public sealed class WebServer : IDisposable
    {
        #region Events
        public event EventHandler ServerStarted;
        #endregion

        #region Global Variables
        private string rootPath;
        private const int bufferSize = 1024 * 512; //512KB
        private HttpListener http;
        #endregion

        #region Constructor(s)
        public WebServer(string rootPath)
        {
            init(rootPath);
        }
        #endregion

        #region Initializer(s)

        private void init(string rootPath)
        {
            Debug.WriteLine("Starting webserver");
            this.rootPath = rootPath;
            http = new HttpListener();
            http.Prefixes.Add("http://*:1235/");
            http.Start();
            http.BeginGetContext(requestWait, null);
        }

        #endregion

        #region StartServer

        public void requestWait(IAsyncResult ar)
        {
            if (!http.IsListening)
                return;
            var c = http.EndGetContext(ar);
            http.BeginGetContext(requestWait, null);

            var url = tuneUrl(c.Request.RawUrl);

            var fullPath = string.IsNullOrEmpty(url) ? rootPath : Path.Combine(rootPath, url);

            Debug.WriteLine(fullPath);

            if (Directory.Exists(fullPath))
                returnDirContents(c, fullPath);
            else if (File.Exists(fullPath))
                returnFile(c, fullPath);
            else
                return404(c);
        }

        private void returnDirContents(HttpListenerContext context, string dirPath)
        {

            context.Response.ContentType = "text/html";
            context.Response.ContentEncoding = Encoding.UTF8;
            using (var sw = new StreamWriter(context.Response.OutputStream))
            {
                sw.WriteLine("<html>");
                sw.WriteLine("<head><meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\"></head>");
                sw.WriteLine("<body><ul>");

                var dirs = Directory.GetDirectories(dirPath);
                foreach (var d in dirs)
                {
                    var link = d.Replace(rootPath, "").Replace('\\', '/');
                    sw.WriteLine("<li>&lt;DIR&gt; <a href=\"" + link + "\">" + Path.GetFileName(d) + "</a></li>");
                }

                var files = Directory.GetFiles(dirPath);
                foreach (var f in files)
                {
                    var link = f.Replace(rootPath, "").Replace('\\', '/');
                    sw.WriteLine("<li><a href=\"" + link + "\">" + Path.GetFileName(f) + "</a></li>");
                }

                sw.WriteLine("</ul></body></html>");
            }
            context.Response.OutputStream.Close();
        }

        private static void returnFile(HttpListenerContext context, string filePath)
        {
            context.Response.ContentType = getcontentType(Path.GetExtension(filePath));
            var buffer = new byte[bufferSize];
            using (var fs = File.OpenRead(filePath))
            {
                context.Response.ContentLength64 = fs.Length;
                int read;
                while ((read = fs.Read(buffer, 0, buffer.Length)) > 0)
                {
                    try
                    {

                        context.Response.OutputStream.Write(buffer, 0, read);
                    } 
                    catch(HttpListenerException ex)
                    {
                        Debug.WriteLine(ex.StackTrace);
                    }
                 }
            }

            context.Response.OutputStream.Close();
        }

        private static void return404(HttpListenerContext context)
        {
            context.Response.StatusCode = 404;
            context.Response.Close();
        }

        private static string tuneUrl(string url)
        {
            url = url.Replace('/', '\\');
            url = HttpUtility.UrlDecode(url, Encoding.UTF8);
            url = url.Substring(1);
            return url;
        }

        private static string getcontentType(string extension)
        {
            switch (extension)
            {
                case ".avi": return "video/x-msvideo";
                case ".css": return "text/css";
                case ".doc": return "application/msword";
                case ".gif": return "image/gif";
                case ".htm":
                case ".html": return "text/html";
                case ".jpg":
                case ".jpeg": return "image/jpeg";
                case ".js": return "application/x-javascript";
                case ".mp3": return "audio/mpeg";
                case ".m4a": return "audio/m4a";
                case ".png": return "image/png";
                case ".pdf": return "application/pdf";
                case ".ppt": return "application/vnd.ms-powerpoint";
                case ".zip": return "application/zip";
                case ".txt": return "text/plain";
                default: return "application/octet-stream";
            }
        }

        //public void Start()
        //{
        //    try
        //    {
        //        this.listener = new HttpListener();
        //        this.listener.Prefixes.Add("http://192.168.1.156:1300/webserver/");
        //        this.listener.Start();

        //        while (true)
        //        {
        //            Debug.WriteLine("Waiting...");
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
        //            Debug.WriteLine("Message sent...");
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine(ex.StackTrace);
        //    }
        //}

        #endregion

        #region Cleanup
        public void Dispose()
        {
            this.http.Stop();
        }
        #endregion
    }
    #endregion 

    #region NEW SERVER
    //public class WebServer
    //{
    //    private readonly string[] _indexFiles = {
    //    "index.html",
    //    "index.htm",
    //    "default.html",
    //    "default.htm"
    //};

    //    private static IDictionary<string, string> _mimeTypeMappings = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase) {
    //    #region extension to MIME type list
    //    {".asf", "video/x-ms-asf"},
    //    {".asx", "video/x-ms-asf"},
    //    {".avi", "video/x-msvideo"},
    //    {".bin", "application/octet-stream"},
    //    {".cco", "application/x-cocoa"},
    //    {".crt", "application/x-x509-ca-cert"},
    //    {".css", "text/css"},
    //    {".deb", "application/octet-stream"},
    //    {".der", "application/x-x509-ca-cert"},
    //    {".dll", "application/octet-stream"},
    //    {".dmg", "application/octet-stream"},
    //    {".ear", "application/java-archive"},
    //    {".eot", "application/octet-stream"},
    //    {".exe", "application/octet-stream"},
    //    {".flv", "video/x-flv"},
    //    {".gif", "image/gif"},
    //    {".hqx", "application/mac-binhex40"},
    //    {".htc", "text/x-component"},
    //    {".htm", "text/html"},
    //    {".html", "text/html"},
    //    {".ico", "image/x-icon"},
    //    {".img", "application/octet-stream"},
    //    {".iso", "application/octet-stream"},
    //    {".jar", "application/java-archive"},
    //    {".jardiff", "application/x-java-archive-diff"},
    //    {".jng", "image/x-jng"},
    //    {".jnlp", "application/x-java-jnlp-file"},
    //    {".jpeg", "image/jpeg"},
    //    {".jpg", "image/jpeg"},
    //    {".js", "application/x-javascript"},
    //    {".mml", "text/mathml"},
    //    {".mng", "video/x-mng"},
    //    {".mov", "video/quicktime"},
    //    {".mp3", "audio/mpeg"},
    //    {".mpeg", "video/mpeg"},
    //    {".mpg", "video/mpeg"},
    //    {".msi", "application/octet-stream"},
    //    {".msm", "application/octet-stream"},
    //    {".msp", "application/octet-stream"},
    //    {".pdb", "application/x-pilot"},
    //    {".pdf", "application/pdf"},
    //    {".pem", "application/x-x509-ca-cert"},
    //    {".pl", "application/x-perl"},
    //    {".pm", "application/x-perl"},
    //    {".png", "image/png"},
    //    {".prc", "application/x-pilot"},
    //    {".ra", "audio/x-realaudio"},
    //    {".rar", "application/x-rar-compressed"},
    //    {".rpm", "application/x-redhat-package-manager"},
    //    {".rss", "text/xml"},
    //    {".run", "application/x-makeself"},
    //    {".sea", "application/x-sea"},
    //    {".shtml", "text/html"},
    //    {".sit", "application/x-stuffit"},
    //    {".swf", "application/x-shockwave-flash"},
    //    {".tcl", "application/x-tcl"},
    //    {".tk", "application/x-tcl"},
    //    {".txt", "text/plain"},
    //    {".war", "application/java-archive"},
    //    {".wbmp", "image/vnd.wap.wbmp"},
    //    {".wmv", "video/x-ms-wmv"},
    //    {".xml", "text/xml"},
    //    {".xpi", "application/x-xpinstall"},
    //    {".zip", "application/zip"},
    //    #endregion
    //};
    //    private Thread _serverThread;
    //    private string _rootDirectory;
    //    private HttpListener _listener;
    //    private int _port;

    //    public int Port
    //    {
    //        get { return _port; }
    //        private set { }
    //    }

    //    /// <summary>
    //    /// Construct server with given port.
    //    /// </summary>
    //    /// <param name="path">Directory path to serve.</param>
    //    /// <param name="port">Port of the server.</param>
    //    public WebServer(string path, int port)
    //    {
    //        this.Initialize(path, port);
    //    }

    //    /// <summary>
    //    /// Construct server with suitable port.
    //    /// </summary>
    //    /// <param name="path">Directory path to serve.</param>
    //    public WebServer(string path)
    //    {
    //        //get an empty port
    //        TcpListener l = new TcpListener(IPAddress.Loopback, 0);
    //        l.Start();
    //        int port = ((IPEndPoint)l.LocalEndpoint).Port;
    //        l.Stop();
    //        this.Initialize(path, port);
    //    }

    //    /// <summary>
    //    /// Stop server and dispose all functions.
    //    /// </summary>
    //    public void Stop()
    //    {
    //        _serverThread.Abort();
    //        _listener.Stop();
    //    }

    //    private void Listen()
    //    {
    //        _listener = new HttpListener();
    //        _listener.Prefixes.Add("http://*:" + _port.ToString() + "/");
    //        _listener.Start();
    //        while (true)
    //        {
    //            try
    //            {
    //                HttpListenerContext context = _listener.GetContext();
    //                Process(context);
    //            }
    //            catch (Exception ex)
    //            {

    //            }
    //        }
    //    }

    //    private void Process(HttpListenerContext context)
    //    {
    //        string filename = context.Request.Url.AbsolutePath;
    //        Console.WriteLine(filename);
    //        filename = filename.Substring(1);

    //        if (string.IsNullOrEmpty(filename))
    //        {
    //            foreach (string indexFile in _indexFiles)
    //            {
    //                if (File.Exists(Path.Combine(_rootDirectory, indexFile)))
    //                {
    //                    filename = indexFile;
    //                    break;
    //                }
    //            }
    //        }

    //        filename = Path.Combine(_rootDirectory, filename);

    //        if (File.Exists(filename))
    //        {
    //            try
    //            {
    //                Stream input = new FileStream(filename, FileMode.Open);

    //                //Adding permanent http response headers
    //                string mime;
    //                context.Response.ContentType = _mimeTypeMappings.TryGetValue(Path.GetExtension(filename), out mime) ? mime : "application/octet-stream";
    //                context.Response.ContentLength64 = input.Length;
    //                context.Response.AddHeader("Date", DateTime.Now.ToString("r"));
    //                context.Response.AddHeader("Last-Modified", System.IO.File.GetLastWriteTime(filename).ToString("r"));

    //                byte[] buffer = new byte[1024 * 16];
    //                int nbytes;
    //                while ((nbytes = input.Read(buffer, 0, buffer.Length)) > 0)
    //                    context.Response.OutputStream.Write(buffer, 0, nbytes);
    //                input.Close();

    //                context.Response.StatusCode = (int)HttpStatusCode.OK;
    //                context.Response.OutputStream.Flush();
    //            }
    //            catch (Exception ex)
    //            {
    //                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
    //            }

    //        }
    //        else
    //        {
    //            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
    //        }

    //        context.Response.OutputStream.Close();
    //    }

    //    private void Initialize(string path, int port)
    //    {
    //        this._rootDirectory = path;
    //        this._port = port;
    //        _serverThread = new Thread(this.Listen);
    //        _serverThread.Start();
    //    }
    //}

    #endregion
}
