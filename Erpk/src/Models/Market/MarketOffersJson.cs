using System.Collections.Generic;

namespace Erpk.Models.Market
{
    public class MarketOffersJson
    {
        public List<MarketOfferJson> offers { get; set; }
    }

    public class MarketOfferJson
    {
        public ulong id { get; set; }
        public int country_id { get; set; }
        public int industry_id { get; set; }
        public int citizen_id { get; set; }
        public int amount { get; set; }
        public decimal price { get; set; }
        public int customization_level { get; set; }
        public string name { get; set; }
        public int is_for_export { get; set; }
        public decimal priceWithTaxes { get; set; }
    }
}