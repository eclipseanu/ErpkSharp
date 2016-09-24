using System.Collections.Generic;

namespace Erpk.Models.Common
{
    public class RegionsListJson
    {
        public Dictionary<int, TravelRegionJson> regions { get; set; }
    }

    public class TravelRegionJson
    {
        public int id { get; set; }
        public string name { get; set; }
        public string permalink { get; set; }
        public bool is_original { get; set; }
        public float distance { get; set; }
        public int cost { get; set; }
        public bool can_move { get; set; }
    }
}