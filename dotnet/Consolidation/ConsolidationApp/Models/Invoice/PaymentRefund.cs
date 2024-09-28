using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsolidationApp.Models.Invoice
{
    public class InvoicePaymentRefundModel
    {
        /// <summary>
        /// the unique id of the invoice hot wallet refund (PK)
        /// </summary>
        public Guid Id { get; set; }

        public Guid InvoicePaymentId { get; set; }

        /// <summary>
        /// the timestamp of when this refund was created
        /// </summary>
        public DateTimeOffset Created { get; set; }

        /// <summary>
        /// the timestamp of when this refund expires and can no longer be claimed (default = 30 days)
        /// </summary>
        public DateTimeOffset Expires { get; set; }

        /// <summary>
        /// the timestamp of when the last email notification was sent
        /// </summary>
        public DateTimeOffset? LastEmailNotificationSent { get; set; }

        /// <summary>
        /// the timestamp of when the next email notification should be sent
        /// </summary>
        public DateTimeOffset NextEmailNotification { get; set; }

        /// <summary>
        /// the timestamp of when this refund was claimed
        /// </summary>
        public DateTimeOffset? Claimed { get; set; }

        /// <summary>
        /// the id of the spend request that created the claiming transaction
        /// </summary>
        public Guid? SpendRequestId { get; set; }

        /// <summary>
        /// the address to which the refund was sent
        /// </summary>
        public string? PayoutAddress { get; set; }

        /// <summary>
        /// the amount of the refund that was paid out (not including fees)
        /// </summary>
        public decimal PayoutAmount { get; set; }

        /// <summary>
        /// the network fees that were deducted from the <see cref="RefundAvailable"/> amount
        /// </summary>
        public decimal PayoutNetworkFees { get; set; }

        /// <summary>
        /// the email address to send refund emails to
        /// </summary>
        public string? RefundEmail { get; set; }

        /// <summary>
        /// the total number of refund emails that have been sent
        /// </summary>
        public int RefundEmailsSent { get; set; }

        /// <summary>
        /// the amount of the refund available (less network fees when paid back)
        /// </summary>
        public decimal RefundAvailable { get; set; }

        /// <summary>
        /// the estimated network fees
        /// </summary>
        public decimal EstimatedNetworkFees { get; set; }

        /// <summary>
        /// the ip address of the refund claimer
        /// </summary>
        public string? ClaimerIpAddress { get; set; }
    }
}
