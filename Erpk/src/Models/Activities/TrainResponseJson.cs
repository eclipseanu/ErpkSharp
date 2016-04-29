using Erpk.Json;
using Newtonsoft.Json;

namespace Erpk.Models.Activities
{
    public enum TrainResultMessage
    {
        [JsonEnum(true)] Successful,
        [JsonEnum("not_enough_gold_error")] NotEnoughGold,
        [JsonEnum("already_trained")] AlreadyTrained
    }

    public class TrainResponseJson : CommonResponseJson
    {
        [JsonConverter(typeof(EnumConverter))]
        public TrainResultMessage message { get; set; }
    }
}