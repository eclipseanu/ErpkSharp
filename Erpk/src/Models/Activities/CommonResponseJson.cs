using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Erpk.Models.Activities
{
    public abstract class CommonResponseJson
    {
        public bool status { get; set; }

        public JRaw result { get; set; }

        public T ParseResult<T>() => JsonConvert.DeserializeObject<T>(result.ToString());
    }
}