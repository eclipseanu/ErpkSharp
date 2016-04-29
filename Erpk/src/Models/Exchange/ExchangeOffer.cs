using System;

namespace Erpk.Models.Exchange
{
    public class ExchangeOffer : IEquatable<ExchangeOffer>
    {
        public ulong Id { get; set; }
        public decimal Amount { get; set; }
        public decimal Rate { get; set; }
        public int SellerId { get; set; }
        public string SellerName { get; set; }

        public bool Equals(ExchangeOffer exchangeOffer)
        {
            return Id == exchangeOffer.Id && Amount == exchangeOffer.Amount;
        }
    }
}