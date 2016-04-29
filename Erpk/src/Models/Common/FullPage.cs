using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Erpk.Http;
using Erpk.Models.Exchange;
using NLog;

namespace Erpk.Models.Common
{
    public class FullPage
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public FullPage(Response response)
        {
            var xs = response.XPath();
            var html = xs.OuterHtml;

            IsOrganization = xs.FindAny("//div[@class='user_level'][1]/b[1][text() = 'Or']");

            Match match;
            if (!IsOrganization)
            {
                try
                {
                    var energy = xs.Find("//strong[@id='current_health']").Extract().Trim().Split('/');
                    EnergyState = new EnergyState
                    {
                        Current = int.Parse(energy[0]),
                        Maximum = int.Parse(energy[1]),
                        Recoverable = int.Parse(xs.Find("//big[@class='tooltip_health_limit']").Extract())
                    };
                }
                catch
                {
                    Logger.Error("Cannot retrieve energy bar.");
                    throw;
                }


                if (EnergyState.Recoverable < EnergyState.Maximum)
                {
                    try
                    {
                        match = Regex.Match(html, @"new_date = '(\d+)'");
                        var seconds = int.Parse(match.Groups[1].Value);
                        EnergyState.RecoverMoreIn = TimeSpan.FromSeconds(seconds);
                    }
                    catch
                    {
                        Logger.Error("Cannot parse refill status.");
                        throw;
                    }
                }
                else
                {
                    EnergyState.RecoverMoreIn = TimeSpan.FromSeconds(0);
                }


                try
                {
                    match = Regex.Match(html, @"reset_has_food = (\d+)");
                    EnergyState.HasFood = int.Parse(match.Groups[1].Value) > 0;
                }
                catch
                {
                    Logger.Error("Cannot parse refill status.");
                    throw;
                }
            }

            string script;
            try
            {
                match = Regex.Match(html, @"<script.*>\s*(var erepublik[\s\S]*?)</script>");
                script = match.Groups[1].Value;
            }
            catch
            {
                Logger.Error("Cannot extract <script> with 'var citizen' variable.");
                throw;
            }

            var intVars = ParseIntegerVariables(script);

            try
            {
                Token = Regex.Match(script, @"csrfToken\s*:\s*'([a-z0-9]+)'").Groups[1].Value;
            }
            catch
            {
                Logger.Error("Cannot retrieve CSRF token.");
                throw;
            }

            CitizenshipCountryId = intVars["country"];
            ResidenceCountryId = intVars["countryLocationId"];
            Division = intVars["division"];

            try
            {
                Accounts = new Dictionary<Currency, float>
                {
                    [Currency.Gold] = intVars["gold"],
                    [Currency.Cash] = intVars["currencyAmount"]
                };
            }
            catch
            {
                Logger.Error("Cannot retrieve gold or currency amount from sidebar.");
                throw;
            }

            try
            {
                var userLink = xs.Find("//a[@class='user_name']");

                CitizenId = intVars["citizenId"];
                CitizenName = userLink.Extract().Trim();
            }
            catch
            {
                Logger.Error("Cannot retrieve citizen ID or name from HTML source");
                throw;
            }
        }

        /// <summary>
        ///     How much gold and currency does citizen have.
        /// </summary>
        public Dictionary<Currency, float> Accounts { get; }

        /// <summary>
        ///     Citizen ID.
        /// </summary>
        public int CitizenId { get; }

        /// <summary>
        ///     Citizen name.
        /// </summary>
        public string CitizenName { get; }

        /// <summary>
        ///     Is the current account an organization or regular citizen.
        /// </summary>
        public bool IsOrganization { get; }

        /// <summary>
        ///     Citizen's division.
        /// </summary>
        public int Division { get; }

        /// <summary>
        ///     Citizenship country of citizen.
        /// </summary>
        public int CitizenshipCountryId { get; }

        /// <summary>
        ///     Residence country of citizen.
        /// </summary>
        public int ResidenceCountryId { get; }

        /// <summary>
        ///     How much energy does citizen have.
        /// </summary>
        public EnergyState EnergyState { get; }

        /// <summary>
        ///     CSRF token
        /// </summary>
        public string Token { get; }

        private static Dictionary<string, int> ParseIntegerVariables(string script)
        {
            var matches = Regex.Matches(script,
                @"(?:(citizenId|energy|gold|currencyAmount|userLevel|canWorkTrainAgainIn|currentExperiencePoints|energyToRecover|country|countryLocationId|division)\s*:\s*'?(\d+))");

            var vars = new Dictionary<string, int>();

            foreach (Match number in matches)
            {
                var key = number.Groups[1].Value;
                var value = int.Parse(number.Groups[2].Value);
                vars.Add(key, value);
            }
            return vars;
        }
    }
}