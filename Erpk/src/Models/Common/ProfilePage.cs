using Erpk.Http;

namespace Erpk.Models.Common
{
    public class ProfilePage : FullPage
    {
        public ProfilePage(Response response, int citizenId)
            : base(response)
        {
            var xs = response.XPath();

            LookedUpCitizenId = citizenId;
            LookedUpCitizenName = xs.Find("//img[@class='citizen_avatar'][1]").Extract();
            AlreadyFriends = xs.FindAny("//a[@class='action_friend_remove tip']");
        }

        public int LookedUpCitizenId { get; }
        public string LookedUpCitizenName { get; }
        public bool AlreadyFriends { get; }
    }
}