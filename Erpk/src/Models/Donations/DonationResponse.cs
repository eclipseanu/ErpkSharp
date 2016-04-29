using Erpk.Http;
using Erpk.Models.Common;

namespace Erpk.Models.Donations
{
    public enum DonationResult
    {
        Successful,
        NotEnoughItems,
        NotEnoughMoney,
        NothingHappened,
        InvalidAmount,
        Failed
    }

    public class DonationResponse : FullPage
    {
        public DonationResponse(Response response)
            : base(response)
        {
            var xs = response.XPath();
            DonationMessage = xs.FindOneOrNull("//div[@class='citizen_content up donate'][1]/div[1]/table[1]/tr[1]/td[1]")?.Extract();
        }

        public string DonationMessage { get; }

        public DonationResult ParseDonationMessage()
        {
            if (string.IsNullOrEmpty(DonationMessage))
            {
                return DonationResult.NothingHappened;
            }

            if (DonationMessage.StartsWith("Successfully transferred") || DonationMessage.StartsWith("You have successfully donated"))
                return DonationResult.Successful;

            if (DonationMessage.StartsWith("You do not have enough items"))
                return DonationResult.NotEnoughItems;

            if (DonationMessage.StartsWith("You do not have enough money"))
                return DonationResult.NotEnoughMoney;

            if (DonationMessage.StartsWith("Please enter a valid amount value"))
                return DonationResult.InvalidAmount;

            return DonationResult.Failed;
        }
    }
}