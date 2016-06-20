using System.Threading.Tasks;
using Erpk.Models.Exchange;

namespace Erpk.Modules
{
    public class ExchangeModule : Module
    {
        /// <summary>
        ///     Retrieves the list of exchange market offers.
        /// </summary>
        public async Task<ExchangeRetrieveJson> Scan(Currency curr, int page = 1)
        {
            var req = Client.Post("economy/exchange/retrieve/").CSRF().XHR();
            req.Form.Add("page", page - 1);
            req.Form.Add("personalOffers", 0);
            req.Form.Add("currencyId", (int) curr);
            req.AddReferer("economy/exchange-market/");

            var res = await req.Send();
            return res.JSON<ExchangeRetrieveJson>();
        }

        /// <summary>
        ///     Purchases exchange market offer.
        /// </summary>
        public async Task<ExchangePurchaseJson> Buy(ulong offerId, decimal amount)
        {
            var req = Client.Post("economy/exchange/purchase/").CSRF().XHR();
            req.Form.Add("page", 0);
            req.Form.Add("offerId", offerId);
            req.Form.Add("amount", amount);
            req.AddReferer("economy/exchange-market/");

            var res = await req.Send();
            return res.JSON<ExchangePurchaseJson>();
        }
    }
}