using System.Text.Json.Serialization;

namespace ConsolidationApp.Models;

public class Paged<T>
{
    public T[] Items { get; set; }
}

public class PagedItemsDto<T>
{
    /// <summary>
    /// current page of items
    /// </summary>
    public required T[] Items { get; set; }

    /// <summary>
    /// paging information
    /// </summary>
    public PagingDto? Paging { get; set; }
}

public sealed class PagingDto
{
    /// <summary>
    /// the cursors for the current page
    /// </summary>
    public PagingCursorsDto? Cursors { get; set; }

    /// <summary>
    /// the limit that was specified, the query may have resulted in fewer items </summary>

    public int Limit { get; set; }

    /// <summary>
    /// the URI to the first page of results, if available
    /// </summary>
    public string? First { get; set; }

    /// <summary>
    /// the URI to the next page of results, if the next link doesn't exist then the current page is the last page
    /// of results
    /// </summary>
    /// <example>https://api.coinpayments.net/v1/...</example>
    public string? Next { get; set; }

    /// <summary>
    /// the URI to the previous page of results, if the previous link doesn't exist then the current page is the
    /// first page of results
    /// </summary>
    /// <example>https://api.coinpayments.net/v1/...</example>
    public string? Previous { get; set; }

    /// <summary>
    /// the URI to the last page of results, if available
    /// </summary>
    public string? Last { get; set; }
}

public sealed class PagingCursorsDto
{
    /// <summary>
    /// cursor that points to the start of the page of data that has been returned
    /// </summary>
    public string? Before { get; set; }

    /// <summary>
    /// cursor that points to the end of the page of data that has been returned
    /// </summary>
    public string? After { get; set; }
}


public class ListDtos<T>
{
    [JsonPropertyName("items")]
    public T[] Items { get; set; } = Array.Empty<T>();
}

public record PaymentCurrencyDto(
    [property: JsonPropertyName("id")] Guid Id,
    [property: JsonPropertyName("merchant_id")] Guid MerchantId,
    [property: JsonPropertyName("currency_id")] int CurrencyId,
    [property: JsonPropertyName("smart_contract")] string? SmartContract,
    [property: JsonPropertyName("payout_frequency")] PayoutCurrencyFrequencyDto PayoutFrequency,
    [property: JsonPropertyName("order")] int Order,
    [property: JsonPropertyName("discount_percent")] float DiscountPercent,
    [property: JsonPropertyName("markup_percent")] float MarkupPercent,
    [property: JsonPropertyName("payout_settings")] PayoutCurrencyDto PayoutSettings,
    [property: JsonPropertyName("is_enabled")] bool IsEnabled);

public enum PayoutCurrencyFrequencyDto
{
    Normal = 1,
    AsSoonAsPossible = 2,
    Hourly = 3,
    Nightly = 4,
    Weekly = 5
}

public record PayoutCurrencyDto(
    [property: JsonPropertyName("currency_id")] int CurrencyId,
    [property: JsonPropertyName("smart_contract")] string? SmartContract,
    [property: JsonPropertyName("to_address")] string? ToAddress,
    [property: JsonPropertyName("to_wallet_id")] Guid? ToWalletId,
    [property: JsonPropertyName("payout_split")] int PayoutSplit);



public record PaymentHotWalletDto([property: JsonPropertyName("error")] SettlementErrorDto? Error);

public record SettlementErrorDto(
    [property: JsonPropertyName("modes")] InvoicePaymentSettlementModeErrorDto[] Modes,
    [property: JsonPropertyName("message")] string? Message);

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

public record RefundDto(
    [property: JsonPropertyName("id")] Guid Id,
    [property: JsonPropertyName("invoice_payment_id")] Guid InvoicePaymentId,
    [property: JsonPropertyName("created")] DateTimeOffset Created,
    [property: JsonPropertyName("expires")] DateTimeOffset Expires,
    [property: JsonPropertyName("last_email_notification_sent")] DateTimeOffset? LastEmailNotificationSent,
    [property: JsonPropertyName("next_email_notification")] DateTimeOffset NextEmailNotification,
    [property: JsonPropertyName("claimed")] DateTimeOffset? Claimed,
    [property: JsonPropertyName("payout_address")] string? PayoutAddress,
    [property: JsonPropertyName("payout_amount")] decimal PayoutAmount,
    [property: JsonPropertyName("payout_network_fees")] decimal PayoutNetworkFees,
    [property: JsonPropertyName("refund_email")] string? RefundEmail,
    [property: JsonPropertyName("refund_emails_sent")] int RefundEmailsSent,
    [property: JsonPropertyName("refund_available")] decimal RefundAvailable,
    [property: JsonPropertyName("estimated_network_fees")] decimal EstimatedNetworkFees,
    [property: JsonPropertyName("claimer_ip_address")] string? ClaimerIpAddress,
    [property: JsonPropertyName("requested_native_coins_date")] DateTimeOffset? RequestedNativeCoinsDate,
    [property: JsonPropertyName("native_coins_received_date")] DateTimeOffset? NativeCoinsReceivedDate);

public record PayoutDto(
    [property: JsonPropertyName("id")] Guid Id,
    [property: JsonPropertyName("merchant_payout_wallet_id")] Guid? MerchantPayoutWalletId,
    [property: JsonPropertyName("merchant_payout_wallet_currency_id")] int MerchantPayoutWalletCurrencyId,
    [property: JsonPropertyName("service_fee_payout_wallet_id")] Guid? ServiceFeePayoutWalletId,
    [property: JsonPropertyName("spend_request_id")] Guid? SpendRequestId,
    [property: JsonPropertyName("merchant_payout_address")] string? MerchantPayoutAddress,
    [property: JsonPropertyName("payout_amount_to_merchant")] decimal PayoutAmountToMerchant,
    [property: JsonPropertyName("network_fees_available_for_payout")] decimal NetworkFeesAvailableForPayout,
    [property: JsonPropertyName("refund_amount_to_keep_in_wallet")] decimal RefundAmountToKeepInWallet,
    [property: JsonPropertyName("blockchain_transaction_id")] string? BlockchainTransactionId,
    [property: JsonPropertyName("destination_amount")] decimal? DestinationAmount,
    [property: JsonPropertyName("payout_amount_to_coin_payments_for_merchant_service_fees")] decimal PayoutAmountToCoinPaymentsForMerchantServiceFees);
public class ForwardPagingInputDto
{
    private const string AfterQueryName = "after";
    private const string LimitQueryName = "limit";

    /// <summary>
    /// cursor that points to the end of the page of data that has been returned
    /// </summary>
    public string After { get; set; }

    /// <summary>
    /// the maximum number of objects that may be returned, the query may return fewer results than the requested maximum.  If `after` is specified then `limit` specifies the number of items to return starting from `after`.  If not specified then `limit` specifies the number of items to return from the beginning.
    /// </summary>
    public int? Limit { get; set; }

    public ForwardPagingQuery ToForwardPagingQueryOrDefault()
    {
        if (!string.IsNullOrEmpty(After) || Limit != null)
        {
            return new ForwardPagingQuery { After = After, First = Limit };
        }

        return null;
    }
}
public class ForwardPagingQuery
{
    /// <summary>
    /// cursor that points to the end of the page of data that has been returned (e.g. id of the last item)
    /// </summary>
    public string? After { get; set; }

    /// <summary>
    /// specifies the number of items to return starting from <see cref="After"/>, or the first entry if
    /// <see cref="After"/> is not specified.
    /// </summary>
    public int? First { get; set; }
}