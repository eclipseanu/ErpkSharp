using System.Threading.Tasks;
using Erpk.Models.Market;
using Erpk.Models.MyPlaces;
using Newtonsoft.Json.Linq;

namespace Erpk.Modules
{
    public class MarketModule : Module
    {
        /// <summary>
        ///     Retrieves citizen's inventory.
        /// </summary>
        public async Task<StoragePage> Inventory()
        {
            var res = await Client.Get("economy/inventory").Send();
            return new StoragePage(res);
        }

        /// <summary>
        ///     Retrieves the list of matching market offers.
        /// </summary>
        public async Task<MarketOffersJson> Scan(IMarketQuery query, int page = 1)
        {
            var req = Client.Post("economy/marketplace").CSRF().XHR();
            req.Form.Add("countryId", query.Country);
            req.Form.Add("industryId", (int) query.Industry);
            req.Form.Add("quality", query.Quality);
            req.Form.Add("orderBy", "price_asc");
            req.Form.Add("currentPage", page);
            req.Form.Add("ajaxMarket", 1);

            var res = await req.Send();
            return res.JSON<MarketOffersJson>();
        }

        /// <summary>
        ///     Purchases existing market offer.
        /// </summary>
        public async Task<MarketBuyResponseJson> BuyOffer(ulong offerId, int amount, int page = 1)
        {
            var req = Client.Post("economy/marketplace").CSRF().XHR();
            req.Form.Add("offerId", offerId);
            req.Form.Add("amount", amount);
            req.Form.Add("orderBy", "price_asc");
            req.Form.Add("currentPage", page);
            req.Form.Add("buyAction", 1);
            req.AddReferer("economy/marketplace");

            var res = await req.Send();
            return res.JSON<MarketBuyResponseJson>();
        }

        /// <summary>
        ///     Posts a new market offer.
        /// </summary>
        public async Task<JObject> PostOffer(INewMarketOffer offer)
        {
            var req = Client.Post("economy/postMarketOffer").CSRF().XHR();
            req.Form.Add("industryId", (int) offer.Industry);
            req.Form.Add("customization", offer.Quality);
            req.Form.Add("amount", offer.Amount);
            req.Form.Add("price", offer.Price);
            req.Form.Add("countryId", offer.Country);

            var res = await req.Send();
            return res.JSON<JObject>();
        }

        /// <summary>
        ///     Deletes an existing market offer.
        /// </summary>
        public async Task<JObject> DeleteOffer(ulong offerId)
        {
            var req = Client.Post("economy/deleteMarketOffer").CSRF().XHR();
            req.Form.Add("id", offerId);

            var res = await req.Send();
            return res.JSON<JObject>();
        }
    }
}