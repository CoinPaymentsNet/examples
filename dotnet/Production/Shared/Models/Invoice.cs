using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Shared.Models
{

    public record CreateMerchantInvoiceResponse(InvoicesCreated[] Invoices);

    public record InvoicesCreated(string Id, string Link);

    public record CreateMerchantInvoiceRequest(
        bool IsEmailDelivery,
        MerchantInvoiceEmailDeliveryOptions EmailDelivery,
        DateTimeOffset? DueDate,
        DateTimeOffset? InvoiceDate,
        bool? Draft,
        string? ClientId,
        string? InvoiceId,
        Buyer? Buyer,
        string? Description,
        MerchantInvoiceLineItem[] Items,
        InvoiceAmount Amount,
        bool? RequireBuyerNameAndEmail,
        string? BuyerDataCollectionMessage,
        string? Notes,
        string? NotesToRecipient,
        string? TermsAndConditions,
        InvoiceMerchantOptions MerchantOptions,
        Dictionary<string, string>? CustomData,
        string? PONumber,
        InvoiceWebhook[] Webhooks,
        InvoicePayoutConfigDto? PayoutConfig);
    public record InvoicePayoutConfigDto(
        int CurrencyId,
        string? ContractAddress,
        string Address,
        PayoutCurrencyFrequency Frequency);


    public record InvoiceMerchantDto(Guid Id);


    public enum PayoutCurrencyFrequency
    {
        /// <summary>
        /// normal payout frequency, could be daily
        /// </summary>
        Normal = 1,

        /// <summary>
        /// payout as soon as possible
        /// </summary>
        AsSoonAsPossible = 2,

        /// <summary>
        /// payout every hour at HH:00:00 (0 minutes, 0 seconds)
        /// </summary>
        Hourly = 3,

        /// <summary>
        /// payout every day at 00:00:00 (midnight)
        /// </summary>
        Nightly = 4,

        /// <summary>
        /// payout every week at 00:00:00 (midnight) on Monday
        /// </summary>
        Weekly = 5
    }

    public enum MerchantClientWebhookNotification
    {
        invoiceCreated,
        invoicePending,
        invoicePaid,
        invoiceCompleted,
        invoiceCancelled
    }

    public record InvoiceMerchantOptions(
        bool ShowAddress,
        bool ShowEmail,
        bool ShowPhone,
        bool ShowRegistrationNumber,
        string? AdditionalInfo);

    public record InvoiceAmount(
        [property: JsonPropertyName("currencyId")]
    string CurrencyId,
        string? ContractAddress,
        string? DisplayValue,
        long Value,
        InvoiceAmountBreakdownDto? Breakdown);

    public record InvoiceAmountBreakdownDto(
        InvoiceMoney Subtotal,
        InvoiceMoney Shipping,
        InvoiceMoney Handling,
        InvoiceMoney TaxTotal,
        InvoiceMoney Discount);

    public record MerchantInvoiceLineItem(
        string? CustomId,
        string? SKU,
        string Name,
        string? Description,
        MerchantInvoiceLineItemQuantity Quantity,
        InvoiceMoney OriginalAmount,
        InvoiceMoney Amount,
        InvoiceMoney? Tax);

    public record InvoiceMoney(
        [property: JsonPropertyName("currencyId")]
    string CurrencyId,
        string? ContractAddress,
        string? DisplayValue,
        [property: JsonConverter(typeof(Utilites.NumericConverter<int>))]
    int Value);

    public record MerchantInvoiceLineItemQuantity(
        int Value,
        int Type);

    public record MerchantInvoiceEmailDeliveryOptions(
        string To,
        string? Cc,
        string? Bcc);

    public record Buyer(
        string CompanyName,
        FullName Name,
        string EmailAddress,
        string PhoneNumber,
        Address Address);

    public record Address(
        string Address1,
        string Address2,
        string Address3,
        string ProvinceOrState,
        string City,
        string SuburbOrDistrict,
        string CountryCode,
        string PostalCode);

    public record FullName(
        string FirstName,
        string LastName);

    public record CreateInvoicePaymentRequest(string RefundEmail);

    public sealed class InvoicePaymentDto
    {
        public string PaymentId { get; set; }
        public DateTimeOffset Expires { get; set; }
        public InvoicePaymentCurrencyDto[] PaymentCurrencies { get; set; }
    }

    public sealed class InvoicePaymentCurrencyDto
    {
        public CurrencyDto Currency { get; set; }
        public bool IsDisabled { get; set; }
        public InvoicePaymentCurrencyAmountDueDto Amount { get; set; }
        public InvoicePaymentCurrencyAmountDueDto NativePreferredAmount { get; set; }
        public MoneyDto RemainingAmount { get; set; } = null!;
    }

    public sealed class InvoicePaymentCurrencyAmountDueDto : MoneyDto
    {
        public decimal Rate { get; set; }
    }

    public sealed class InvoicePaymentCurrencyPaymentDetailsDto
    {
        public CurrencyDto Currency { get; set; }
        public InvoicePaymentCurrencyAmountDueDto Amount { get; set; }
        public InvoicePaymentCurrencyPaymentAddressesDto Addresses { get; set; }
    }

    public record ClaimRefundResponseDto(
        string PayoutAddress,
        MoneyDto PayoutAmount,
        MoneyDto PayoutNetworkFees);

    public class InvoicePaymentCurrencyPaymentAddressesDto
    {
        public string Address { get; set; }
    }

    public class MerchantClientWebhookEndpointsDto : PagedItemsDto<MerchantClientWebhookEndpointDto>
    {
    }

    public class MerchantClientWebhookEndpointDto
    {
        /// <summary>
        /// the id of the notifications endpoint
        /// </summary>
        public required string Id { get; set; }

        /// <summary>
        /// the url to which to POST webhook notifications to
        /// </summary>
        public required string NotificationsUrl { get; set; }

        /// <summary>
        /// the types of notifications to send to this endpoint
        /// </summary>
        public required MerchantClientWebhookNotification[] Notifications { get; set; }
    }

    public class CreateMerchantClientWebhookResponseDto
    {
        public required string Id { get; set; }
    }

    public class CreateMerchantClientWebhookRequestDto
    {
        public required string NotificationsUrl { get; set; }

        public required MerchantClientWebhookNotification[] Notifications { get; set; }
    }

    public class UpdateMerchantClientWebhookDto
    {
        /// <summary>
        /// the url to which to POST webhook notifications to
        /// </summary>
        [Url]
        public string? NotificationsUrl { get; set; }

        /// <summary>
        /// the types of notifications to send to this endpoint
        /// </summary>
        public required MerchantClientWebhookNotification[] Notifications { get; set; }
    }

    public record InvoiceStatusDtoResponseDto(
        DateTimeOffset Created,
        DateTimeOffset? Expires,
        InvoiceStatus Status,
        InvoicePaymentStatusDto Payment,
        bool PartialAcceptAvailable);

    public record InvoicePaymentStatusDto(
        int CurrencyId,
        int Confirmations,
        int RequiredConfirmations,
        MoneyDto ConfirmedAmount,
        MoneyDto UnconfirmedAmount);

    public record RefundApiDto(
        DateTimeOffset Expires,
        CurrencyDto Currency,
        bool IsClaimed,
        MoneyDto RefundAvailable,
        MoneyDto EstimatedNetworkFees,
        CurrencyDto NetworkFeeCurrency);

    public class CreateInvociePaymentCurrencyDto
    {
        [JsonPropertyName("currency_ranks")] public CurrencyRank[] CurrencyRanks { get; set; } = null!;
    }

    public record CurrencyRank(
        [property: JsonPropertyName("currency_id")]
    int CurrencyId,
        [property: JsonPropertyName("rank")] int Rank,
        [property: JsonPropertyName("smart_contract")]
    string? SmartContract,
        [property: JsonPropertyName("wallet_id")]
    Guid WalletId);

    public class InvoiceDtoResponse
    {
        [JsonPropertyName("id")] public required Guid Id { get; set; }

        [JsonPropertyName("merchant_id")] public required Guid MerchantId { get; set; }

        [JsonPropertyName("user_id")] public required Guid UserId { get; set; }

        [JsonPropertyName("payments")] public required PaymentDto[] Payments { get; set; } = Array.Empty<PaymentDto>();
    }

    public record PaymentDto(
        [property: JsonPropertyName("id")] Guid Id,
        [property: JsonPropertyName("invoice_id")]
    Guid InvoiceId,
        [property: JsonPropertyName("hot_wallet")]
    PaymentHotWalletDto? HotWallet,
        [property: JsonPropertyName("payout")] PayoutDto? Payout,
        [property: JsonPropertyName("refund")] RefundDto? Refund);

    public record PaymentHotWalletDto([property: JsonPropertyName("error")] SettlementErrorDto? Error);

    public enum InvoicePaymentSettlementModeErrorDto
    {
        Unknown = -1,
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
        UserWalletIsLocked = 131_072
    }

    public record SettlementErrorDto(
        [property: JsonPropertyName("modes")] InvoicePaymentSettlementModeErrorDto[] Modes,
        [property: JsonPropertyName("message")]
    string? Message);

    public record RefundDto(
        [property: JsonPropertyName("id")] Guid Id,
        [property: JsonPropertyName("invoice_payment_id")]
    Guid InvoicePaymentId,
        [property: JsonPropertyName("created")]
    DateTimeOffset Created,
        [property: JsonPropertyName("expires")]
    DateTimeOffset Expires,
        [property: JsonPropertyName("last_email_notification_sent")]
    DateTimeOffset? LastEmailNotificationSent,
        [property: JsonPropertyName("next_email_notification")]
    DateTimeOffset NextEmailNotification,
        [property: JsonPropertyName("claimed")]
    DateTimeOffset? Claimed,
        [property: JsonPropertyName("payout_address")]
    string? PayoutAddress,
        [property: JsonPropertyName("payout_amount")]
    decimal PayoutAmount,
        [property: JsonPropertyName("payout_network_fees")]
    decimal PayoutNetworkFees,
        [property: JsonPropertyName("refund_email")]
    string? RefundEmail,
        [property: JsonPropertyName("refund_emails_sent")]
    int RefundEmailsSent,
        [property: JsonPropertyName("refund_available")]
    decimal RefundAvailable,
        [property: JsonPropertyName("estimated_network_fees")]
    decimal EstimatedNetworkFees,
        [property: JsonPropertyName("claimer_ip_address")]
    string? ClaimerIpAddress,
        [property: JsonPropertyName("requested_native_coins_date")]
    DateTimeOffset? RequestedNativeCoinsDate,
        [property: JsonPropertyName("native_coins_received_date")]
    DateTimeOffset? NativeCoinsReceivedDate);

    public record PayoutDto(
        [property: JsonPropertyName("id")] Guid Id,
        [property: JsonPropertyName("merchant_payout_wallet_id")]
    Guid? MerchantPayoutWalletId,
        [property: JsonPropertyName("merchant_payout_wallet_currency_id")]
    int MerchantPayoutWalletCurrencyId,
        [property: JsonPropertyName("service_fee_payout_wallet_id")]
    Guid? ServiceFeePayoutWalletId,
        [property: JsonPropertyName("spend_request_id")]
    Guid? SpendRequestId,
        [property: JsonPropertyName("merchant_payout_address")]
    string? MerchantPayoutAddress,
        [property: JsonPropertyName("payout_amount_to_merchant")]
    decimal PayoutAmountToMerchant,
        [property: JsonPropertyName("network_fees_available_for_payout")]
    decimal NetworkFeesAvailableForPayout,
        [property: JsonPropertyName("refund_amount_to_keep_in_wallet")]
    decimal RefundAmountToKeepInWallet,
        [property: JsonPropertyName("blockchain_transaction_id")]
    string? BlockchainTransactionId,
        [property: JsonPropertyName("destination_amount")]
    decimal? DestinationAmount,
        [property: JsonPropertyName("payout_amount_to_coin_payments_for_merchant_service_fees")]
    decimal PayoutAmountToCoinPaymentsForMerchantServiceFees);


    public record InvoiceWebhook(InvoiceWebhookType type, InvoiceDto invoice);
    public record InvoiceDto(Guid id, [property: JsonPropertyName("merchant_client_id")] Guid merchantClientId);

    public enum InvoiceWebhookType
    {
        InvoiceCreated = 1,
        InvoicePending = 2,
        InvoicePaid = 3,
        InvoiceCompleted = 4,
        InvoiceCancelled = 5,
        InvoiceTimedOut = 6,
        CallbackDepositDetected = 10,
        CallbackDepositConfirmed = 11,
    }
}
