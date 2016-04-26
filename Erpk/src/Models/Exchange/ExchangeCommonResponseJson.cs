using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Erpk.XPath;

namespace Erpk.Models.Exchange
{
    public class MoneyAccountJson
    {
        public int id { get; set; }
        public decimal value { get; set; }
        public int citizen_id { get; set; }
        public int currencyId { get; set; }
        public string currency { get; set; }
        public string currency_icon { get; set; }
    }

    public class ExchangeCommonResponseJson
    {
        public bool error { get; set; }
        public string message { get; set; }
        public MoneyAccountJson ecash { get; set; }
        public MoneyAccountJson gold { get; set; }
        public int page { get; set; }
        public int currencyId { get; set; }
        public string essentials { get; set; }
        public string buy_mode { get; set; }

        public Dictionary<Currency, decimal> ParseAccounts() => new Dictionary<Currency, decimal>
        {
            [Currency.Gold] = gold.value,
            [Currency.Cash] = ecash.value
        };

        public IEnumerable<ExchangeOffer> ParseExchangeOffers()
        {
            return NodeBuilder.FromHtml(buy_mode)
                .FindAll("//*[@class='exchange_offers']/tr")
                .Select(node =>
                {
                    var offer = new ExchangeOffer();

                    var a = node.Find("td[1]/a[1]");
                    var url = a.GetAttribute("href");
                    var button = node.Find("td[@class='ex_buy']/button[1]");

                    offer.Id = ulong.Parse(button.GetAttribute("id").Replace("purchase_", ""));
                    offer.Amount = decimal.Parse(button.GetAttribute("data-max"), CultureInfo.InvariantCulture);
                    offer.Rate = decimal.Parse(button.GetAttribute("data-price"), CultureInfo.InvariantCulture);
                    offer.SellerId = int.Parse(url.Substring(url.LastIndexOf("/", StringComparison.Ordinal) + 1));
                    offer.SellerName = a.GetAttribute("title");

                    return offer;
                });
        }
    }
}