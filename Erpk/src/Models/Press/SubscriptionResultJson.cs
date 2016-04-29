using Newtonsoft.Json;

namespace Erpk.Models.Press
{
    public class SubscriptionResultJson
    {
        [JsonProperty("show_captcha")]
        public bool CaptchaRequired { get; set; }

        [JsonProperty("subscribers")]
        public int Subscribers { get; set; }
    }
}