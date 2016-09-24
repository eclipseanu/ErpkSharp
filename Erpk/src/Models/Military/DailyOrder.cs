using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Erpk.Models.Military
{
    public class DailyOrder
    {
        public int CampaignId { get; set; }
        public bool Done { get; set; }
        public int KillsCompleted { get; set; }
        public int KillsTotal { get; set; }
        public string RegionName { get; set; }
        public bool RewardCollected { get; set; }
        public bool RewardAvailable => Done && !RewardCollected;
    }

    public class DailyOrderJson
    {
        [JsonProperty("do_enabled")]
        public bool Enabled { get; set; }

        [JsonProperty("do_mission_id")]
        public long MissionId { get; set; }

        [JsonProperty("do_region_id")]
        public int RegionId { get; set; }

        [JsonProperty("do_region_name")]
        public string RegionName { get; set; }

        [JsonProperty("do_for_country")]
        public string ForCountry { get; set; }

        [JsonProperty("do_battle_id")]
        public int CampaignId { get; set; }

        [JsonProperty("do_progress")]
        public int KillsProgress { get; set; }

        [JsonProperty("do_target")]
        public int KillsTarget { get; set; }

        [JsonProperty("do_show_reward")]
        public bool ShowReward { get; set; }

        [JsonProperty("do_reward_on")]
        public bool RewardOn { get; set; }
    }

    public class GroupMissionsMessageJson
    {
        // Might be bool or integer. Integer means how many DO kills have been dealt so far.
        [JsonProperty("completed")]
        public JRaw Completed { private get; set; }

        [JsonProperty("hasReward")]
        public bool HasReward { get; set; }

        [JsonProperty("successfullyRewarded", Required = Required.Default)]
        public bool SuccessfullyRewarded { get; set; }

        public bool ParseCompleted()
        {
            var reader = new JsonTextReader(new StringReader(Completed.ToString()));
            reader.Read();

            switch (reader.TokenType)
            {
                case JsonToken.String:
                case JsonToken.Integer:
                    return false;
                case JsonToken.Boolean:
                    return (bool) reader.Value;
                default:
                    return false;
            }
        }
    }

    public class GroupMissionsJson
    {
        [JsonProperty("msg")]
        public GroupMissionsMessageJson Message { get; set; }

        [JsonProperty("error")]
        public bool Error { get; set; }

        [JsonProperty("multiplier")]
        public float Multiplier { get; set; }
    }
}