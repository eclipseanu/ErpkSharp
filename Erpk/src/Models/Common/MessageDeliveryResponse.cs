using Erpk.Http;

namespace Erpk.Models.Common
{
    public class MessageDeliveryResponse
    {
        public string RecipentName { get; }
        public long ThreadId { get; }

        public MessageDeliveryResponse(Response res)
        {
            var xs = res.XPath();
            var nameholder = xs.FindOneOrNull("//div[@class='nameholder']/a[2]");
            if (nameholder != null)
            {
                RecipentName = nameholder.GetAttribute("title");
            }

            var threadholder = xs.Find("//input[@id='thread_id']");
            ThreadId = long.Parse(threadholder.GetAttribute("value"));
        }
    }
}