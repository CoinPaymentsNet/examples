using ConsolidationApp.Models.Currencies;
using ConsolidationApp.Models.Invoice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsolidationApp.Models.Merchant
{

    public record PayoutInfo
    {
        private PayoutInfo() { }

        public static PayoutInfo Create(string address, Currency toCurrency, PayoutCurrencyFrequency payoutFrequency)
        {
            if (string.IsNullOrEmpty(address))
                throw new ArgumentNullException($"Address cannot be null for {nameof(PayoutInfo)} in to address mode");

            return new PayoutInfo { Address = address, ToCurrency = toCurrency, PayoutFrequency = payoutFrequency };
        }

        public static PayoutInfo Create(Guid walletId, Currency toCurrency, PayoutCurrencyFrequency payoutFrequency)
            => new PayoutInfo { WalletId = walletId, ToCurrency = toCurrency, PayoutFrequency = payoutFrequency };

        public string? Address { get; private set; }
        public Guid? WalletId { get; private set; }
        public Currency ToCurrency { get; private set; }
        public PayoutCurrencyFrequency PayoutFrequency { get; private set; }

        public bool IsToBalance => WalletId.HasValue;

        public bool IsActiveAddress =>
            IsToBalance ||
            (!string.IsNullOrWhiteSpace(Address) && !Address.StartsWith("Invalid", StringComparison.OrdinalIgnoreCase));
    }

}
