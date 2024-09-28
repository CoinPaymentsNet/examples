using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ConsolidationApp.Models.Merchant
{

    public class MerchantOptionsModel
    {
        /// <summary>
        /// for merchant profile fields that can be shown/hidden on an invoice, if field is present in the flag it should be shown on the invoice.
        /// If no flags provided then default is to show email only.
        /// </summary>
        public MerchantFields ShowFieldsFlag { get; set; } = MerchantFields.ShowEmail;

        /// <summary>
        /// miscellaneous information to be displayed on the invoice, such as business hours or other info specific to the invoice
        /// </summary>
        public string? AdditionalInfo { get; init; }
    }

    [Flags]
    public enum MerchantFields
    {
        ShowNone = 0,
        ShowAddress = 0x01,
        ShowEmail = 0x02,
        ShowPhone = 0x04,
        ShowRegistrationNumber = 0x08
    }
    [Flags]
    public enum InvoiceFlags
    {
        None = 0,

        /// <summary>
        /// prompt the buyer for their name and email address
        /// </summary>
        RequireBuyerNameAndEmail = 1,

        /// <summary>
        /// sends a payment completed e-mail notification to the merchant
        /// </summary>
        SendPaymentCompleteEmailNotification = 2,

        /// <summary>
        /// Is pos.
        /// </summary>
        IsPos = 4
    }
    public class InvoiceMerchantOptionsDto
    {
        /// <summary>
        /// Indicates whether the address should be shown on the invoice. Default is don't show if not provided.
        /// </summary>
        public bool ShowAddress { get; set; }

        /// <summary>
        /// Indicates whether the email should be shown on the invoice. Default is show the email if not provided.
        /// </summary>
        public bool ShowEmail { get; set; } = true;

        /// <summary>
        /// Indicates whether the phone should be shown on the invoice. Default is don't show if not provided.
        /// </summary>
        public bool ShowPhone { get; set; }

        /// <summary>
        /// Indicates whether the business registration number should be shown on the invoice. Default is don't show if not provided.
        /// </summary>
        public bool ShowRegistrationNumber { get; set; }

        /// <summary>
        /// Miscellaneous information to be displayed on the invoice, such as business hours or other info specific to the invoice
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? AdditionalInfo { get; set; }
    }
}
