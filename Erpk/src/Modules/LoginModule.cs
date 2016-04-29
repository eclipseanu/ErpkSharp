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
            var token = await GetLoginCsrfToken();

            do
            {
                var res = await TryLogin(token, solution);
                var html = await res.Content.ReadAsStringAsync();

                if (html.Contains("class=\"logout\">Logout</a>"))
                {
                    var home = new HomePage(res);
                    Client.Session.Token = home.Token;
                    return home;
                }

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

        private async Task<string> GetLoginCsrfToken()
        {
            var res = await Client.Get().DisableAutologin().Send();
            return res.XPath().Find("//input[@id='_token']").GetAttribute("value");
        }
    }
}