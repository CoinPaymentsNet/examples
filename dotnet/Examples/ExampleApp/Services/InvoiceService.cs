using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Linq.Expressions;
namespace ExampleApp.Clients
{
    public class InvoiceService(CoinPaymentsApiClient _client)
    {
        public async Task<CreateMerchantInvoiceResponse?> CreateInvoice(uint? migratedUserId = null,
            InvoicePayoutConfigDto? payoutConfig = null,
            int amount = 100_00,
            CancellationToken ct = default)
        {
            var request = new CreateMerchantInvoiceRequest(
                IsEmailDelivery: false,
                EmailDelivery: null,
                DueDate: null,
                InvoiceDate: null,
                Draft: false,
                ClientId: _client.CurrentClient.Id,
                InvoiceId: null,
                Buyer: null,
                Description: null,
                Items: new MerchantInvoiceLineItem[]
                {
                    new MerchantInvoiceLineItem(
                        CustomId: null,
                        SKU: null,
                        Name: "test item",
                        Description: null,
                        Quantity: new MerchantInvoiceLineItemQuantity(Value: 1, Type: 2),
                        OriginalAmount: new InvoiceMoney("5057", null, null, Value: amount),
                        Amount: new InvoiceMoney("5057", null, null, Value: amount),
                        Tax: null)
                },
                Amount: new InvoiceAmount("5057", null, null, Value: amount, new InvoiceAmountBreakdownDto(
                    new InvoiceMoney("5057", null, null, Value: amount),
                    new InvoiceMoney("5057", null, null, Value: 0),
                    new InvoiceMoney("5057", null, null, Value: 0),
                    new InvoiceMoney("5057", null, null, Value: 0),
                    new InvoiceMoney("5057", null, null, Value: 0))),
                RequireBuyerNameAndEmail: null,
                BuyerDataCollectionMessage: null,
                Notes: null,
                NotesToRecipient: null,
                TermsAndConditions: null,
                MerchantOptions: new InvoiceMerchantOptions(
                    ShowAddress: false,
                    ShowEmail: true,
                    ShowPhone: false,
                    ShowRegistrationNumber: false,
                    AdditionalInfo: null),
                CustomData: null,
                PONumber: null,
                Webhooks: null,
                /*new InvoiceWebhook[]
                {
                    new(
                        "http://localhost:5095/ipn",
                        new[]
                        {
                            MerchantClientWebhookNotification.invoiceCreated,
                            MerchantClientWebhookNotification.invoiceCancelled,
                            MerchantClientWebhookNotification.invoiceCompleted,
                            MerchantClientWebhookNotification.invoicePending,
                            MerchantClientWebhookNotification.invoicePaid,
                        }
                    )
                },*/
                PayoutConfig: payoutConfig
            );

            var response =
                await _client.AuthExecuteAsync<CreateMerchantInvoiceResponse>($"merchant/invoices",
                    HttpMethod.Post, _client.CurrentClient.Id, _client.CurrentClient.Secret, request, ct);
            return response;
        }

        public async Task<InvoicePaymentDto> CreateInvoicePayments(string invoiceId, CancellationToken ct = default)
        {
            var request = new CreateInvoicePaymentRequest($"{Random.Shared.Next()}@in.crypto");
            var response = await _client.AuthExecuteAsync<InvoicePaymentDto>(
                $"invoices/{invoiceId}/payments", HttpMethod.Post, _client.CurrentClient.Id,
                _client.CurrentClient.Secret, request, ct);
            return response;
        }

        public async Task<InvoicePaymentCurrencyPaymentDetailsDto> GetInvoicePaymentCurrencyDetails(string invoiceId,
            int currencyId, string? contractAddress = null, CancellationToken ct = default)
        {
            var currency = string.IsNullOrEmpty(contractAddress)
                ? currencyId.ToString()
                : $"{currencyId}:{contractAddress}";

            var response = await _client.AuthExecuteAsync<InvoicePaymentCurrencyPaymentDetailsDto>(
                $"invoices/{invoiceId}/payment-currencies/{currency}", HttpMethod.Get,
                _client.CurrentClient.Id, _client.CurrentClient.Secret, null, ct);
            return response;
        }


        public async Task<InvoiceResponseDto> GetInvoice(Guid invoiceId, CancellationToken ct = default)
        {
            var response = await _client.AuthExecuteAsync<InvoiceResponseDto>(
                $"merchant/invoices/{invoiceId}?include_full_details=true", HttpMethod.Get,
                _client.CurrentClient.Id, _client.CurrentClient.Secret, null, ct);
            return response!;
        }

