using Erpk.Json;
using Newtonsoft.Json;

namespace Erpk.Models.Activities
{
    public enum DailyTasksResultMessage
    {
        [JsonEnum("error")] Error,
        [JsonEnum("success")] Success
    }

    public class DailyTasksResponseJson
    {
        [JsonConverter(typeof(EnumConverter))]
        public DailyTasksResultMessage message { get; set; }

        public int day { get; set; }
        public int strength { get; set; }
        public int xp { get; set; }
    }
}