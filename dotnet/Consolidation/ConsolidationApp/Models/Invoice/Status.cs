using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsolidationApp.Models.Invoice
{
    public enum InvoiceStatus
    {
        /// <summary>
        /// the invoice has been partially or fully filled out but never sent or scheduled
        /// </summary>
        Draft,

        /// <summary>
        /// the invoice has been created for a future date; this also includes recurring billing
        /// </summary>
        Scheduled,

        /// <summary>
        /// the invoice is active (ie. it can be paid) but no payments have been seen on the blockchain
        /// </summary>
        Unpaid,

        /// <summary>
        /// the invoice has been partially or completely paid on the blockchain but the payments have not yet been confirmed
        /// </summary>
        Pending,

        /// <summary>
        /// the invoice payments, with enough funds to cover the invoice balance, have been confirmed on the blockchain but not yet transferred to merchant's wallet(s)
        /// </summary>
        Paid,

        /// <summary>
        /// the invoice payments, minus network and service fees, have been transferred to the merchants wallet(s)
        /// </summary>
        Completed,

        /// <summary>
        /// the invoice has been cancelled and can no longer receive payments
        /// </summary>
        Cancelled,

        /// <summary>
        /// the checkout payment invoice was not paid within the time limit and can no longer receive payments
        /// </summary>
        TimedOut,

        /// <summary>
        /// the invoice has been deleted and can no longer receive payments
        /// </summary>
        Deleted,
    }
public enum InvoiceState
    {
        /// <summary>
        /// the invoice was in draft state and has been deleted
        /// </summary>
        Deleted = -3,

        /// <summary>
        /// the invoice has been partially or fully filled out but never sent or scheduled
        /// </summary>
        Draft = -2,

        /// <summary>
        /// the invoice has been created for a future date; this also includes recurring billing
        /// </summary>
        Scheduled = -1,

        /// <summary>
        /// the invoice is active (ie. it can be paid) but no payments have been seen on the blockchain
        /// </summary>
        Unpaid = 1,

        /// <summary>
        /// the invoice has been partially or completely paid but the payments have not yet been confirmed
        /// </summary>
        Pending = 2,

        /// <summary>
        /// the invoice payments have been confirmed and enough funds were received to cover the invoice amount
        /// </summary>
        Paid = 3,

        /// <summary>
        /// the invoice has been scheduled for payout to the merchant
        /// </summary>
        ScheduledForPayout = 4,

        /// <summary>
        /// the invoice payments have been transferred to the merchants wallets and any refunds have been sent
        /// </summary>
        Completed = 5,

        /// <summary>
        /// the invoice is in the process of being cancelled, cancelled by the user
        /// </summary>
        Cancelling = 9,

        /// <summary>
        /// the invoice was cancelled or was not paid within the time limit and can no longer receive payments
        /// </summary>
        Cancelled = 10,

        /// <summary>
        /// the invoice was timed out.
        /// </summary>
        TimedOut = 12
    }
}