        public async Task<InvoiceStatusDtoResponseDto> GetInvoicePaymentStatus(string invoiceId, int currencyId,
            string? smartContract = null, CancellationToken ct = default)
        {
            var currency = string.IsNullOrEmpty(smartContract)
                ? currencyId.ToString()
                : $"{currencyId}:{smartContract}";

            var url = $"invoices/{invoiceId}/payment-currencies/{currency}/status";

            return await _client.AuthExecuteAsync<InvoiceStatusDtoResponseDto>(url, HttpMethod.Get, _client.CurrentClient.Id,
                _client.CurrentClient.Secret, null, ct);
        }

        public async Task<InvoicePayoutsDetailsDto> GetPayoutDetails(Guid invoiceId, CancellationToken ct = default)
        {
            var response = await _client.AuthExecuteAsync<InvoicePayoutsDetailsDto>(
                $"merchant/invoices/{invoiceId}/payouts", HttpMethod.Get, _client.CurrentClient.Id,
                _client.CurrentClient.Secret, null, ct);
            return response!;
        }

        public async Task<MerchantInvoiceSummariesDto> GetInvoices(string? clientId = null,
            CancellationToken ct = default)
        {
            clientId ??= _client.CurrentClient.Id;
            var response = await _client.AuthExecuteAsync<MerchantInvoiceSummariesDto>(
                $"merchant/invoices?clientId={clientId}", HttpMethod.Get, _client.CurrentClient.Id,
                _client.CurrentClient.Secret, null, ct);
            return response!;
        }
        public async Task<string> CreatePaymentButtonHtmlUsingMerchantClient(int amount,
            CancellationToken ct = default)
        {
            var body = CreateBuyNowButtonDto.CreateDefault(amount);
            var (_, response) = await _client.AuthExecuteAsync(
                $"merchant/invoices/buy-now-button", HttpMethod.Post, _client.CurrentClient.Id,
                _client.CurrentClient.Secret,
                body, ct);

            return response;
        }

        private MerchantWallet[] _prodMerchantWallets;

        private async Task GetProdMerchantWallets()
        {
            //_prodMerchantWallets = await _client.GetMerchantWallets();
        }

        public async Task CreateInvoiceInUSD_and_List_LTCT_Payouts(int amountAsUsd = 10_00)
        {

            //var newInvoice = await _client.CreateInvoice(amount: amountAsUsd);
            //var invoice = newInvoice.Invoices.First();
            //var invoiceId = Guid.Parse(invoice.Id);
            //var payment = await _client.CreateInvoicePayments(invoiceId.ToString());
            //var paymentDetails =
            //    await _client.GetInvoicePaymentCurrencyDetails(invoiceId.ToString(), Currencies.LTCT.Id);
            //Console.WriteLine($"Awaiting {payment.PaymentCurrencies.FirstOrDefault(x => x.Currency.Id == Currencies.LTCT.Id.ToString()).RemainingAmount.DisplayValue} " +
            //    $"LTCT amount on {paymentDetails?.Addresses.Address}");
            //var payoutSetting = await _client.GetPayoutDetails(invoiceId);

            //var invoiceStatusResponse = await _client.GetInvoicePaymentStatus(invoice.Id, Currencies.LTCT.Id, null);
            ///The following lines can be used to pay the invoice related to another wallet of the current merchant..
           /*
            //await GetProdMerchantWallets();

            //var highestBalanceWallet = _prodMerchantWallets.Where(x => x.CurrencyId == Currencies.LTCT.Id)
            //    .OrderByDescending(x => Convert.ToInt64(x.ConfirmedBalance?.Value)).FirstOrDefault();

            var spendRequest = await _prodClient.RequestSpendFromMerchantWallet(Currencies.LTCT.Id,
                Guid.Parse(highestBalanceWallet.WalletId), paymentDetails.Addresses.Address,
                Convert.ToDecimal(paymentDetails.Amount.Value));

            var allFees = spendRequest.BlockchainFee + spendRequest.CoinpaymentsFee;
            await _prodClient.ConfirmSpendFromMerchantWallet(Guid.Parse(highestBalanceWallet.WalletId),
                spendRequest.SpendRequestId);
            Stopwatch sw = new Stopwatch();
            sw.Start();
            var isInvoiceStatusPaid = false;
            while (sw.Elapsed <= TimeSpan.FromMinutes(3))
            {
                await Task.Delay(2000);
                invoiceStatusResponse = await _prodClient.GetInvoicePaymentStatus(invoice.Id, Currencies.LTCT.Id, null);
                if (invoiceStatusResponse.Status == InvoiceStatus.Paid)
                {
                    isInvoiceStatusPaid = true;
                    sw.Stop();
                    break;
                }
            }

            if (!isInvoiceStatusPaid)
            {
                throw new Exception("Invoice payment failed cause to invoice not paid in 3 minutes.");
            }


            await Task.Delay(5000);
            payoutSetting = await _prodClient.GetPayoutDetails(invoiceId);
            var allMerchantFees = payoutSetting.Items.Sum(x => x.MerchantFees.TransactionFee.ValueAsDecimal) +
                                  //payoutSetting.Items.Sum(x => x.MerchantFees.ConversionFee.ValueAsDecimal) +
                                  payoutSetting.Items.Sum(x => x.MerchantFees.NetworkFee.ValueAsDecimal);

            var beforeSourceBalance = Convert.ToDecimal(highestBalanceWallet.ConfirmedBalance.Value);
            var actualSourceBalance = Convert.ToDecimal(highestBalanceWallet.ConfirmedBalance.Value) - allFees -
                                      Convert.ToDecimal(paymentDetails.Amount.Value);

            await GetProdMerchantWallets();

            highestBalanceWallet = GetProdMerchantWalletBy(x => x.WalletId == highestBalanceWallet.WalletId);

            // GetProdMerchantWalletBy(x => x.DepositAddress == paymentDetails.Addresses.Address);
            if (Convert.ToDecimal(highestBalanceWallet.ConfirmedBalance.Value) == actualSourceBalance)
            {

            }
            else
            {
                throw new Exception("Invoice payment failed. Source and target wallet's amount are not valid");
            }
           */
        }
        MerchantWallet? GetProdMerchantWalletBy(Expression<Func<MerchantWallet, bool>> exp) =>
                _prodMerchantWallets.FirstOrDefault(exp.Compile());


    }
    public sealed class CreateMerchantInvoiceResponseV2Dto
    {
        /// <summary>
        /// the id of the created invoice
        /// </summary>
        public required InvoicesCreatedV2Dto[] Invoices { get; set; }
    }

