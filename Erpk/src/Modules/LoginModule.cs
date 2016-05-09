using System;
using System.Threading.Tasks;
using Erpk.Http;
using Erpk.Models.Common;
using Erpk.Models.ReCaptcha;
using NLog;

namespace Erpk.Modules
{
    public class LoginModule : Module
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        ///     Logs in eRepublik.
        /// </summary>
        public async Task<HomePage> Login()
        {
            ReCaptchaSolution solution = null;

            // Requests home page.
            // If we checked "remember me", it should redirect us to "/en/login" and then straight away to "/en".
            // HTTP client will follow redirects by default.
            var res = await Client.Get().DisableAutologin().Send();
            if (IsLoggedIn(res))
            {
                // Session cookie has been refreshed and we are already logged in.
                var home = new HomePage(res);
                // Update CSRF token.
                Client.Session.Token = home.Token;
                return home;
            }

            var token = res.XPath().Find("//input[@id='_token']").GetAttribute("value");
            do
            {
                res = await TryLogin(token, solution);
                if (IsLoggedIn(res))
                {
                    var home = new HomePage(res);
                    Client.Session.Token = home.Token;
                    return home;
                }

                var html = await res.Content.ReadAsStringAsync();

                if (html.Contains("The challenge solution was incorrect."))
                    throw new Exception("The captcha challenge solution was incorrect.");

                if (html.Contains("The email address is not valid</span>"))
                    throw new Exception("Invalid e-mail address.");

                if (html.Contains("<form class=\"login\"") && html.Contains("Wrong password"))
                {
                    if (html.Contains("Complete the captcha challenge"))
                    {
                        solution = await Client.SolveCaptcha();
                    }
                    else
                    {
                        throw new Exception("Invalid password.");
                    }
                }
                else
                {
                    Logger.Error("Unknown login error.");
                    Logger.Trace(html);
                    throw new Exception("Unknown login error.");
                }
            } while (true);
        }

        private async Task<Response> TryLogin(string token, ReCaptchaSolution solution)
        {
            var req = Client.Post("login").DisableAutologin();
            req.Form.Add("_token", token);
            req.Form.Add("citizen_email", Client.Session.Email);
            req.Form.Add("citizen_password", Client.Session.Password);
            req.Form.Add("remember", 1);
            req.AddReferer();

            if (solution != null)
            {
                req.Form.Add(solution);
            }

            return await req.Send();
        }

        private static bool IsLoggedIn(Response res)
        {
            return res.ContentAsString.Contains("class=\"logout\">Logout</a>");
        }
    }
}