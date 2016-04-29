using System;
using System.Net;
using System.Threading.Tasks;
using Erpk.Http;
using Erpk.Models.Common;

namespace Erpk.Modules
{
    public class CommunityModule : Module
    {
        /// <summary>
        ///     Invites citizen to your friends.
        /// </summary>
        public async Task<bool> InviteFriend(int citizenId)
        {
            var url = new Uri(Client.BaseUri, $"citizen/friends/add/{citizenId}?_token={Client.Session.Token}");
            var req = Client.Post(url.ToString());
            req.AddReferer($"citizen/profile/{citizenId}");

            var res = await req.Send();
            return res.Content.ReadAsStringAsync().Result.Contains("Your friendship request has been sent.");
        }

        /// <summary>
        ///     Returns citizen profile page.
        /// </summary>
        public async Task<ProfilePage> GetProfile(int citizenId)
        {
            var res = await Client.Get("citizen/profile/" + citizenId).Send();
            if (res.StatusCode == HttpStatusCode.NotFound)
            {
                throw new ArgumentOutOfRangeException(nameof(citizenId),
                    "Citizen with ID " + citizenId + " does not exist.");
            }

            return new ProfilePage(res, citizenId);
        }
    }
}