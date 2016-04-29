using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Erpk.Json;
using Newtonsoft.Json;

namespace Erpk.Models.Military
{
    public class CampaignsModelJson
    {
        [JsonProperty("request_time")]
        [JsonConverter(typeof(UnixTimestampConverter))]
        public DateTimeOffset RequestTime { get; set; }

        [JsonProperty("last_updated")]
        [JsonConverter(typeof(UnixTimestampConverter))]
        public DateTimeOffset LastUpdated { get; set; }

        [JsonProperty("battles")]
        public Dictionary<int, CampaignJson> Campaigns { get; set; }

        [JsonProperty("countries")]
        public Dictionary<int, CountryJson> Countries { get; set; }
    }

    public class CampaignJson
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("war_id")]
        public int WarId { get; set; }

        [JsonProperty("is_rw")]
        public bool IsResistanceWar { get; set; }

        [JsonProperty("is_as")]
        public bool IsAirstrike { get; set; }

        [JsonProperty("start")]
        [JsonConverter(typeof(UnixTimestampConverter))]
        public DateTimeOffset StartTime { get; set; }

        [JsonProperty("det")]
        public double Determination { get; set; }

        [JsonProperty("region")]
        public RegionJson Region { get; set; }

        [JsonProperty("is_dict")]
        public bool IsDictatorship { get; set; }

        [JsonProperty("is_lib")]
        public bool IsLiberation { get; set; }

        [JsonProperty("inv")]
        public SideJson Invader { get; set; }

        [JsonProperty("def")]
        public SideJson Defender { get; set; }

        [JsonProperty("div")]
        public Dictionary<int, DivisionJson> Divisions { get; set; }

        public IEnumerable<SideJson> Sides => new List<SideJson> {Invader, Defender};
    }

    public class SideJson
    {
        [JsonProperty("id")]
        public int CountryId { get; set; }

        [JsonProperty("points")]
        public int CampaignPoints { get; set; }
    }

    public class DivisionJson
    {
        [JsonProperty("div")]
        public int DivisionNumber { get; set; }

        [JsonProperty("end")]
        [JsonConverter(typeof(UnixTimestampConverter))]
        public DateTimeOffset? EndTime { get; set; }

        [JsonProperty("epic")]
        public int Epic { get; set; }

        [JsonProperty("co")]
        public CombatOrdersJson CombatOrders { get; set; }
    }

    public class CombatOrdersJson
    {
        [JsonProperty("inv")]
        public List<CombatOrderJson> Invader { get; set; }

        [JsonProperty("def")]
        public List<CombatOrderJson> Defender { get; set; }
    }

    public class CombatOrderJson
    {
        [JsonProperty("reward")]
        public float Reward { get; set; }

        [JsonProperty("budget")]
        public double Budget { get; set; }

        [JsonProperty("threshold")]
        public double Threshold { get; set; }

        [JsonProperty("sub_country")]
        public int AllowedCitizenship { get; set; }

        [JsonProperty("sub_mu")]
        public int AllowedMilitaryUnit { get; set; }
    }

    public class CountryJson
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("allies")]
        public List<int> Allies { get; set; }

        [JsonProperty("is_empire")]
        public bool IsEmpire { get; set; }

        [JsonProperty("cotd")]
        public int CampaignOfTheDay { get; set; }

        public string Permalink => GetPermalink(Name);
        public string Flag => "//www.erepublik.net/images/flags_png/L/" + Permalink + ".png";

        private static string GetPermalink(string str)
        {
            str = Regex.Replace(str, @"\s+", "-");
            str = Regex.Replace(str, @"[()]", "");
            return str;
        }
    }

    public class RegionJson
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}