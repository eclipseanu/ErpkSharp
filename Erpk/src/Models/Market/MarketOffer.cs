using Erpk.Models.Common;

namespace Erpk.Models.Market
{
    public interface IMarketQuery
    {
        int Country { get; }
        Industry Industry { get; }
        int Quality { get; }
    }

    public interface INewMarketOffer : IMarketQuery
    {
        int Amount { get; }
        decimal Price { get; }
    }

    public interface IExistingMarketOffer : INewMarketOffer
    {
        ulong Id { get; }
    }

    public class MarketQuery : IMarketQuery
    {
        public int Country { get; set; }
        public Industry Industry { get; set; }
        public int Quality { get; set; } = 1;
    }

    public class NewMarketOffer : MarketQuery, INewMarketOffer
    {
        public int Amount { get; set; }
        public decimal Price { get; set; }
    }

    public class ExistingMarketOffer : NewMarketOffer, IExistingMarketOffer
    {
        public ulong Id { get; set; }
    }
}