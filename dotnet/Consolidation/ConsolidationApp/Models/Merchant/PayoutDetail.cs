using ConsolidationApp.Models.Invoice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ConsolidationApp.Models.Merchant
{

    public class InvoicePayoutsDetailsDto : PagedItemsDto<InvoicePayoutDetailsDto>
    {

        /// <summary>
        /// An array of paid transaction details, including transaction hash, amount, and conversion Id.
        /// </summary>
        public InvoicePaymentTransaction[]? PaidTransactions { get; set; }

        /// <summary>
        /// The date and time when the payment was made.
        /// </summary>
        [JsonPropertyName("paid")]
        public DateTimeOffset? PaidDate { get; set; }

        /// <summary>
        /// The ID of the completed transaction.
        /// </summary>
        public string? CompletedTxId { get; set; }

        /// <summary>
        /// The external address where the payout is deposited
        /// </summary>
        public string? ExternalAddress { get; set; }

        /// <summary>
        /// The currency ID of the destination for the payout
        /// </summary>
        public string? DestinationCurrencyId { get; set; }

        /// <summary>
        /// The expected display value of the payout.
        /// </summary>
        public string? ExpectedDisplayValue { get; set; }

        /// <summary>
        /// The currency ID of the source for the payout
        /// </summary>
        public string? SourceCurrencyId { get; set; }

        /// <summary>
        /// The ID of the destination wallet for the payout
        /// </summary>
        public string? DestinationWalletId { get; set; }

        /// <summary>
        /// Indicates whether a currency conversion is involved in the payout
        /// </summary>
        public bool IsConversion { get; set; }

        /// <summary>
        /// The progress status of the currency conversion
        /// </summary>
        public decimal? ConversionProgress { get; set; }


        public int? SettlementModeErrorCode { get; set; }

        /// <summary>
        /// The destination amount of the payout, including payout amount, state and merchant fees.
        /// </summary>
        public InvoicePayoutDestinationAmountDto? DestinationAmount { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? ReceivedBlockchainTxId { get; set; }
    }
}
