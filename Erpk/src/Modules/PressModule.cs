using System;
using System.Threading.Tasks;
using Erpk.Http;
using Erpk.Models.Press;
using Erpk.Models.ReCaptcha;

namespace Erpk.Modules
{
    public class PressModule : Module
    {
        /// <summary>
        ///     Upvotes an article.
        /// </summary>
        public async Task<int> VoteArticle(int articleId)
        {
            var req = Client.Post(new Uri(Client.BaseUri, "vote-article")).CSRF();
            req.Form.Add("article_id", articleId);

            var res = await req.Send();
            return (int) res.JSON()["votes"];
        }

        /// <summary>
        ///     Subscribes or unsubscribes from newspaper.
        /// </summary>
        public async Task<SubscriptionResultJson> ManageNewspaperSubscription(int newspaperId, SubscriptionAction action)
        {
            var unsubscribe = action == SubscriptionAction.Unsubscribe;

            var result = await TrySubscribeNewspaper(newspaperId, unsubscribe);
            if (result.CaptchaRequired)
            {
                result = await TrySubscribeNewspaper(newspaperId, unsubscribe, await Client.SolveCaptcha());
            }

            return result;
        }

        private async Task<SubscriptionResultJson> TrySubscribeNewspaper(int newspaperId, bool unsubscribe,
            ReCaptchaSolution solution = null)
        {
            var req = Client.Post(new Uri(Client.BaseUri, "subscribe")).CSRF();
            req.Form.Add("type", (unsubscribe ? "un" : "") + "subscribe");
            req.Form.Add("n", newspaperId);
            req.AddReferer("newspaper/" + newspaperId + "/1");

            if (solution != null)
            {
                req.Form.Add(solution);
            }

            var res = await req.Send();
            return res.JSON<SubscriptionResultJson>();
        }
    }
}