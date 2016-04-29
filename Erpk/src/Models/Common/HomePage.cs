using System.Linq;
using System.Text.RegularExpressions;
using Erpk.Http;
using Erpk.Models.Military;
using Newtonsoft.Json;

namespace Erpk.Models.Common
{
    public class HomePage : FullPage
    {
        public HomePage(Response response) : base(response)
        {
            var xs = response.XPath();

            if (IsOrganization)
            {
                return;
            }

            var script = xs.FindAll("//script")
                .First(n => n.Extract().Trim().StartsWith("var erepublik"))
                .Extract()
                .Split('\n')
                .Select(l => l.Trim().TrimEnd(',')).ToList();

            var dailyTasksDone = script.First(l => l.StartsWith("dailyTasksDone")).EndsWith("true");
            var dailyOrderDone = script.First(l => l.StartsWith("dailyOrderDone")).EndsWith("true");
            var hasDailyTasksReward = script.First(l => l.StartsWith("hasReward")).EndsWith("true");
            var hasDailyOrderReward = script.First(l => l.StartsWith("hasDailyOrderReward")).EndsWith("true");

            var scriptMap = xs.FindAll("//script")
                .First(n => n.Extract().Trim().StartsWith("var mapSettings"))
                .Extract();

            var doJson = Regex.Match(scriptMap, @"var mapDailyOrder\s*=\s*(.+?);").Groups[1].Value;
            var doModel = JsonConvert.DeserializeObject<DailyOrderJson>(doJson);

            DailyOrder = new DailyOrder
            {
                CampaignId = doModel.CampaignId,
                RegionName = doModel.RegionName,
                Done = dailyOrderDone,
                RewardCollected = hasDailyOrderReward,
                KillsCompleted = doModel.KillsProgress,
                KillsTotal = doModel.KillsTarget
            };

            DailyRewardAvailable = dailyTasksDone && !hasDailyTasksReward;
        }

        public DailyOrder DailyOrder { get; }

        public bool DailyRewardAvailable { get; }
    }
}