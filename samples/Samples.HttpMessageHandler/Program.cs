using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Datadog.Trace;

namespace Samples.HttpMessageHandler
{
    public static class Program
    {
        private const string RequestContent = "PING";
        private const string ResponseContent = "PONG";
        private static readonly Encoding Utf8 = Encoding.UTF8;

        private static string Url;

#if NETFRAMEWORK
        // On .NET Framework, tell the runtime to load assemblies from the GAC domain-neutral.
        // In this sample, this will affect System and System.Net.Http
        [LoaderOptimization(LoaderOptimization.MultiDomainHost)]
#endif
        public static void Main(string[] args)
        {
            bool tracingDisabled = args.Any(arg => arg.Equals("TracingDisabled", StringComparison.OrdinalIgnoreCase));
            Console.WriteLine($"TracingDisabled {tracingDisabled}");

            bool useHttpClient = args.Any(arg => arg.Equals("HttpClient", StringComparison.OrdinalIgnoreCase));
            Console.WriteLine($"HttpClient {useHttpClient}");

            bool useWebClient = args.Any(arg => arg.Equals("WebClient", StringComparison.OrdinalIgnoreCase));
            Console.WriteLine($"WebClient {useWebClient}");

            string port = args.FirstOrDefault(arg => arg.StartsWith("Port="))?.Split('=')[1] ?? "9000";
            Console.WriteLine($"Port {port}");

            Url = $"http://localhost:{port}/Samples.HttpMessageHandler/";

            Console.WriteLine();
            Console.WriteLine($"Starting HTTP listener at {Url}");

            using (var listener = new HttpListener())
            {
                listener.Prefixes.Add(Url);
                listener.Start();

                // handle http requests in a background thread
                var listenerThread = new Thread(HandleHttpRequests);
                listenerThread.Start(listener);

                if (args.Length == 0 || args.Any(arg => arg.Equals("HttpClient", StringComparison.OrdinalIgnoreCase)))
                {
                    // send an http request using HttpClient
                    Console.WriteLine();
                    Console.WriteLine("Sending request with HttpClient.");
                    HttpClientHelpers.SendHttpClientRequestsAsync(tracingDisabled, Url, RequestContent).GetAwaiter().GetResult();
                }

                if (args.Length == 0 || args.Any(arg => arg.Equals("WebClient", StringComparison.OrdinalIgnoreCase)))
                {
                    // send an http request using WebClient
                    Console.WriteLine();
                    Console.WriteLine("Sending request with WebClient.");
                    WebClientHelpers.SendWebClientsRequest(tracingDisabled, Url, RequestContent);
                }

                Console.WriteLine();
                Console.WriteLine("Stopping HTTP listener.");
                listener.Stop();
            }

            // Force process to end, otherwise the background listener thread lives forever in .NET Core.
            // Apparently listener.GetContext() doesn't throw an exception if listener.Stop() is called,
            // like it does in .NET Framework.
            Environment.Exit(0);
        }

        private static void HandleHttpRequests(object state)
        {
            var listener = (HttpListener)state;

            while (listener.IsListening)
            {
                try
                {
                    var context = listener.GetContext();

                    Console.WriteLine("[HttpListener] received request");

                    // read request content and headers
                    using (var reader = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding))
                    {
                        string requestContent = reader.ReadToEnd();
                        Console.WriteLine($"[HttpListener] request content: {requestContent}");

                        foreach (string headerName in context.Request.Headers)
                        {
                            string headerValue = context.Request.Headers[headerName];
                            Console.WriteLine($"[HttpListener] request header: {headerName}={headerValue}");
                        }
                    }

                    // write response content
                    byte[] responseBytes = Utf8.GetBytes(ResponseContent);
                    context.Response.ContentEncoding = Utf8;
                    context.Response.ContentLength64 = responseBytes.Length;
                    context.Response.OutputStream.Write(responseBytes, 0, responseBytes.Length);

                    // we must close the response
                    context.Response.Close();
                }
                catch (HttpListenerException)
                {
                    // listener was stopped,
                    // ignore to let the loop end and the method return
                }
            }
        }
    }
}
