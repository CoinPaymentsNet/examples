using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsolidationApp.Models.Invoice
{
    public class InvoicePaymentHotWalletAssignmentModel
    {

        /// <summary>
        /// the timestamp from which the pooled hot wallet is assigned to an invoice payment
        /// </summary>
        public required DateTimeOffset AssignedFrom { get; set; }

        /// <summary>
        /// the timestamp to which the pooled hot wallet is assigned to an invoice payment
        /// </summary>
        public required DateTimeOffset AssignedUntil { get; set; }

        public DateTimeOffset? CompletedDate { get; set; }
    }
}
