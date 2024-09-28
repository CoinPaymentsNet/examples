using ConsolidationApp.Models.Currencies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsolidationApp.Models.Invoice
{

    public class InvoicePaymentPayoutModel
    {
        /// <summary>
        /// the unique id of the invoice hot wallet payout (PK)
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Invoice payment id
        /// </summary>
        public Guid InvoicePaymentId { get; set; }

        /// <summary>
        /// the id of the hot wallet to which this payout refers
        /// </summary>
        public Guid InvoicePaymentHotWalletId { get; set; }

        /// <summary>
        /// the timestamp of when this payout was created (or scheduled)
        /// </summary>
        public DateTimeOffset Created { get; set; }

        /// <summary>
        /// the timestamp of when this payout was sent (e.g. broadcast on the blockchain)
        /// </summary>
        public DateTimeOffset? Sent { get; set; }

        /// <summary>
        /// the timestamp of when this payout was confirmed (e.g. on the blockchain)
        /// </summary>
        public DateTimeOffset? Confirmed { get; set; }

        /// <summary>
        /// The timestamp of when this payout was failed.
        /// </summary>
        public DateTimeOffset? Failed { get; set; }

        /// <summary>
        /// the merchant wallet to which the payout was sent (if not sent to an external payout address)
        /// </summary>
        public Guid? MerchantPayoutWalletId { get; set; }

        /// <summary>
        /// the merchants payment output address to send <see cref="PayoutAmountToMerchant"/> to
        /// </summary>
        public string? MerchantPayoutAddress { get; set; }

        /// <summary>
        /// the amount to be paid out to merchant wallets (subtotal minus service and network fees)
        /// </summary>
        public decimal PayoutAmountToMerchant { get; set; }

        /// <summary>
        /// the blockchain transaction id in which this payout was broadcast
        /// </summary>
        public string? BlockchainTxId { get; set; }

        /// <summary>
        /// The blockchain txId for to external + Conversion 
        /// </summary>
        public string? ReceivedBlockchainTxId { get; set; }

        /// <summary>
        /// the current state of the payout
        /// </summary>
        public PaymentPayoutModelState State { get; set; }

        public Guid? BatchId { get; set; }

        /// <summary>
        /// Payout destination amount from completed wallet transaction.
        /// </summary>
        public decimal? DestinationAmount { get; set; }

        /// <summary>
        /// the amount to be paid out to CoinPayments for service fees collected from buyers.
        /// This does not include the <see cref="PayoutAmountToCoinPaymentsDueToOverpayment"/> amount.
        /// </summary>
        public decimal PayoutAmountToCoinPaymentsForBuyerServiceFees { get; set; }

        /// <summary>
        /// the amount to be paid out to CoinPayments wallets as a result of the buyer overpaying but by an
        /// amount so small that it wasn't worth it to send it back to the buyer (less than network fees)
        /// </summary>
        public decimal PayoutAmountToCoinPaymentsDueToOverpayment { get; set; }

        /// <summary>
        /// the hot wallet to which this payout refers
        /// </summary>
        public required InvoicePaymentHotWalletModel InvoicePaymentHotWallet { get; set; }

        public Currency MerchantPayoutWalletCurrency { get; set; }

        public Guid? TransactionId { get; set; }
    }

    public enum PaymentPayoutModelState
    {
        /// <summary>
        /// the payout has been created and scheduled
        /// </summary>
        Scheduled = 1,

        /// <summary>
        /// the payout is in the process of being sent
        /// </summary>
        Sending = 2,

        /// <summary>
        /// the payout has been sent
        /// </summary>
        Sent = 3,

        /// <summary>
        /// the payout has been confirmed
        /// </summary>
        Confirmed = 4,

        /// <summary>
        /// The payout has been sent to conversion
        /// </summary>
        WaitingConversion = 5,

        /// <summary>
        /// Payout is failed
        /// </summary>
        Failed = 6,

        WaitingInternalReceive = 7,

        WaitingExternalConfirm = 8
    }
}
