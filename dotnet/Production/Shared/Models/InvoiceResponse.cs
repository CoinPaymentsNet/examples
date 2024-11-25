using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Shared.Models
{

    public sealed class InvoiceResponseDto
    {
        private string? _invoiceId;

        /// <summary>
        /// the CoinPayments id for the invoice
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// the optional API caller provided external invoice number.  Appears in screens shown to the Buyer and emails sent.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull), StringLength(maximumLength: 127)]
        public string? InvoiceId
        {
            get => !string.IsNullOrEmpty(InvoiceIdSuffix) ? $"{_invoiceId}-{InvoiceIdSuffix}" : _invoiceId;
            set => _invoiceId = value;
        }

        /// <summary>
        /// the optional numeric suffix for the <see cref="InvoiceId"/>. Used when the invoice is emailed to multiple customers
        /// </summary>
        [JsonIgnore]
        public string? InvoiceIdSuffix { get; set; }

        /// <summary>
        /// the timestamp when the invoice was created
        /// </summary>
        public DateTimeOffset Created { get; set; }

        /// <summary>
        /// the date of the invoice, either the system created date or custom date specified by the merchant
        /// </summary>
        public DateTimeOffset InvoiceDate { get; set; }

        /// <summary>
        /// optional due date of the invoice
        /// </summary>
        public DateTimeOffset? DueDate { get; set; }

        /// <summary>
        /// the timestamp when the invoice was confirmed (paid)
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public DateTimeOffset? Confirmed { get; set; }

        /// <summary>
        /// the timestamp when the invoice was completed (paid out to the merchant and CoinPayments fees)
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public DateTimeOffset? Completed { get; set; }

        /// <summary>
        /// the timestamp when the invoice was manually cancelled. Applicable for payment invoices only
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public DateTimeOffset? Cancelled { get; set; }

        /// <summary>
        /// the timestamp when the invoice expires. Applicable for checkout and POS invoices only
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public DateTimeOffset? Expires { get; set; }

        /// <summary>
        /// the currency the invoice is in
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public InvoiceCurrencyDto? Currency { get; set; }

        /// <summary>
        /// the merchant the invoice is for
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public InvoiceMerchantDto? Merchant { get; set; }

        /// <summary>
        /// options to show/hide merchant information on an invoice, or include additional merchant information specific to an invoice
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public InvoiceMerchantOptionsDto? MerchantOptions { get; set; }

        /// <summary>
        /// the buyer information, if not provided it will be requested during payment so that refunds can be sent if
        /// there is a problem with the payment.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public BuyerDto? Buyer { get; set; }

        /// <summary>
        /// the purchase description
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull), StringLength(maximumLength: 200)]
        public string? Description { get; set; }

        /// <summary>
        /// the optional array of items and/or services that a buyer intends to purchase from the merchant
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public LineItemDto[]? Items { get; set; }

        public bool ShouldSerializeItems() => Items != null && Items.Length > 0;

        /// <summary>
        /// the total amount of the invoice, with an optional breakdown that provides details, such as the total item
        /// amount, total tax amount, shipping, handling, insurance and discounts, if any
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public required InvoiceAmountDto Amount { get; set; }

        /// <summary>
        /// the shipping method and address
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public InvoiceShippingDetailDto? Shipping { get; set; }

        /// <summary>
        /// any custom data the caller wishes to attach to the invoice which will be sent back in notifications
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Dictionary<string, string>? CustomData { get; set; }

        /// <summary>
        /// the status of the invoice (including payments received and payments confirmed)
        /// </summary>
        public InvoiceStatus Status { get; set; }

        /// <summary>
        /// flag indicating whether a buyer name and email are required, they will be requested at checkout
        /// if not provider by the caller.  The <see cref="BuyerDataCollectionMessage"/> will be displayed
        /// to the buyer when prompted.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? RequireBuyerNameAndEmail { get; set; }

        /// <summary>
        /// the message to display when collecting buyer user data
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? BuyerDataCollectionMessage { get; set; }

        /// <summary>
        /// notes for the merchant only, these are not visible to the buyers
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Notes { get; set; }

        /// <summary>
        /// any additional information to share with the buyer about the transaction
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? NotesToRecipient { get; set; }

        /// <summary>
        /// any terms and conditions, e.g. a cancellation policy
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? TermsAndConditions { get; set; }

        /// <summary>
        /// the invoice email delivery options if the invoice is to be emailed
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public InvoiceEmailDeliveryOptionsDto? EmailDelivery { get; set; }

        /// <summary>
        /// indicates if invoice was delivered by email or to be delivered by email
        /// </summary>
        public bool IsEmailDelivery { get; set; }

        /// <summary>
        /// the invoice metadata, the integration and host where the invoice was created
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public InvoiceMetadataDto? Metadata { get; set; }

        public string? PONumber { get; set; }

        public InvoicePayoutsDetailsDto? PayoutDetails { get; set; }

        public MerchantPaymentSummaryDto[] Payments { get; set; } = Array.Empty<MerchantPaymentSummaryDto>();

        /// <summary>
        /// Invoice in finished state more than 90 days
        /// </summary>
        public bool IsLifeTimeFinished { get; set; }

        public bool ShouldSerializeEmailDelivery() =>
            !string.IsNullOrEmpty(EmailDelivery?.To) ||
            !string.IsNullOrEmpty(EmailDelivery?.Cc) ||
            !string.IsNullOrEmpty(EmailDelivery?.Bcc);

        public bool ShouldSerializeMetadata() =>
            !string.IsNullOrEmpty(Metadata?.Integration) ||
            !string.IsNullOrEmpty(Metadata?.Hostname);
    }

    public class BuyerModel
    {
        /// <summary>
        /// the company name of the buyer
        /// </summary>
        public string? CompanyName { get; set; }

        /// <summary>
        /// the buyer's first name
        /// </summary>
        public string? FirstName { get; set; }

        /// <summary>
        /// the buyer's last name
        /// </summary>
        public string? LastName { get; set; }

        /// <summary>
        /// the buyer's email
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// the buyer's phone number
        /// </summary>
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// the buyer's address
        /// </summary>
        public AddressModel? Address { get; set; }

        public bool HasData => CompanyName != null || FirstName != null || LastName != null || Email != null ||
                               PhoneNumber != null || Address is { HasData: true };
    }
    public class AddressModel
    {

        /// <summary>
        /// the first line of the address. For example, number or street.
        /// </summary>
        /// <example>123 Fake street</example>
        public string? Address1 { get; set; }

        /// <summary>
        /// the second line of the address. For example, suite or apartment number.
        /// </summary>
        /// <example>Apartment 42</example>
        public string? Address2 { get; set; }

        /// <summary>
        /// the third line of the address, if needed. For example, a street complement for Brazil, direction text 
        /// such as 'next to Walmart', or a landmark in an Indian address.
        /// </summary>
        public string? Address3 { get; set; }

        /// <summary>
        /// the highest level sub-division in a country, which is usually a province, state, or ISO-3166-2 subdivision.
        ///
        /// Format for postal delivery. For example, `CA` instead of `California`
        /// 
        ///  - UK: county
        ///  - US: state
        ///  - Canada: province
        ///  - Japan: prefecture
        ///  - Switzerland: kanton
        /// </summary>
        /// <example>BC</example>
        public string? ProvinceOrState { get; set; }

        /// <summary>
        /// the city, town or village
        /// </summary>
        /// <example>Vancouver</example>
        public string? City { get; set; }

        /// <summary>
        /// the neighborhood, suburb or district.
        ///   - Brazil: Suburb, bairoo, or neighborhood
        ///   - India: Sub-locality or district, Street name information is not always available but a sub-locality
        ///            or district can be a very small area
        /// </summary>
        public string? SuburbOrDistrict { get; set; }

        /// <summary>
        /// the two-character IS0-3166-1 country code
        /// </summary>
        /// <example>CA</example>
        public string? CountryCode { get; set; }

        /// <summary>
        /// the postal code, which is the zip code or equivalent.
        /// </summary>
        public string? PostalCode { get; set; }

        /// <summary>
        /// value indicating whether any data has been set on the object
        /// </summary>
        public bool HasData =>
            Address1 != null || Address2 != null || Address3 != null || ProvinceOrState != null ||
            City != null || SuburbOrDistrict != null || CountryCode != null || PostalCode != null;
    }
    public class ShippingModel
    {

        /// <summary>
        /// the shipping method
        /// </summary>
        public string? Method { get; set; }

        /// <summary>
        /// the company name of the party to ship the items to
        /// </summary>
        public string? CompanyName { get; set; }

        /// <summary>
        /// the first name of the party to ship the items to
        /// </summary>
        public string? FirstName { get; set; }

        /// <summary>
        /// the last name of the party to ship the items to
        /// </summary>
        public string? LastName { get; set; }

        /// <summary>
        /// the email of the party to ship the items to
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// the phone number of the party to ship the items to
        /// </summary>
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// the address of the party to ship the items to
        /// </summary>
        public AddressModel? Address { get; set; }

        /// <summary>
        /// value indicating whether any data has been set on the object
        /// </summary>
        public bool HasData => Method != null || CompanyName != null || FirstName != null || LastName != null
                               || Email != null || PhoneNumber != null || Address is { HasData: true };
    }
    public sealed class InvoiceShippingDetailDto
    {
        /// <summary>
        /// the shipping method
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault), StringLength(maximumLength: 127)]
        public string? Method { get; set; }

        /// <summary>
        /// the company name of the party to ship the items to
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? CompanyName { get; set; }

        /// <summary>
        /// the name of the party to ship the items to
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public FullNameDto? Name { get; set; }

        /// <summary>
        /// the email address of the party to ship the items to
        /// </summary>
        /// <example>customer@example.com</example>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault), EmailAddress]
        public string? EmailAddress { get; set; }

        /// <summary>
        /// the phone number
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault), RegularExpression(@"^[.()\+\-0-9 ]*$", MatchTimeoutInMilliseconds = 1000)]
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// the address of the party to ship the items to
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public AddressDto? Address { get; set; }

        public bool HasData => Method != null || Name?.FirstName != null || Name?.LastName != null ||
                               CompanyName != null || EmailAddress != null || PhoneNumber != null || Address != null;
    }

    public class InvoiceEmailDeliveryOptionsDto
    {
        /// <summary>
        /// the email `to` field, multiple addresses separated by semicolons
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? To { get; set; }

        /// <summary>
        /// the email `cc` field, multiple addresses separated by semicolons
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? Cc { get; set; }

        /// <summary>
        /// the email `bcc` field, multiple addresses separated by semicolons
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? Bcc { get; set; }
    }

    public sealed class InvoiceMetadataDto
    {
        /// <summary>
        /// the integration from which the invoice was created
        /// </summary>
        public string? Integration { get; set; }

        /// <summary>
        /// the hostname on which the invoice was created
        /// </summary>
        public string? Hostname { get; set; }
    }

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

    public class LineItemModel
    {
        public Guid InvoiceId { get; set; }

        /// <summary>
        /// the unique id of the line item (PK)
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// the API caller provided external ID for the item.  Appears on the Merchant dashboard and reports only.
        /// </summary>
        public string? CustomId { get; set; }

        /// <summary>
        /// the stock keeping unit (SKU) of the item
        /// </summary>
        public string? SKU { get; set; }

        /// <summary>
        /// the name or title of the item
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// the detailed description of the item
        /// </summary>
        public string? Description { get; set; }

        public QuantityModel? Quantity { get; set; }

        public LineItemType Type { get; set; }

        /// <summary>
        /// the subtotal price of the item
        /// </summary>
        public AmountModel? Amount { get; set; }

        /// <summary>
        /// the original total price of the item if <see cref="Amount"/> represents a discounted price
        /// </summary>
        public AmountModel? OriginalAmount { get; set; }

        /// <summary>
        /// the total taxes charged on this item
        /// </summary>
        public decimal? Tax { get; set; }
    }
    public enum LineItemType
    {
        Hours = 1,
        Quantity = 2
    }
    public class QuantityModel
    {
        public int Value { get; set; }
        public string? Type { get; set; }
    }

    public class AmountModel
    {
        public string CurrencyId { get; set; }
        public string DisplayValue { get; set; }
        public string Value { get; set; }
    }
    public sealed class LineItemDto
    {
        /// <summary>
        /// the API caller provided external ID for the item.  Appears on the Merchant dashboard and reports only.
        /// </summary>
        [StringLength(127)]
        public string? CustomId { get; set; }

        /// <summary>
        /// the stock keeping unit (SKU) of the item
        /// </summary>
        [StringLength(127)]
        public string? SKU { get; set; }

        /// <summary>
        /// the name or title of the item
        /// </summary>
        [Required, StringLength(127)]
        public string? Name { get; set; }

        /// <summary>
        /// the detailed description of the item
        /// </summary>
        [StringLength(1000)]
        public string? Description { get; set; }

        /// <summary>
        /// the quantity of the item.  Must be greater than 0 and less than 999,999,999‬
        /// </summary>
        [Required]
        public required LineItemQuantityDto Quantity { get; set; }

        /// <summary>
        /// the original total price of the item if <see cref="Amount"/> represents a discounted price
        /// </summary>
        /// <remarks>
        /// the UI can display the discounted price on the cart / checkout screens for this item
        /// </remarks>
        public InvoiceMoneyDto? OriginalAmount { get; set; }

        /// <summary>
        /// the subtotal price of the item (note: this is not a per unit price but the total price for the total quantity)
        /// </summary>
        public required InvoiceMoneyDto Amount { get; set; }

        /// <summary>
        /// the total taxes charged on this item
        /// </summary>     
        public InvoiceMoneyDto? Tax { get; set; }

        /// <summary>
        /// Calculates the line item total (base amount + taxes + charges - discounts)
        /// </summary>
        public bool TryGetTotal(out decimal total)
        {
            if (!decimal.TryParse(Amount.Value, out total))
            {
                return false;
            }

            if (Tax != null)
            {
                if (!decimal.TryParse(Tax.Value, out var taxAmount))
                {
                    return false;
                }
                total += taxAmount;
            }

            return true;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            IEnumerable<ValidationResult> GetNotSameCurrencyValidationResults(string expectedCurrencyId, params InvoiceMoneyDto?[] moneys)
            {
                foreach (var money in moneys)
                {
                    if (money == null)
                    {
                        continue;
                    }

                    if (money.CurrencyId != expectedCurrencyId)
                    {
                        yield return new ValidationResult($"All item monetary fields (charges, discounts, taxes) must have the same currency as the items base amount, expected currency '{expectedCurrencyId}' but found '{money.CurrencyId}'");
                    }
                }
            }

            foreach (var result in GetNotSameCurrencyValidationResults(Amount.CurrencyId, OriginalAmount, Tax))
            {
                yield return result;
            }
        }
    }
    public class LineItemQuantityDto : IValidatableObject
    {
        /// <summary>
        /// the quantity of the item.  Must be greater than 0 and less than 999,999,999‬.
        /// defaults to 1 if not provided.
        /// </summary>
        [Range(1, 999_999_999)]
        public int Value { get; set; } = 1;

        public LineItemQuantityTypeDto Type { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!Enum.GetValues<LineItemQuantityTypeDto>().Contains(Type))
            {
                var allowedValues = string.Join(", ", Enum.GetValues<LineItemQuantityTypeDto>().Select(x => $"{(int)x} ({x.ToString()})"));
                yield return new ValidationResult($"Wrong value {Type} for item type. Allowed values are: {allowedValues}", [nameof(Type)]);
            }
        }
    }

    public enum LineItemQuantityTypeDto
    {
        Hours = 1,
        Quantity = 2
    }
    public sealed class InvoiceAmountDto : InvoiceMoneyDto
    {
        /// <summary>
        /// the breakdown of the amount, providing details such as total item amount, total tax amount, shipping, 
        /// handling, insurance, and discounts, if any.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public InvoiceAmountBreakdownDto? Breakdown { get; set; }
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
