using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Erpk.Modules;

namespace Erpk.Http
{
    public class Request
    {
        private readonly Client _client;
        private readonly HttpClient _netClient;
        private readonly HttpRequestMessage _netRequest;
        private bool _autologin = true;
        private bool _csrfToken;

        public Request(Uri uri, HttpMethod method, Client client, HttpClient netClient)
        {
            _client = client;
            _netClient = netClient;
            _netRequest = new HttpRequestMessage(method, uri);
        }

        /// <summary>
        ///     Gets POST fields form.
        /// </summary>
        public Form Form { get; } = new Form();

        /// <summary>
        ///     Gets request HTTP headers collection.
        /// </summary>
        public HttpRequestHeaders Headers => _netRequest.Headers;

        /// <summary>
        ///     Disables autologin. You need this when you're actually logging in to avoid infinite recursion.
        /// </summary>
        public Request DisableAutologin()
        {
            _autologin = false;
            return this;
        }

        /// <summary>
        ///     Marks request as XMLHttpRequest
        /// </summary>
        public Request XHR()
        {
            Headers.Add("X-Requested-With", "XMLHttpRequest");
            return this;
        }

        /// <summary>
        ///     Adds CSRF token to POST fields.
        /// </summary>
        public Request CSRF()
        {
            _csrfToken = true;
            return this;
        }

        /// <summary>
        ///     Adds HTTP referer header relative to base URL.
        /// </summary>
        public void AddReferer(string path = "")
        {
            Headers.Add("Referer", Client.BuildRequestUri(path).ToString());
        }

        /// <summary>
        ///     Sends this request asynchronously and returns the response.
        /// </summary>
        public async Task<Response> Send()
        {
            if (_autologin && !_client.Session.IsFresh())
            {
                await _client.Resolve<LoginModule>().Login();
            }

            if (Form.FieldsCount > 0)
            {
                if (_csrfToken)
                {
                    Form.Add("_token", _client.Session.Token);
                }
                _netRequest.Content = Form.Encode();
            }

            var rawResponse = await _netClient.SendAsync(_netRequest);
            if (rawResponse.StatusCode == HttpStatusCode.ProxyAuthenticationRequired)
            {
                throw new HttpRequestException("Proxy authentication required or username/password is invalid.");
            }

            _client.Session.OnModified();

            return new Response(rawResponse);
        }
    }
}