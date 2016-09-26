using System;
using System.Linq;
using System.Net;
using Erpk.Json;
using Newtonsoft.Json;

namespace Erpk.Http
{
    /// <summary>
    ///     Holds session information: cookies, token and account credentials.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Session
    {
        private string _token;

        [JsonConstructor]
        internal Session([JsonProperty("email")] string email)
        {
            Email = email;
        }

        public Session(string email, string password)
        {
            Email = email;
            Password = password;
        }

        /// <summary>
        ///     Account's e-mail.
        /// </summary>
        [JsonProperty("email")]
        public string Email { get; }

        /// <summary>
        ///     Account's password.
        /// </summary>
        public string Password { get; internal set; }

        /// <summary>
        ///     CSRF token.
        /// </summary>
        [JsonProperty("token")]
        public string Token
        {
            get { return _token; }
            set
            {
                _token = value;
                OnModified();
            }
        }

        /// <summary>
        ///     Holds HTTP cookies.
        /// </summary>
#if NET461
        [JsonProperty("cookies")]
        [JsonConverter(typeof(BinaryFormatterConverter))]
#endif
        public CookieContainer CookieContainer { get; set; } = new CookieContainer();

        /// <summary>
        ///     Occurs when session properies have been modified.
        /// </summary>
        public event Action Modified;

        /// <summary>
        ///     Tells if session is fresh or expired and needs relogin.
        /// </summary>
        public bool IsFresh()
        {
            var cookies = CookieContainer.GetCookies(Client.BaseUri).Cast<Cookie>().ToList();

            var session = cookies.FirstOrDefault(cookie => cookie.Name == "erpk");
            var rememberMe = cookies.FirstOrDefault(cookie => cookie.Name == "erpk_rm");
            
            return Token != null
                && session != null && !session.Expired
                && rememberMe != null && !rememberMe.Expired;
        }

        /// <summary>
        ///     Invokes Modified event.
        /// </summary>
        public virtual void OnModified()
        {
            Modified?.Invoke();
        }
    }
}