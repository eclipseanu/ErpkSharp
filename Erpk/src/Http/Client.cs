using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Erpk.Models.ReCaptcha;
using Erpk.Modules;

namespace Erpk.Http
{
    public class Client
    {
        private readonly HttpClient _netClient;

        private readonly HttpClientHandler _netClientHandler = new HttpClientHandler
        {
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
            AllowAutoRedirect = true
        };

        public Client(Session session)
        {
            Session = session;

            _netClientHandler.CookieContainer = Session.CookieContainer;
            _netClient = new HttpClient(_netClientHandler)
            {
                Timeout = TimeSpan.FromSeconds(30)
            };

            SetupDefaultRequestHeaders(_netClient.DefaultRequestHeaders);
        }

        /// <summary>
        ///     You can set this property to register captcha solving callback function.
        /// </summary>
        public ReCaptchaSolver ReCaptchaSolver { get; set; }

        /// <summary>
        ///     Gets current session instance.
        /// </summary>
        public Session Session { get; }

        /// <summary>
        ///     Base URI of eRepublik.
        /// </summary>
        public static Uri BaseUri => new Uri("https://www.erepublik.com/en");

        /// <summary>
        ///     Gets or sets default HTTP User-Agent header.
        /// </summary>
        public string UserAgent
        {
            get { return _netClient.DefaultRequestHeaders.UserAgent.ToString(); }
            set { _netClient.DefaultRequestHeaders.Add("User-Agent", value); }
        }

        /// <summary>
        ///     Gets or sets default web proxy.
        /// </summary>
        public IWebProxy Proxy
        {
            get { return _netClientHandler.Proxy; }
            set
            {
                _netClientHandler.Proxy = value;
                _netClientHandler.UseProxy = true;
            }
        }

        /// <summary>
        ///     Initializes module and injects dependencies.
        /// </summary>
        public T Resolve<T>() where T : Module, new()
        {
            return new T {Client = this};
        }

        /// <summary>
        ///     Solve ReCaptcha. Will throw exception when there is not captcha solver specified.
        /// </summary>
        public async Task<ReCaptchaSolution> SolveCaptcha()
        {
            if (ReCaptchaSolver == null)
            {
                throw new InvalidOperationException("There is no captcha solver specified.");
            }
            return await ReCaptchaSolver();
        }

        /// <summary>
        ///     Sets up common HTTP headers to mimic web browser.
        /// </summary>
        private static void SetupDefaultRequestHeaders(HttpRequestHeaders headers)
        {
            headers.ExpectContinue = false;

            headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));
            headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xhtml+xml"));
            headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml", 0.9));
            headers.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*", 0.8));

            headers.AcceptLanguage.Add(new StringWithQualityHeaderValue("en-US"));
            headers.AcceptLanguage.Add(new StringWithQualityHeaderValue("en", 0.7));
        }

        /// <summary>
        ///     Converts unknown kind of URL/path to fully qualified request URI.
        /// </summary>
        public static Uri BuildRequestUri(string url = "")
        {
            if (string.IsNullOrEmpty(url))
            {
                return BaseUri;
            }

            var partialUri = new Uri(url, UriKind.RelativeOrAbsolute);
            return partialUri.IsAbsoluteUri
                ? partialUri
                : new Uri(new Uri(BaseUri.AbsoluteUri + "/"), partialUri);
        }

        /// <summary>
        ///     Creates GET request.
        /// </summary>
        public Request Get(string url = "") => CreateRequest(url, HttpMethod.Get);

        /// <summary>
        ///     Creates POST request.
        /// </summary>
        public Request Post(string url = "") => CreateRequest(url, HttpMethod.Post);

        /// <summary>
        ///     Creates GET request.
        /// </summary>
        public Request Get(Uri url) => CreateRequest(url.ToString(), HttpMethod.Get);

        /// <summary>
        ///     Creates POST request.
        /// </summary>
        public Request Post(Uri url) => CreateRequest(url.ToString(), HttpMethod.Post);

        /// <summary>
        ///     Creates HTTP request.
        /// </summary>
        private Request CreateRequest(string url, HttpMethod method)
        {
            return new Request(BuildRequestUri(url), method, this, _netClient);
        }
    }
}