using System;
using Erpk.Http;
using NLog;

namespace Erpk.Models.Common
{
    public class EnergyState
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public EnergyState()
        {
        }

        public EnergyState(Response response)
        {
            try
            {
                var raw = response.JSON();
                Current = (int) raw["health"];
                Recoverable = (int) raw["food_remaining"];

                var maxApprox = Current/((double) raw["current_energy_ratio"]/100.0);
                Maximum = (int) Math.Round(maxApprox/10.0)*10;
                HasFood = (int) raw["has_food_in_inventory"] > 0;

                var str = ((string) raw["food_remaining_reset"]).Split(':');
                RecoverMoreIn = TimeSpan.FromMinutes(int.Parse(str[1]));
                RecoverMoreIn = RecoverMoreIn.Add(TimeSpan.FromSeconds(int.Parse(str[2])));
            }
            catch
            {
                Logger.Error("Cannot parse JSON eat result.");
                throw;
            }
        }

        public int Current { get; set; }
        public int Maximum { get; set; }
        public int Recoverable { get; set; }
        public TimeSpan RecoverMoreIn { get; set; }
        public bool HasFood { get; set; }
        public int Total => Current + Recoverable;
        public bool CanEat => HasFood && Recoverable > 0 && Current < Maximum;

        public override string ToString()
        {
            return $"{Current}/{Maximum} (+{Recoverable})";
        }
    }
}