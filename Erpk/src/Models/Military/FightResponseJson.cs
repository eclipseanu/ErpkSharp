using System;
using Erpk.Json;
using Erpk.Models.Common;
using Newtonsoft.Json;

namespace Erpk.Models.Military
{
    public enum FightResultMessage
    {
        [JsonEnum("ENEMY_KILLED")] EnemyKilled,
        [JsonEnum("ENEMY_ATTACKED")] EnemyAttacked,
        [JsonEnum("LOW_HEALTH")] LowEnergy,
        [JsonEnum("ZONE_INACTIVE")] ZoneInactive,
        [JsonEnum("SHOOT_LOCKOUT")] ShootLockout
    }

    public class FightResponseJson
    {
        public bool error { get; set; }

        [JsonConverter(typeof(EnumConverter))]
        public FightResultMessage message { get; set; }

        public FightResponseDetailsJson details { get; set; }
        public FightResponseUserJson user { get; set; }

        public int hits { get; set; }


        public EnergyState ParseEnergyState()
        {
            if (error || details == null || user == null)
            {
                return null;
            }

            return new EnergyState
            {
                Recoverable = user.food_remaining,
                Current = details.wellness,
                Maximum = details.energy_limit,
                HasFood = user.has_food_in_inventory > 0,
                RecoverMoreIn = TimeSpan.FromSeconds(user.food_time_reset)
            };
        }
    }

    public class FightResponseDetailsJson
    {
        public int level { get; set; }
        public long points { get; set; }
        public long max { get; set; }
        public float percent { get; set; }
        public int wellness { get; set; }
        public int energy_limit { get; set; }
        public int current_energy_ratio { get; set; }
        public int remaining_energy_ratio { get; set; }
        public decimal currency { get; set; }
        public decimal gold { get; set; }
        public int specialFoodValue { get; set; }
        public int specialFoodmout { get; set; }
    }

    public class FightResponseUserJson
    {
        public int givenDamage { get; set; }
        public int earnedRankPoints { get; set; }
        public int earnedXp { get; set; }
        public int PVPKillCount { get; set; }
        public float health { get; set; }
        public int countWeapons { get; set; }
        public int weaponId { get; set; }
        public float skill { get; set; }
        public string weaponImage { get; set; }
        public int weaponDamage { get; set; }
        public int weaponDamagePercent { get; set; }
        public int weaponDurability { get; set; }
        public float weaponDurabilityPercent { get; set; }
        public int hasBazookaAmmo { get; set; }
        public int level { get; set; }
        public int division { get; set; }
        public bool bazookaAmmo { get; set; }
        public int has_food_in_inventory { get; set; }
        public int food_remaining { get; set; }
        public long food_remaining_reset { get; set; }
        public int food_time_reset { get; set; }
        public int specialFoodAmount { get; set; }
        public int specialFoodValue { get; set; }
        public float weaponInfluence { get; set; }
        public int damageBoost { get; set; }
        public int maxHit { get; set; }
        public int weaponQuantity { get; set; }
    }
}