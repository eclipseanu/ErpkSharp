using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Erpk.Models.Military;
using Newtonsoft.Json.Linq;

namespace Erpk.Modules
{
    public class MilitaryModule : Module
    {
        /// <summary>
        ///     Returns list of active campaigns.
        /// </summary>
        public async Task<CampaignsModelJson> FetchActiveCampaigns()
        {
            var req = Client.Get("military/campaigns-new");
            var res = await req.Send();
            return res.JSON<CampaignsModelJson>();
        }

        /// <summary>
        ///     Returns list of selected Military Unit members.
        /// </summary>
        public async Task<List<UnitMember>> FetchUnitMembers(int unitId)
        {
            var req = Client.Get($"main/group-list/members/{unitId}");
            var res = await req.Send();

            if (res.StatusCode == HttpStatusCode.NotFound)
            {
                throw new ArgumentOutOfRangeException(nameof(unitId), $"Military Unit with ID {unitId} does not exist.");
            }

            var regiments = res.XPath()
                .FindAll("//select[@id='regiments_lists']/option")
                .Select(opt => new KeyValuePair<int, int>(
                    int.Parse(Regex.Match(opt.Extract(), @"(\d+)").Groups[1].Value),
                    int.Parse(opt.GetAttribute("value"))
                    ));

            var list = new List<UnitMember>();
            foreach (var regiment in regiments)
            {
                req = Client.Get($"main/group-list/members/{unitId}/{regiment.Value}").XHR();
                res = await req.Send();

                var members = res.XPath().FindAll("//tr[@memberid]").Select(tr => new UnitMember
                {
                    Id = int.Parse(tr.GetAttribute("memberid")),
                    Name = tr.Find("td[@class='avatar']").GetAttribute("sort"),
                    RegimentId = regiment.Value,
                    RegimentIndex = regiment.Key
                });

                list.AddRange(members);
            }
            return list;
        }

        /// <summary>
        ///     Returns statistics of selected campaign.
        /// </summary>
        public async Task<CampaignStatsJson> CampaignStats(int campaignId)
        {
            var req = Client.Get($"military/nbp-stats/{campaignId}/1");
            var res = await req.Send();
            return res.JSON<CampaignStatsJson>();
        }

        /// <summary>
        ///     Returns campaign information.
        /// </summary>
        public async Task<Battlefield> Battlefield(int campaignId)
        {
            var req = Client.Get($"military/battlefield-new/{campaignId}");
            var res = await req.Send();

            if (res.XPath().FindAny("//div[@class='listing resistance']"))
            {
                throw new Exception("You need to choose side first!");
            }

            return new Battlefield(res);
        }

        /// <summary>
        ///     Chooses side in selected campaign.
        /// </summary>
        public async Task<Battlefield> ChooseSide(int campaignId, int sideId)
        {
            var req = Client.Get($"military/battlefield-choose-side/{campaignId}/{sideId}");
            var res = await req.Send();
            return new Battlefield(res);
        }

        /// <summary>
        ///     Change weapon in selected campaign.
        /// </summary>
        public async Task<JObject> ChangeWeapon(int campaignId, int weaponId)
        {
            var req = Client.Post("military/change-weapon").CSRF().XHR();
            req.Form.Add("battleId", campaignId);
            req.Form.Add("customizationLevel", weaponId);
            var res = await req.Send();
            return res.JSON();
        }

        /// <summary>
        ///     Lists available weapons.
        /// </summary>
        public async Task<List<AvailableWeaponJson>> ShowWeapons(int campaignId)
        {
            var req = Client.Get(string.Format(
                "military/show-weapons?_token={0}&battleId={1}",
                Client.Session.Token,
                campaignId
                )).XHR();

            var res = await req.Send();
            return res.JSON<List<AvailableWeaponJson>>();
        }

        /// <summary>
        ///     Fights in selected campaign.
        /// </summary>
        public async Task<FightResponseJson> Fight(int campaignId, int sideId)
        {
            var req = Client.Post($"military/fight-shooot/{campaignId}").CSRF().XHR();
            req.AddReferer($"military/battlefield-new/{campaignId}");
            req.Form.Add("battleId", campaignId);
            req.Form.Add("sideId", sideId);
            var res = await req.Send();
            return res.JSON<FightResponseJson>();
        }

        /// <summary>
        ///     Collects the reward for completed Daily Order.
        /// </summary>
        public async Task<GroupMissionsJson> CompleteDailyOrder()
        {
            return await DailyOrderAction();
        }

        /// <summary>
        ///     Previews the reward status of Daily Order.
        /// </summary>
        public async Task<GroupMissionsJson> PreviewDailyOrder()
        {
            return await DailyOrderAction(true);
        }

        private async Task<GroupMissionsJson> DailyOrderAction(bool preview = false)
        {
            var req = Client.Post("military/group-missions").CSRF();
            if (preview)
            {
                req.Form.Add("check", "previewDailyOrderReward");
            }
            req.Form.Add("action", "check");
            var res = await req.Send();
            return res.JSON<GroupMissionsJson>();
        }
    }
}