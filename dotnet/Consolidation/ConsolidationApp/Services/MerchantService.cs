using ConsolidationApp.Models.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using ConsolidationApp.Helpers;
using ConsolidationApp.Models.Currencies;
using ConsolidationApp.Models.Invoice;
using ConsolidationApp.Models;
using ConsolidationApp.Models.Merchant;

namespace ConsolidationApp.Services
{
    public class MerchantService 
    {
        const string CoinPaymentsApiClientHeaderName = "X-CoinPayments-Client";
        const string CoinPaymentsApiSignatureHeaderName = "X-CoinPayments-Signature";
        const string CoinPaymentsApiTimestampHeaderName = "X-CoinPayments-Timestamp";
        protected const int TimeoutMilliseconds = 60_000;

        protected static readonly JsonSerializerOptions Options = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new JsonStringEnumConverter() }
        };
        public async Task<NewWalletResponse> CreateMerchantWallet(
            string clientId,
            string clientSecret,
            int currencyId,
            string label,
            string? url,
            string? contractAddress,
            string environmentRootUrl,
            bool hasPermanentAddress,
            CancellationToken ct = default)
        {
            var request = new NewWalletRequest
            {
                CurrencyId = currencyId,
                Label = label,
                WebhookUrl = url,
                ContractAddress = contractAddress,
                UsePermanentAddresses = hasPermanentAddress,
            };
            var response = await RequestWithAuth<NewWalletResponse>(
                $"{environmentRootUrl}/api/v1/merchant/wallets", clientId, clientSecret, ct, HttpMethod.Post, request);
            return response!;
        }

        public async Task<CreateWalletAddressResponseDto> CreateMerchantWalletAddress(
            string clientId,
            string clientSecret,
            Guid walletId,
            string label,
            string environmentRootUrl,
            AddressType type = AddressType.Personal,
            string? notificationUrl = null,
            CancellationToken ct = default)
        {
            var request = new CreateWalletAddressRequestDto
            { Label = label, NotificationUrl = notificationUrl, Type = type };
            var response = await RequestWithAuth<CreateWalletAddressResponseDto>(
                $"{environmentRootUrl}/api/v1/merchant/wallets/{walletId}/addresses", clientId, clientSecret, ct,
                HttpMethod.Post, request);
            return response!;
        }

        public async Task<WalletAddressDto> GetMerchantWalletAddress(string clientId, string clientSecret,
            Guid walletId, Guid addressId, string environmentRootUrl,
            CancellationToken ct = default)
        {
            var response = await RequestWithAuth<WalletAddressDto>(
                $"{environmentRootUrl}/api/v1/merchant/wallets/{walletId}/addresses/{addressId}", clientId,
                clientSecret, ct, HttpMethod.Get);
            return response;
        }

        public async Task<WalletAddressDto[]> GetMerchantWalletAddresses(string clientId, string clientSecret,
            Guid walletId, string environmentRootUrl, CancellationToken ct = default)
        {
            var response = await RequestWithAuth<WalletAddressDto[]>(
                $"{environmentRootUrl}/api/v1/merchant/wallets/{walletId}/addresses", clientId, clientSecret, ct,
                HttpMethod.Get);
            return response;
        }

        public async Task<WalletConsolidationDto> GetWalletConsolidation(string clientId, string clientSecret, Guid walletId, string environmentRootUrl, Guid[]? addressIds = null, CancellationToken ct = default)
        {
            var url = $"{environmentRootUrl}/api/v1/merchant/wallets/{walletId}/consolidation";
            if (addressIds is not null)
                url += "?addressIds=" + string.Join(',', addressIds);
            var response = await RequestWithAuth<WalletConsolidationDto>(url, clientId, clientSecret, ct, HttpMethod.Get);
            return response;
        }

        public async Task<WalletConsolidationDto> GetMultiWalletConsolidation(string clientId, string clientSecret, string environmentRootUrl, WalletsConsolidationRequestDto wallets, CancellationToken ct = default)
        {
            var url = $"{environmentRootUrl}/api/v1/merchant/wallets/consolidation-preview";
            var response = await RequestWithAuth<WalletConsolidationDto>(url, clientId, clientSecret, ct, HttpMethod.Post, wallets);
            return response;
        }

        public async Task<WalletConsolidationDto> ExecuteMultiWalletConsolidation(string clientId, string clientSecret, string environmentRootUrl, WalletsConsolidationRequestDto wallets, Guid? toWalletId = null, CancellationToken ct = default)
        {
            var url = $"{environmentRootUrl}/api/v1/merchant/wallets/consolidation";
            if (toWalletId.HasValue)
                url += $"/{toWalletId}";
            var response = await RequestWithAuth<WalletConsolidationDto>(url, clientId, clientSecret, ct, HttpMethod.Post, wallets);
            return response;
        }

        public async Task<WalletConsolidationDto> ExecuteWalletConsolidation(string clientId, string clientSecret, Guid walletId, string environmentRootUrl, Guid[]? addressIds = null, Guid? toWalletId = null, CancellationToken ct = default)
        {
            var url = $"{environmentRootUrl}/api/v1/merchant/wallets/{walletId}/consolidation";
            if (toWalletId.HasValue)
                url += $"/{toWalletId}";
            if (addressIds is not null)
                url += "?addressIds=" + string.Join(',', addressIds);
            var response = await RequestWithAuth<WalletConsolidationDto>(url, clientId, clientSecret, ct, HttpMethod.Post);
            return response;
        }


        public async Task<WalletSpendResponseDto> RequestSpendFromMerchantWallet(
            string clientId,
            string clientSecret,
            int toCurrencyId,
            Guid fromWalletId,
            string toAddress,
            decimal amount,
            string environmentRootUrl,
            string? memo = null,
            CancellationToken ct = default,
            string? fromContractAddress = null,
            string? toContractAddress = null,
            bool receiverPaysFee = false)
        {
            var request = new WalletSpendRequestDto
            {
                ToAddress = toAddress,
                ToCurrencyId = toCurrencyId,
                AmountInSmallestUnits = amount.AsSmallestUnitsString(),
                Memo = memo,
                FromContractAddress = fromContractAddress,
                ToContractAddress = toContractAddress,
                ReceiverPaysFee = receiverPaysFee
            };
            var response = await RequestWithAuth<WalletSpendResponseDto>(
                $"{environmentRootUrl}/api/v1/merchant/wallets/{fromWalletId}/spend/request", clientId, clientSecret,
                ct, HttpMethod.Post, request);
            return response;
        }

        public async Task ConfirmSpendFromMerchantWallet(string clientId, string clientSecret, Guid fromWalletId,
            Guid spendRequestId, string environmentRootUrl, CancellationToken ct = default)
        {
            var request = new WalletSpendConfirmationRequestDto()
            {
                SpendRequestId = spendRequestId
            };
            var response = await RequestWithAuth<string>(
                $"{environmentRootUrl}/api/v1/merchant/wallets/{fromWalletId}/spend/confirmation", clientId,
                clientSecret, ct, HttpMethod.Post, request);
        }

        public async Task<MerchantWallet[]> GetAllMerchantWallets(string clientId, string clientSecret,
            string environmentRootUrl,
            CancellationToken ct = default)
        {
            var wallets = await RequestWithAuth<MerchantWallet[]>($"{environmentRootUrl}/api/v1/merchant/wallets",
                clientId, clientSecret, ct, HttpMethod.Get);
            return wallets;
        }

        public async Task<RequiredConfirmationDto[]> GetCurrenciesConfirmations(string environmentRootUrl, CancellationToken ct = default)
        {
            return await GetFromJsonAsync<RequiredConfirmationDto[]>($"{environmentRootUrl}/api/v1/currencies/required-confirmations", ct);
        }
        public async Task<MerchantInvoiceSummariesDto> ListInvoices(string environmentRootUrl,
            string clientId,
            string clientSecret,
            string? q = null,
            string? integration = null,
            string? payoutWalletId = null,
            ForwardPagingInputDto? paging = null,
            CancellationToken ct = default
        )
        {
            return await RequestWithAuth<MerchantInvoiceSummariesDto>($"{environmentRootUrl}/api/v1/merchant/invoices",
                clientId, clientSecret, ct, HttpMethod.Get);
        }
        public async Task<InvoiceResponseDto> GetInvoice(Guid invoiceId, string clientId, string clientSecret, string environmentRootUrl, CancellationToken ct = default)
        {
            var response = await RequestWithAuth<InvoiceResponseDto>($"{environmentRootUrl}/api/v1/merchant/invoices/{invoiceId}?include_full_details=true", clientId, clientSecret, ct, HttpMethod.Get);
            return response!;
        }
        public async Task<InvoicePayoutsDetailsDto> GetPayoutDetails(Guid invoiceId, string clientId, string clientSecret, string environmentRootUrl, CancellationToken ct = default)
        {
            var response = await RequestWithAuth<InvoicePayoutsDetailsDto>($"{environmentRootUrl}/api/v1/merchant/invoices/{invoiceId}/payouts", clientId, clientSecret, ct, HttpMethod.Get);
            return response!;
        }
        protected async Task<T> GetFromJsonAsync<T>(string url, CancellationToken? ct = null)
        {
            if (ct == default || ct == CancellationToken.None)
                ct = new CancellationTokenSource(TimeoutMilliseconds).Token;
            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(url, ct!.Value);
            if (!response.IsSuccessStatusCode)
            {
                var strContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"GET {url}. Returned {response.StatusCode}, with content: '{strContent}'");
            }

            return await response.Content.ReadFromJsonAsync<T>(Options, ct!.Value);
        }
        public async Task<CreateMerchantInvoiceResponse?> CreateInvoice(string clientId, string clientSecret,
            string currentEnvironmentUrl, uint? migratedUserId = null, InvoicePayoutConfigDto? payoutConfig = null,
            int amount = 100_00,
            CancellationToken ct = default)
        {
            var request = new CreateMerchantInvoiceRequest(
                IsEmailDelivery: false,
                EmailDelivery: null,
                DueDate: null,
                InvoiceDate: null,
                Draft: false,
                ClientId: clientId,
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

            var url = migratedUserId.HasValue
                ? $"{currentEnvironmentUrl}/api/v1/migrations/{migratedUserId.Value}/invoices"
                : $"{currentEnvironmentUrl}/api/v1/merchant/invoices";

            var response =
                await RequestWithAuth<CreateMerchantInvoiceResponse>(url, clientId, clientSecret, ct, HttpMethod.Post,
                    request);
            return response;
        }

        private async Task<T> RequestWithAuth<T>(string url, string clientId, string clientSecret, CancellationToken ct,
            HttpMethod method, object? body = null)
        {
            using HttpClient HttpClient = new HttpClient();
            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            options.Converters.Add(new JsonStringEnumConverter());
            var bodyString = body is null ? null : JsonSerializer.Serialize(body, Options);
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(clientSecret)))
            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream, Encoding.UTF8))
            {
                var timestampHeader = DateTime.UtcNow.ToString("s");
                writer.Write(method.Method);
                writer.Write(url);
                writer.Write(clientId);
                writer.Write(timestampHeader);
                if (!string.IsNullOrEmpty(bodyString))
                    writer.Write(bodyString);
                writer.Flush();

                stream.Position = 0;

                var hashBytes = hmac.ComputeHash(stream);
                var hash = Convert.ToBase64String(hashBytes);

                HttpClient.DefaultRequestHeaders.Clear();
                HttpClient.DefaultRequestHeaders.Add(CoinPaymentsApiClientHeaderName, clientId);
                HttpClient.DefaultRequestHeaders.Add(CoinPaymentsApiSignatureHeaderName, hash);
                HttpClient.DefaultRequestHeaders.Add(CoinPaymentsApiTimestampHeaderName, timestampHeader);
            }

            var response = method.Method switch
            {
                "GET" => await HttpClient.GetAsync(url, ct),
                "POST" => await HttpClient.PostAsync(url, bodyString != null ?
                    new StringContent(bodyString!, Encoding.UTF8, "application/json") : new StringContent(String.Empty), ct),
                "DELETE" => await HttpClient.DeleteAsync(url, ct),
                _ => throw new NotImplementedException(),
            };
            if (!response.IsSuccessStatusCode)
            {
                var strContent = await response.Content.ReadAsStringAsync();
                throw new Exception(
                    $"{method.Method} {url}. Returned {response.StatusCode}, with content: '{strContent}'");
            }

            var result = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrEmpty(result))
                return default;
            return JsonSerializer.Deserialize<T>(result, options)!;
        }
    }

}
