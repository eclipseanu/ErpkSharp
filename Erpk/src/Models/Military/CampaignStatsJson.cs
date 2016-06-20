using System.Collections.Generic;
using Erpk.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Erpk.Models.Military
{
    public class CampaignStatsJson
    {
        public JObject stats { get; set; }
        public JObject division { get; set; }
        public Dictionary<string, JRaw> logs { get; set; }

        [JsonConverter(typeof(DictionaryOrEmptyArrayConverter<int, JRaw>))]
        public Dictionary<int, JRaw> fightersData { get; set; }

        public int opponentsInQueue { get; set; }
        public bool isInQueue { get; set; }
    }

    public class FighterDataJson
    {
        public int id { get; set; }
        public string name { get; set; }
        public int residence_country_id { get; set; }
        public string avatar { get; set; }
    }
}