using System;
using System.Net;
using System.Threading.Tasks;
using Erpk.Models.Activities;
using Erpk.Models.Common;
using Erpk.Models.Politics;
using Newtonsoft.Json.Linq;
using NLog;

namespace Erpk.Modules
{
    public class MainModule : Module
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        ///     Scrapes home/front page.
        /// </summary>
        public async Task<HomePage> Home()
        {
            var res = await Client.Get().Send();
            return new HomePage(res);
        }

        /// <summary>
        ///     Refills energy.
        /// </summary>
        public async Task<EnergyState> Eat()
        {
            var res = await Client.Get("main/eat?format=json&_token=" + Client.Session.Token).Send();
            return new EnergyState(res);
        }

        /// <summary>
        ///     Collects reward from Daily Tasks.
        /// </summary>
        public async Task<DailyTasksResponseJson> CollectDailyTasksReward()
        {
            var req = Client.Get("main/daily-tasks-reward").XHR();
            req.AddReferer();

            var res = await req.Send();
            return res.JSON<DailyTasksResponseJson>();
        }

        /// <summary>
        ///     Collects reward from Weekly Challenge.
        /// </summary>
        public async Task<JObject> CollectWeeklyChallengeReward(int id)
        {
            var req = Client.Post("main/weekly-challenge-collect-reward").CSRF().XHR();
            req.AddReferer();
            req.Form.Add("id", id);

            var res = await req.Send();
            return res.JSON();
        }

        /// <summary>
        ///     Retrieves list of party members.
        /// </summary>
        public async Task<PartyMembersPage> GetPartyMembers(int partyId)
        {
            var res = await Client.Get("main/party-members/" + partyId).Send();
            if (res.StatusCode == HttpStatusCode.NotFound)
            {
                throw new ArgumentOutOfRangeException(nameof(partyId),
                    "Political party with ID " + partyId + " does not exist.");
            }

            return new PartyMembersPage(res);
        }

        /// <summary>
        ///     Sends private message to given recipient.
        /// </summary>
        public async Task<MessageDeliveryResponse> SendPrivateMessage(int citizenId, string subject, string message)
        {
            var gotCaptcha = await CheckPrivateMessageCaptchaFound(citizenId);
            var solution = gotCaptcha ? await Client.SolveCaptcha() : null;

            var url = "main/messages-compose/" + citizenId;
            var req = Client.Post(url).CSRF().XHR();
            req.AddReferer(url);
            req.Form.Add("citizen_name", citizenId);
            req.Form.Add("citizen_subject", subject);
            req.Form.Add("citizen_message", message);
            if (gotCaptcha)
            {
                req.Form.Add(solution);
            }
            var res = await req.Send();

            try
            {
                return new MessageDeliveryResponse(res);
            }
            catch
            {
                Logger.Error("Cannot parse message sending result.");
                throw;
            }
        }

        private async Task<bool> CheckPrivateMessageCaptchaFound(int citizenId)
        {
            var url = "main/messages-compose/" + citizenId;
            var req = Client.Get(url);

            var res = await req.Send();
            return res.XPath().FindAny("//div[@id='recaptchaContainer']");
        }
    }
}