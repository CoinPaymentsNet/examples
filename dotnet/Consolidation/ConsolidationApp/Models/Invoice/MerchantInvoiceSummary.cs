using ConsolidationApp.Models.Money;
using ConsolidationApp.Models.Wallet;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ConsolidationApp.Models.Invoice
{

    public class MerchantInvoiceSummariesDto : PagedItemsDto<MerchantInvoiceSummaryDto>
    {
    }

    /// <summary>
    /// Summary for a merchant invoice
    /// </summary>
    public class MerchantInvoiceSummaryDto
    {
        private string? _invoiceId;

        /// <summary>
        /// the CoinPayments id for the invoice
        /// </summary>
        public required string Id { get; set; }

        /// <summary>
        /// the optional API caller provided external invoice number.  Appears in screens shown to the Buyer and emails sent.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? InvoiceId
        {
            get => !string.IsNullOrEmpty(InvoiceIdSuffix) ? $"{_invoiceId}-{InvoiceIdSuffix}" : _invoiceId;
            set => _invoiceId = value;
        }

        /// <summary>
        /// the optional numeric suffix for the <see cref="InvoiceId"/>. Used when the invoice is emailed to multiple customers
        /// </summary>
        [JsonIgnore]
        public string? InvoiceIdSuffix { get; init; }

        /// <summary>
        /// the timestamp when the invoice entity was created
        /// </summary>
        public DateTimeOffset Created { get; set; }

        /// <summary>
        /// the date of the invoice, either the system created date or custom date specified by the merchant
        /// </summary>
        public DateTimeOffset InvoiceDate { get; set; }

        /// <summary>
        /// optional due date of the invoice
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public DateTimeOffset? DueDate { get; set; }

        /// <summary>
        /// the timestamp when the invoice was confirmed (paid)
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public DateTimeOffset? Confirmed { get; set; }

        /// <summary>
        /// the timestamp when the invoice was completed (paid out to the merchant and CoinPayments fees)
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public DateTimeOffset? Completed { get; set; }

        /// <summary>
        /// the timestamp when the invoice was manually cancelled. Applicable for payment invoices only
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public DateTimeOffset? Cancelled { get; set; }

        /// <summary>
        /// the currency the invoice is in
        /// </summary>
        public InvoiceCurrencyDto? Currency { get; set; }

        /// <summary>
        /// the buyer information
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public BuyerDto? Buyer { get; set; }

        /// <summary>
        /// the purchase description
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? Description { get; set; }

        /// <summary>
        /// the total amount of the invoice
        /// </summary>
        public MerchantInvoiceSummaryMoneyDto? Amount { get; set; }

        /// <summary>
        /// the shipping info 
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public MerchantInvoiceSummaryShippingDto? Shipping { get; set; }

        /// <summary>
        /// the status of the invoice (including payments received and payments confirmed)
        /// </summary>
        public InvoiceStatus Status { get; set; }

        /// <summary>
        /// the invoice metadata, the integration where the invoice was created
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public MerchantInvoiceSummaryMetadataDto? Metadata { get; set; }


        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public MerchantPaymentSummaryDto[]? Payments { get; set; }

        /// <summary>
        /// indicates if invoice was delivered or to be delivered by email
        /// </summary>
        public bool IsEmailDelivery { get; set; }

        /// <summary>
        /// the email delivery options of this invoice
        /// </summary>
        public EmailDeliveryModel? EmailDelivery { get; set; }

        public string? Notes { get; set; }

        public string? NotesToRecipient { get; set; }

        public bool PartialAcceptAvailable { get; set; }

        public bool IsLifeTimeFinished { get; set; }
    }
    public class MerchantInvoiceSummaryMetadataDto
    {
        /// <summary>
        /// the integration from which the invoice was created
        /// </summary>
        public string? Integration { get; set; }
    }
    public class EmailDeliveryModel
    {
        /// <summary>
        /// the To field of an invoice email
        /// </summary>
        public string? To { get; set; }

        /// <summary>
        /// the Cc field of an invoice email
        /// </summary>
        public string? Cc { get; set; }

        /// <summary>
        /// the Bcc field of an invoice email
        /// </summary>
        public string? Bcc { get; set; }

        /// <summary>
        /// Used to determine whether email delivery is used. Will have non null values if email delivery is to be used (empty strings allowed for drafts)
        /// </summary>
        public bool HasNonNullData => To != null || Cc != null || Bcc != null;
    }
    public class MerchantInvoiceSummaryMoneyDto : InvoiceMoneyDto
    {
        public required string? CurrencySymbol { get; set; }
    }
    public class MerchantInvoiceSummaryShippingDto
    {
        /// <summary>
        /// the address of the party to ship the items to
        /// </summary>
        public AddressDto? Address { get; set; }
    }
    public class InvoiceMoneyDto
    {
        private string _value = "0";

        public string CurrencyId { get; set; } = null!;

        public string? DisplayValue { get; set; }

        public string Value
        {
            get => _value;
            set => _value = string.IsNullOrWhiteSpace(value) ? "0" : value;
        }

        public decimal ValueAsDecimal
        {
            get => decimal.Parse(Value, CultureInfo.InvariantCulture);
            set => Value = Math.Floor(value).ToString(CultureInfo.InvariantCulture);
        }
    }

    public sealed class InvoiceCurrencyDto
    {
        /// <summary>
        /// the unique id of the currency on the CoinPayments platform
        /// </summary>
        /// <example>'4' or '4:0xdac17f958d2ee523a2206206994597c13d831ec7'</example>
        public required string Id { get; set; }

        /// <summary>
        /// ticker symbol for the currency.  For fiat currencies this is the three character (ISO-4217) currency code,
        /// and for crypto currencies their multi-character symbol.  
        /// </summary>
        /// <remarks>
        /// For crypto currencies these are not unique and can change with crypto currency rebrands.
        /// </remarks>
        /// <example>BTC</example>
        public required string Symbol { get; set; }

        /// <summary>
        /// the name of the currency, e.g. 'United States Dollar' or 'Bitcoin'
        /// </summary>
        /// <example>Bitcoin</example>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? Name { get; set; }

        /// <summary>
        /// the token details if this currency represents an ERC20 or similar token
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public InvoiceCurrencyTokenDto? Token { get; set; }

        /// <summary>
        /// the logo urls for the currency
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public InvoiceCurrencyLogoDto? Logo { get; set; }

        public bool ShouldSerializeLogo() =>
            !string.IsNullOrEmpty(Logo?.ImageUrl) || !string.IsNullOrEmpty(Logo?.VectorUrl);

        /// <summary>
        /// the number of digits after the decimal separator, e.g. 2 for USD, 8 for BTC, 0 for JPY
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int? DecimalPlaces { get; set; }
    }
    public sealed class InvoiceCurrencyTokenDto
    {
        /// <summary>
        /// name for the token, if available
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public required string Name { get; set; }

        /// <summary>
        /// ticker symbol for the token, if available
        /// </summary>
        /// <example>USD₮</example>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public required string Symbol { get; set; }

        /// <summary>
        /// the address of the contract
        /// </summary>
        public required string ContractAddress { get; set; }

        /// <summary>
        /// the number of digits after the decimal separator, e.g. 8 for BTC, 6 for USD₮ on ERC-20
        /// </summary>
        public int DecimalPlaces { get; set; }
    }
    public class InvoiceCurrencyLogoDto
    {
        /// <summary>
        /// Link to a CoinPayments hosted image for a currency, 64x64 is the default size returned.
        /// Replace "64x64" in the image url with these alternative sizes: 32, 64, 128, 200.
        /// </summary>
        public string? ImageUrl { get; set; }

        /// <summary>
        /// If available then the link to a CoinPayments hosted vector image (SVG) for the currency.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? VectorUrl { get; set; }
    }

    public sealed class BuyerDto
    {
        /// <summary>
        /// the name of the buyer's company
        /// </summary>
        public string? CompanyName { get; set; }

        /// <summary>
        /// the name of the buyer
        /// </summary>
        public FullNameDto? Name { get; set; }

        /// <summary>
        /// the email address of the buyer
        /// </summary>
        /// <example>customer@example.com</example>
        [EmailAddress]
        public string? EmailAddress { get; set; }

        /// <summary>
        /// the phone number
        /// </summary>
        [RegularExpression(@"^[.()\+\-0-9 ]*$", MatchTimeoutInMilliseconds = 1000)]
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// the buyer's address
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public AddressDto? Address { get; set; }

        public bool HasData => CompanyName != null || Name?.FirstName != null || Name?.LastName != null ||
                                EmailAddress != null || PhoneNumber != null || Address != null;
    }

    public sealed class FullNameDto
    {
        /// <summary>
        /// the given, or first, name
        /// </summary>
        [StringLength(maximumLength: 140)]
        public string? FirstName { get; set; }

        /// <summary>
        /// the surname or family name.  Required when the party is a person.
        /// </summary>
        [StringLength(maximumLength: 140)]
        public string? LastName { get; set; }
    }

    public sealed class AddressDto
    {
        /// <summary>
        /// the first line of the address. For example, number or street.
        /// </summary>
        /// <example>123 Fake street</example>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault), StringLength(maximumLength: 300)]
        public string? Address1 { get; set; }

        /// <summary>
        /// the second line of the address. For example, suite or apartment number.
        /// </summary>
        /// <example>Apartment 42</example>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault), StringLength(maximumLength: 300)]
        public string? Address2 { get; set; }

        /// <summary>
        /// the third line of the address, if needed. For example, a street complement for Brazil, direction text 
        /// such as 'next to Walmart', or a landmark in an Indian address.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault), StringLength(maximumLength: 300)]
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
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault), StringLength(maximumLength: 300)]
        public string? ProvinceOrState { get; set; }

        /// <summary>
        /// the city, town or village
        /// </summary>
        /// <example>Vancouver</example>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault), StringLength(maximumLength: 120)]
        public string? City { get; set; }

        /// <summary>
        /// the neighborhood, suburb or district.
        ///   - Brazil: Suburb, bairoo, or neighborhood
        ///   - India: Sub-locality or district, Street name information is not always available but a sub-locality
        ///            or district can be a very small area
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault), StringLength(maximumLength: 100)]
        public string? SuburbOrDistrict { get; set; }

        /// <summary>
        /// the two-character IS0-3166-1 country code
        /// </summary>
        /// <example>CA</example>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault), StringLength(maximumLength: 2, MinimumLength = 2), RegularExpression("^([A-Z]{2})$")]
        public string? CountryCode { get; set; }

        /// <summary>
        /// the postal code, which is the zip code or equivalent.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault), StringLength(maximumLength: 60)]
        public string? PostalCode { get; set; }
    }


    public record MerchantPaymentSummaryDto(
        MerchantInvoiceSummaryMoneyDto ExpectedAmount,
        MerchantInvoiceSummaryMoneyDto NativeExpectedAmount,
        MerchantInvoiceSummaryMoneyDto ActualAmount,
        MerchantInvoiceSummaryMoneyDto NativeActualAmount,
        string? PaymentAddress,
        InvoicePaymentSettlementModeErrorModel? ErrorCode,
        PaymentFeesSummaryDto Fees,
        PaymentFeesSummaryDto NativeFees,
        MerchantPayoutSummaryDto? Payout,
        MerchantInvoiceSummaryMoneyDto? NativePayout,
        string? RefundEmail,
        PaymentState State,
        bool IsActive);

    public class MerchantPayoutSummaryDto : MerchantInvoiceSummaryMoneyDto
    {
        public required DateTimeOffset? ScheduledAt { get; set; }

        public required DateTimeOffset? CompletedAt { get; set; }

        public required string? BlockchainTx { get; set; }

        public required Guid? SpendRequestId { get; set; }

        public required string? Address { get; set; }

        public required Guid? WalletId { get; set; }

        public required DateTimeOffset? SentAt { get; set; }

        public required DateTimeOffset? ExpectedExecutionDate { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? ReceivedBlockchainTx { get; set; }
    }

    public class InvoicePayoutDestinationAmountDto
    {
        public required MoneyDto Amount { get; set; }

        public required MoneyDto NativeAmount { get; set; }
    }
    public class InvoicePaymentTransaction
    {
        public string? Hash { get; set; }

        public required MoneyDto Amount { get; set; }

        public ulong? ConversionId { get; set; }
    }

    public class InvoicePayoutDetailsDto
    {
        /// <summary>
        /// the currency (merchant's accepted currency) that will be received
        /// </summary>
        public required CurrencyDto Currency { get; set; }

        /// <summary>
        /// The amount for service fees in the merchant's accepted currency
        /// </summary>
        public required PayoutMerchantFeesDto MerchantFees { get; set; }

        /// <summary>
        /// This is the amount to be finally paid out to the merchant in the merchant's accepted currency
        /// </summary>
        public required InvoiceMoneyDto PayoutAmount { get; set; }

        /// <summary>
        /// The <see cref="PayoutAmount"/> in the invoice currency
        /// </summary>
        public required InvoiceMoneyDto PayoutAmountInInvoiceCurrency { get; set; }

        /// <summary>
        /// the merchants payment output address at the time the hot wallet was created
        /// </summary>
        public required string MerchantPayoutAddress { get; set; }

        /// <summary>
        /// the timestamp of when this payout was created (or scheduled)
        /// </summary>
        public DateTimeOffset Created { get; set; }

        /// <summary>
        /// the timestamp of when this payout was sent (e.g. broadcast on the blockchain)
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public DateTimeOffset? Sent { get; set; }

        /// <summary>
        /// the approximate timestamp of when this payout is expected to be sent (e.g. broadcast on the blockchain)
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public DateTimeOffset? Expected { get; set; }

        /// <summary>
        /// the timestamp of when this payout was confirmed (e.g. on the blockchain)
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public DateTimeOffset? Confirmed { get; set; }

        /// <summary>
        /// the current state of the payout
        /// </summary>
        public PaymentPayoutModelState State { get; set; }
    }
    public class PayoutMerchantFeesDto
    {
        /// <summary>
        /// The fee charged for the execution and processing of the transaction
        /// </summary>
        public required InvoiceMoneyDto TransactionFee { get; set; }

        /// <summary>
        /// The fee charged for the processing of the transaction on the network
        /// </summary>
        public required InvoiceMoneyDto NetworkFee { get; set; }

        /// <summary>
        /// The fee associated with converting between different currencies or assets
        /// </summary>
        public InvoiceMoneyDto? ConversionFee { get; set; }
    }
    
    public record PaymentFeesSummaryDto(
        MerchantInvoiceSummaryMoneyDto PaymentSubTotal,
        MerchantInvoiceSummaryMoneyDto MerchantMarkupOrDiscount,
        PersonFeeSummaryDto BuyerFee,
        PersonFeeSummaryDto MerchantFee)
    {
        public decimal Gross => PaymentSubTotal.ValueAsDecimal + BuyerFee.Total + MerchantFee.Total;
    }

    public record PersonFeeSummaryDto(
        MerchantInvoiceSummaryMoneyDto CoinPaymentsFee,
        MerchantInvoiceSummaryMoneyDto NetworkFee,
        MerchantInvoiceSummaryMoneyDto ConversionFee)
    {
        public decimal Total => CoinPaymentsFee.ValueAsDecimal + NetworkFee.ValueAsDecimal + ConversionFee.ValueAsDecimal;
    }
    public enum PaymentState
    {
        /// <summary>
        /// active but no payments have been detected
        /// </summary>
        Created = 1,

        /// <summary>
        /// payments detected in the mem-pool
        /// </summary>
        Detected = 2,

        /// <summary>
        /// partially or completely paid but the payments have not yet been confirmed
        /// </summary>
        Pending = 3,

        /// <summary>
        /// payments have been confirmed and enough funds were received to cover the invoice amount
        /// </summary>
        Confirmed = 4,

        /// <summary>
        /// payments have been scheduled for payout to the merchant
        /// </summary>
        ScheduledForPayout = 5,

        /// <summary>
        /// payments have been paid-out to the merchants wallets and any over-payments have claim refunds sent
        /// </summary>
        Completed = 6,

        /// <summary>
        /// payment manually cancelled
        /// </summary>
        CancelledWaitingRefund = 10,

        /// <summary>
        /// payment timed out, was not paid within the time limit and can no longer receive payments. Waiting on refund possibility checks
        /// </summary>
        TimedOutWaitingRefund = 11,

        /// <summary>
        /// payment was timed out and checked on refund possibility
        /// </summary>
        TimedOutRefundProcessed = 12,

        /// <summary>
        /// Cancelled payment was checked on refund possibility
        /// </summary>
        CancelledRefundProcessed = 13,

        /// <summary>
        /// any refunds have been sent
        /// </summary>
        Refunded = 16,
    }

    [Flags]
    public enum InvoicePaymentSettlementModeErrorModel
    {
        Unknown = 0,
        NegativeRate = 1,
        PayoutAddressIsNull = 2,
        PaymentSubTotalIsLessThanMerchantTotalFee = 4,
        TotalBuyerWillPayIsNegativeOrZero = 8,
        TotalBuyerWillPayIsLessThanBuyerNetworkFee = 16,
        TotalMerchantFeeRatioIsMoreThanMaximumRatioSetting = 32,
        PayoutAmountIsLessThanDust = 64,
        CurrencyIsNotActive = 128,
        AmountIsBelowOfConversionLimit = 256,
        AmountIsAboveOfConversionLimit = 512,
        UserLimitIsReached = 1024,
        NotEnoughToActivateRippleAddress = 2048,
        ConversionPairDoesNotExist = 4096,
        AddressIsNotValid = 8_192,
        DoesNotHaveCompletedKyc = 16_384,
        UnstoppableDomainNotFound = 32_768,
        UnstoppableDomainNotFoundForCurrency = 65_536,
        UserWalletIsLocked = 131_072,
        UserWalletIsDeleted = 262_144,
        LargeWithdrawalRejected = 524_288,
    }
}
