using System.Globalization;
using System.Text.RegularExpressions;

namespace Erpk.Models.Exchange
{
    public class ExchangePurchaseJson : ExchangeCommonResponseJson
    {
        public bool hideOffer { get; set; }

        public bool ParseHasExceededGoldLimit() => MatchGoldLimit().Success;

        public float ParseGoldLimitLeft() => float.Parse(MatchGoldLimit().Groups[1].Value, CultureInfo.InvariantCulture);

        private Match MatchGoldLimit() => Regex.Match(message, @"buy more than ([\d.]+)");
    }
}