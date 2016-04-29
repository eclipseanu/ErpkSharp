using System;
using System.Net;
using System.Threading.Tasks;
using Erpk.Http;
using Erpk.Models.Donations;

namespace Erpk.Modules
{
    public class DonationsModule : Module
    {
        /// <summary>
        ///     Donates items to selected citizen.
        /// </summary>
        public async Task<DonationResponse> Donate(IDonateItemsTask task)
        {
            var req = await PrepareDonateRequest(task, "items");
            req.Form.Add("amount", task.Amount);
            req.Form.Add("industry_id", (int) task.Industry);
            req.Form.Add("quality", task.Quality);

            var res = await req.Send();
            return new DonationResponse(res);
        }

        /// <summary>
        ///     Donates money to selected citizen.
        /// </summary>
        public async Task<DonationResponse> Donate(IDonateMoneyTask task)
        {
            var req = await PrepareDonateRequest(task, "money");
            req.Form.Add("amount", task.Amount);
            req.Form.Add("currency_id", (int) task.Currency);

            var res = await req.Send();
            return new DonationResponse(res);
        }

        /// <summary>
        ///     Prepares donation request.
        /// </summary>
        private async Task<Request> PrepareDonateRequest(IDonateTask task, string type)
        {
            var referer = string.Format("economy/donate-{0}/{1}", type, task.RecipientCitizenId);

            if (await CheckCaptchaRequired(referer))
            {
                await PassDonationCaptcha(type, referer);
            }

            var req = Client.Post($"economy/donate-{type}-action").CSRF();
            req.Form.Add("citizen_id", task.RecipientCitizenId);
            req.AddReferer(referer);
            return req;
        }

        /// <summary>
        ///     Passes captcha challenge on donation page (if needed).
        /// </summary>
        /// <param name="type">action_type POST parameter.</param>
        /// <param name="referer">Relative referer header path.</param>
        /// <returns>True when captcha solution was valid, false instead.</returns>
        private async Task<bool> PassDonationCaptcha(string type, string referer)
        {
            var req = Client.Post("economy/captcha-donate").CSRF().XHR();
            req.AddReferer(referer);
            req.Form.Add("action_type", $"donate_{type}");
            req.Form.Add(await Client.SolveCaptcha());
            var res = await req.Send();
            return (bool) res.JSON()["status"];
        }

        /// <summary>
        ///     Checks if captcha is required on donation page.
        /// </summary>
        private async Task<bool> CheckCaptchaRequired(string referer)
        {
            var res = await Client.Get(referer).Send();
            var xs = res.XPath();

            if (res.StatusCode == HttpStatusCode.NotFound)
            {
                throw new ArgumentOutOfRangeException(nameof(referer), "Citizen {0} does not exist.", referer);
            }
            if (xs.FindAny("//div[@id='wall_master']"))
            {
                throw new ArgumentException("You cannot donate {0}.", referer);
            }

            return xs.FindAny("//div[@id='recaptchaContainer']");
        }
    }
}