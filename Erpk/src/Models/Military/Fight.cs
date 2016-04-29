using Newtonsoft.Json;

namespace Erpk.Models.Military
{
    public class AvailableWeaponJson
    {
        [JsonProperty("weaponId")]
        public int Id { get; set; }

        [JsonProperty("weaponQuantity")]
        public int Quantity { get; set; }

        [JsonProperty("weaponInfluence")]
        public float Influence { get; set; }

        [JsonProperty("weaponImage")]
        public string Image { get; set; }

        [JsonProperty("damage")]
        public double Damage { get; set; }
    }

    public enum Ammo
    {
        NW = -1,
        Q1 = 1,
        Q2 = 2,
        Q3 = 3,
        Q4 = 4,
        Q5 = 5,
        Q6 = 6,
        Q7 = 7,
        BAZOOKA = 10
    }
}