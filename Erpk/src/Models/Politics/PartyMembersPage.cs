using System.Collections.Generic;
using System.Linq;
using Erpk.Http;
using Erpk.Models.Common;

namespace Erpk.Models.Politics
{
    public class PartyMember : GroupMember
    {
        public List<string> Positions { get; set; }
    }

    public class PartyMembersPage : FullPage
    {
        public PartyMembersPage(Response response)
            : base(response)
        {
            PartyMembers = response.XPath()
                .FindAll("//div[@class='infoholder']/div[@class='citizen']/../..")
                .Select(node => new PartyMember
                {
                    Id = int.Parse(node.Find("div/div/a").GetAttribute("href").Split('/').Last()),
                    Name = node.Find("div/div/a").GetAttribute("title"),
                    Positions = node.Find("p").Extract().Replace("&nbsp;", "").Split('|').Select(p => p.Trim()).ToList()
                }).ToList();
        }

        public List<PartyMember> PartyMembers { get; }
    }
}