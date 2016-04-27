using Erpk.Models.Common;
using Erpk.Models.Exchange;

namespace Erpk.Models.Donations
{
    public interface IDonateTask
    {
        int RecipientCitizenId { get; }
    }

    public interface IDonateItemsTask : IDonateTask
    {
        Industry Industry { get; }
        int Quality { get; }
        int Amount { get; }
    }

    public interface IDonateMoneyTask : IDonateTask
    {
        Currency Currency { get; }
        float Amount { get; }
    }

    public abstract class DonateTask : IDonateTask
    {
        public int RecipientCitizenId { get; set; }
    }

    public class DonateItemsTask : DonateTask, IDonateItemsTask
    {
        public Industry Industry { get; set; }
        public int Quality { get; set; }
        public int Amount { get; set; }
    }

    public class DonateMoneyTask : DonateTask, IDonateMoneyTask
    {
        public Currency Currency { get; set; }
        public float Amount { get; set; }
    }
}