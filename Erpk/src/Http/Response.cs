using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Erpk.XPath;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;

namespace Erpk.Http
{
    public class Response
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly HttpResponseMessage _response;
        private Node _xpathCached;

        public Response(HttpResponseMessage rawResponse)
        {
            _response = rawResponse;
        }

        /// <summary>
        ///     Returns response content.
        /// </summary>
        public HttpContent Content => _response.Content;

        /// <summary>
        ///     Returns response content as string.
        /// </summary>
        public string ContentAsString => Content.ReadAsStringAsync().Result;

        /// <summary>
        ///     Returns response headers.
        /// </summary>
        public HttpResponseHeaders Headers => _response.Headers;

        /// <summary>
        ///     Returns response status code.
        /// </summary>
        public HttpStatusCode StatusCode => _response.StatusCode;

        /// <summary>
        ///     Returns response reason phrase.
        /// </summary>
        public string ReasonPhrase => _response.ReasonPhrase;

        /// <summary>
        ///     Returns whether response is redirection.
        /// </summary>
        public bool IsRedirect => (int) StatusCode >= 300 && (int) StatusCode <= 399;

        /// <summary>
        ///     Parses HTML content to XPath selector.
        /// </summary>
        public Node XPath()
        {
            return _xpathCached ?? (_xpathCached = NodeBuilder.FromHtml(ContentAsString));
        }

        /// <summary>
        ///     Parses JSON content to specified type.
        /// </summary>
        public T JSON<T>()
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(ContentAsString);
            }
            catch (JsonSerializationException e)
            {
                Logger.Error(e);
                Logger.Trace("Corrupted JSON: {0}", ContentAsString);
                throw;
            }
        }

        /// <summary>
        ///     Parses JSON contents to Linq object.
        /// </summary>
        public JObject JSON()
        {
            return JObject.Parse(ContentAsString);
        }
    }
}