    public sealed class InvoicesCreatedV2Dto
    {
        /// <summary>
        /// The id of the created invoice
        /// </summary>
        public required string Id { get; set; }

        /// <summary>
        /// The link to the created invoice
        /// </summary>
        public required string Link { get; set; }

        /// <summary>
        /// The link to the checkout app
        /// </summary>
        public required string CheckoutLink { get; set; }

        /// <summary>
        /// Invoice payment info
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public InvoicePaymentV2Dto? Payment { get; set; }

        /// <summary>
        /// Invoice hot wallet info
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public InvoicePaymentCurrencyPaymentDetailsV2Dto? HotWallet { get; set; }
    }

    public sealed class InvoicePaymentCurrencyPaymentDetailsV2Dto
    {
        /// <summary>
        /// the currency for the payment details, to which the amount and addresses are specified to
        /// </summary>
        public required CurrencyDto Currency { get; set; }

        /// <summary>
        /// the amount due to be paid in this currency
        /// </summary>
        public required string Amount { get; set; }
        public required string RemainingAmount { get; set; }

        /// <summary>
        /// the addresses and URIs for sending payments to the invoice (for crypto currency payments)
        /// </summary>
        public required InvoicePaymentCurrencyPaymentAddressesDto? Addresses { get; set; }

        /// <summary>
        /// the timestamp when the payment expires and new payments will no longer be accepted
        /// </summary>
        public required DateTimeOffset Expires { get; set; }
    }

    public sealed class InvoicePaymentV2Dto
    {
        /// <summary>
        /// the id of the payment
        /// </summary>
        public required string PaymentId { get; set; }

        /// <summary>
        /// the timestamp when the payment expires and new payments will no longer be accepted
        /// </summary>
        public required DateTimeOffset Expires { get; set; }

        public required InvoicePaymentCurrencyV2Dto[] PaymentCurrencies { get; set; }

        public required string RefundEmail { get; set; } = null!;
    }

    public sealed class InvoicePaymentCurrencyV2Dto
    {
        /// <summary>
        /// the currency information in which an invoice can be paid in
        /// </summary>
        public required CurrencyDto Currency { get; set; }

        /// <summary>
        /// flag indicating whether this currency is currently unavailable (e.g. node or services down)
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public bool IsDisabled { get; set; }

        /// <summary>
        /// the total amount due to pay the remainder of the invoices balance in this currency
        /// </summary>
        public required string Amount { get; set; }

        public required string ApproximateNetworkAmount { get; set; }

        public required string RemainingAmount { get; set; }
    }
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
        invoiceCreated = 0,
        invoicePending = 1,
        invoicePaid = 2,
        invoiceCompleted = 3,
        invoiceCancelled = 4,
        invoiceTimedOut = 5
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
