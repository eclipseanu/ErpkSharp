using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using Erpk.Http;
using Erpk.Models.Common;
using Erpk.Models.Market;
using NLog;

namespace Erpk.Models.MyPlaces
{
    public class ItemSlots : List<ItemSlot>
    {
        public ItemSlots(int total, int used)
        {
            Total = total;
            Used = used;
        }

        public int Total { get; }
        public int Used { get; }
        public int Free => Total - Used;
    }

    public class ItemSlot
    {
        public ItemSlot(Industry industry, int quality, int amount)
        {
            Industry = industry;
            Quality = quality;
            Amount = amount;
        }

        public Industry Industry { get; }
        public int Quality { get; }
        public int Amount { get; }
    }

    public class StoragePage : FullPage
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public StoragePage(Response response)
            : base(response)
        {
            var xs = response.XPath();
            try
            {
                var slots = Regex.Match(xs.Find("//div[@class='area storage']/h4/strong").Extract(),
                    @"([\d,]+)\/([\d,]+)");

                StorageSlots = new ItemSlots(
                    int.Parse(slots.Groups[1].Value.Replace(",", "")),
                    int.Parse(slots.Groups[2].Value.Replace(",", ""))
                    );
            }
            catch
            {
                Logger.Error("Cannot parse storage capacity.");
                throw;
            }

            try
            {
                DefaultMarketCountry = int.Parse(xs.Find("//*[@id='market_select']").GetAttribute("country"));
            }
            catch
            {
                Logger.Error("Cannot parse default trade country.");
                throw;
            }

            var marketOffers = xs.FindAll("//div[@id='sell_offers']/table[1]/tbody[1]/tr");
            foreach (var tr in marketOffers)
            {
                if (tr.GetAttribute("class") == "buy_market_license")
                {
                    continue;
                }
                var offer = new ExistingMarketOffer();
                try
                {
                    offer.Id = ulong.Parse(tr.GetAttribute("id").Replace("offer_", ""));
                    offer.Price = decimal.Parse(tr.Find("td[@class='offer_price']/strong").Extract().Trim(),
                        CultureInfo.InvariantCulture);
                    offer.Amount =
                        int.Parse(tr.Find("td/strong[@class='offer_amount']").Extract().Trim().Replace(",", ""));

                    var matches = Regex.Match(tr.Find("td[1]/img[1]").GetAttribute("src"), @"industry\/([^_]+)");

                    var img = matches.Groups[1].Value.Replace("q", "").Split('/');

                    offer.Industry = (Industry) int.Parse(img[0]);
                    offer.Quality = img[1] == "default" ? 1 : int.Parse(img[1]);

                    MarketOffers.Add(offer);
                }
                catch
                {
                    Logger.Error("Cannot parse market offer in storage.\n" + tr.OuterHtml);
                    throw;
                }
            }

            var prodList = xs.FindAll("//div[@class='item_mask']/ul/li/strong");
            foreach (var node in prodList)
            {
                try
                {
                    var parts = node.GetAttribute("id").Split('_');
                    var slot = new ItemSlot(
                        (Industry) int.Parse(parts[1]),
                        int.Parse(parts[2]),
                        int.Parse(node.Extract().Trim().Replace(",", ""))
                        );

                    if (slot.Amount > 0)
                    {
                        StorageSlots.Add(slot);
                    }
                }
                catch
                {
                    Logger.Error("Cannot parse slot in the storage.\n" + node.OuterHtml);
                    throw;
                }
            }
        }

        public ItemSlots StorageSlots { get; }
        public List<ExistingMarketOffer> MarketOffers { get; } = new List<ExistingMarketOffer>();
        public int DefaultMarketCountry { get; }
    }
}