using Newtonsoft.Json;

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
        [JsonProperty("completed")]
        public bool Completed { get; set; }

        [JsonProperty("hasReward")]
        public bool HasReward { get; set; }

        [JsonProperty("successfullyRewarded", Required = Required.Default)]
        public bool SuccessfullyRewarded { get; set; }
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