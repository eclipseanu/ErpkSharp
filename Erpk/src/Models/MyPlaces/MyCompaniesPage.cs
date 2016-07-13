using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Erpk.Http;
using Erpk.Json;
using Erpk.Models.Common;
using Newtonsoft.Json;
using NLog;

namespace Erpk.Models.MyPlaces
{
    public class MyCompaniesPage : FullPage
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public MyCompaniesPage(Response response)
            : base(response)
        {
            var xs = response.XPath();

            AlreadyWorked = xs.FindAny("//img[@class='employee_worked']");

            var script = xs.FindAll("//script").First(n => n.Extract().Trim().StartsWith("var companies")).Extract();
            HasCaptcha = Regex.Match(script, @"has_captcha\s*=\s*'1'").Success;

            try
            {
                var match = Regex.Match(script, "companies" + @"\s*=\s*(.+);");
                Companies = JsonConvert.DeserializeObject<Dictionary<uint, CompanyJson>>(match.Groups[1].Value);
            }
            catch
            {
                Logger.Error("Cannot retrieve JavaScript 'companies' variable from HTML source.");
                throw;
            }

            try
            {
                var match = Regex.Match(script, "pageDetails" + @"\s*=\s*(.+);");
                Details = JsonConvert.DeserializeObject<CompaniesDetailsJson>(match.Groups[1].Value);
            }
            catch
            {
                Logger.Error("Cannot retrieve JavaScript 'pageDetails' variable from HTML source.");
                throw;
            }
        }

        public Dictionary<uint, CompanyJson> Companies { get; }
        public CompaniesDetailsJson Details { get; }
        public bool AlreadyWorked { get; }
        public bool HasCaptcha { get; }
    }

    public class CompanyJson
    {
        public uint id { get; set; }
        public int industry_id { get; set; }
        public string industry_name { get; set; }
        public string industry_token { get; set; }
        public string products_img { get; set; }
        public int quality { get; set; }
        public string building_name { get; set; }
        public string building_img { get; set; }
        public bool is_raw { get; set; }
        public string raw_img { get; set; }
        public int preset_works { get; set; }
        public int preset_own_work { get; set; }
        public float base_production { get; set; }
        public float resource_bonus { get; set; }
        public int raw_usage { get; set; }
        public float production { get; set; }
        public string employee_limit { get; set; }
        public string upgrade_url { get; set; }
        public bool already_worked { get; set; }
        public int todays_works { get; set; }
        public int total_works { get; set; }
        public string sell_url { get; set; }
        public int max_quality { get; set; }

        [JsonConverter(typeof(DictionaryOrEmptyArrayConverter<int, CompanyUpgradeJson>))]
        public Dictionary<int, CompanyUpgradeJson> upgrades { get; set; }
    }

    public class CompanyUpgradeJson
    {
        public int level { get; set; }
        public int employees { get; set; }
        public int cost { get; set; }
        public string img { get; set; }
        public int raw_usage { get; set; }
        public int type { get; set; }
    }

    public class CompaniesDetailsJson
    {
        public int total_works { get; set; }
        public int food_raw_stock { get; set; }
        public int weapon_raw_stock { get; set; }
        public int house_raw_stock { get; set; }
    }
}