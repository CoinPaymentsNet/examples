using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsolidationApp.Models.Invoice
{

    public class InvoicePaymentModel
    {
        /// <summary>
        /// the unique id of the invoice (PK)
        /// </summary>
        public Guid Id { get; init; }

        /// <summary>
        /// the timestamp when the payment attempt was created
        /// </summary>
        public DateTimeOffset Created { get; init; }

        /// <summary>
        /// the timestamp when the payment attempt expires, payments can't be made within this attempt past this time
        /// </summary>
        public DateTimeOffset Expires { get; init; }

        /// <summary>
        /// the timestamp of when the payment was manually cancelled (invoice cancelled)
        /// </summary>
        public DateTimeOffset? Cancelled { get; init; }

        /// <summary>
        /// the timestamp of when the first payments were detected
        /// </summary>
        public DateTimeOffset? Detected { get; init; }

        /// <summary>
        /// the timestamp of when the full payment amount was detected
        /// </summary>
        public DateTimeOffset? Pending { get; init; }

        /// <summary>
        /// the timestamp of when the payment was fully paid and confirmed
        /// </summary>
        public DateTimeOffset? Confirmed { get; init; }

        /// <summary>
        /// the timestamp of when the payment was completed, paid out and overage refund claims sent
        /// </summary>
        public DateTimeOffset? Completed { get; init; }

        /// <summary>
        /// the timestamp of when the payment was scheduled for payout
        /// </summary>
        public DateTimeOffset? ScheduledForPayout { get; init; }

        /// <summary>
        /// the state of the invoice payment
        /// </summary>
        public PaymentState State { get; init; }

        /// <summary>
        /// the timestamp of when the payment was refunded
        /// </summary>
        public DateTimeOffset? Refunded { get; init; }

        /// <summary>
        /// the e-mail address to send refund instructions to, in case of payment failures
        /// </summary>
        public string? RefundEmail { get; init; }

        public Guid InvoiceId { get; init; }

        public bool IsGuest { get; init; }

        public required InvoicePaymentHotWalletModel? HotWallet { get; set; }

        public required InvoicePaymentPayoutModel? Payout { get; init; }

        public required InvoicePaymentRefundModel? Refund { get; init; }

        public required bool IsActive { get; set; }
    }

}
