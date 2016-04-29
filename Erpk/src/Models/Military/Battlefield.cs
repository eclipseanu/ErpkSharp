using System.Text.RegularExpressions;
using Erpk.Http;

namespace Erpk.Models.Military
{
    public class Battlefield
    {
        public Battlefield(Response response)
        {
            var xs = response.XPath();
            SelectedSideId = int.Parse(Regex.Match(xs.OuterHtml, "countryId" + @"\s*:\s*(.+),").Groups[1].Value);
            SelectedAmmo = (Ammo) int.Parse(xs.Find("//a[@id='weapon_btn']").GetAttribute("data-quality"));
        }

        public Ammo SelectedAmmo { get; }
        public int SelectedSideId { get; }
    }
}