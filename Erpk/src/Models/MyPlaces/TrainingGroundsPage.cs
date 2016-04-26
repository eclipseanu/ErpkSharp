using System.Collections.Generic;
using System.Text.RegularExpressions;
using Erpk.Http;
using Erpk.Models.Common;
using Newtonsoft.Json;
using NLog;

namespace Erpk.Models.MyPlaces
{
    public class TrainingGroundsPage : FullPage
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public TrainingGroundsPage(Response response)
            : base(response)
        {
            var xs = response.XPath();
            HasCaptcha = Regex.IsMatch(xs.OuterHtml, @"has_captcha\s*=\s*'1'");

            try
            {
                var match = Regex.Match(xs.OuterHtml, @"var grounds = (.+);");
                TrainingGrounds = JsonConvert.DeserializeObject<List<TrainingGroundJson>>(match.Groups[1].Value);
            }
            catch
            {
                Logger.Error("Cannot parse JavaScript variable 'grounds' in HTML source.");
                throw;
            }
        }

        public List<TrainingGroundJson> TrainingGrounds { get; }
        public bool HasCaptcha { get; }
    }

    public class TrainingGroundJson
    {
        public uint id { get; set; }
        public string price { get; set; }
        public decimal cost { get; set; }
        public float bonus { get; set; }
        public string quality { get; set; }
        public bool trained { get; set; }
        public int untilMidnight { get; set; }
        public bool @default { get; set; }
        public string img { get; set; }
        public string name { get; set; }
        public string coreName { get; set; }
        public float strength { get; set; }
        public Dictionary<int, TrainingGroundUpgradeJson> upgrades { get; set; }
        public string token { get; set; }
        public string @class { get; set; }
        public string icon { get; set; }
    }

    public class TrainingGroundUpgradeJson
    {
        public int level { get; set; }
        public float strength { get; set; }
        public int cost { get; set; }
        public string img { get; set; }
        public int type { get; set; }
    }
}