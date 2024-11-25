using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Models;
namespace ExampleApp.Clients
{
    public partial class ProdApiClient
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
                ClientId: _currentClient.Id,
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
                await AuthExecuteAsync<CreateMerchantInvoiceResponse>($"{API_URL}/merchant/invoices",
                    HttpMethod.Post, _currentClient.Id, _currentClient.Secret, request, ct);
            return response;
        }

        public async Task<InvoicePaymentDto> CreateInvoicePayments(string invoiceId, CancellationToken ct = default)
        {
            var request = new CreateInvoicePaymentRequest($"{Random.Shared.Next()}@in.crypto");
            var response = await AuthExecuteAsync<InvoicePaymentDto>(
                $"{API_URL}/invoices/{invoiceId}/payments", HttpMethod.Post, _currentClient.Id,
                _currentClient.Secret, request, ct);
            return response;
        }

        public async Task<InvoicePaymentCurrencyPaymentDetailsDto> GetInvoicePaymentCurrencyDetails(string invoiceId,
            int currencyId, string? contractAddress = null, CancellationToken ct = default)
        {
            var currency = string.IsNullOrEmpty(contractAddress)
                ? currencyId.ToString()
                : $"{currencyId}:{contractAddress}";

            var response = await AuthExecuteAsync<InvoicePaymentCurrencyPaymentDetailsDto>(
                $"{API_URL}/invoices/{invoiceId}/payment-currencies/{currency}", HttpMethod.Get,
                _currentClient.Id, _currentClient.Secret, null, ct);
            return response;
        }


        public async Task<InvoiceResponseDto> GetInvoice(Guid invoiceId, CancellationToken ct = default)
        {
            var response = await AuthExecuteAsync<InvoiceResponseDto>(
                $"{API_URL}/merchant/invoices/{invoiceId}?include_full_details=true", HttpMethod.Get,
                _currentClient.Id, _currentClient.Secret, null, ct);
            return response!;
        }

        public async Task<InvoiceStatusDtoResponseDto> GetInvoicePaymentStatus(string invoiceId, int currencyId,
            string? smartContract = null, CancellationToken ct = default)
        {
            var currency = string.IsNullOrEmpty(smartContract)
                ? currencyId.ToString()
                : $"{currencyId}:{smartContract}";

            var url = $"{API_URL}/invoices/{invoiceId}/payment-currencies/{currency}/status";

            return await AuthExecuteAsync<InvoiceStatusDtoResponseDto>(url, HttpMethod.Get, _currentClient.Id,
                _currentClient.Secret, null, ct);
        }

        public async Task<InvoicePayoutsDetailsDto> GetPayoutDetails(Guid invoiceId, CancellationToken ct = default)
        {
            var response = await AuthExecuteAsync<InvoicePayoutsDetailsDto>(
                $"{API_URL}/merchant/invoices/{invoiceId}/payouts", HttpMethod.Get, _currentClient.Id,
                _currentClient.Secret, null, ct);
            return response!;
        }

        public async Task<MerchantInvoiceSummariesDto> GetInvoices(string? clientId = null,
            CancellationToken ct = default)
        {
            clientId ??= _currentClient.Id;
            var response = await AuthExecuteAsync<MerchantInvoiceSummariesDto>(
                $"{API_URL}/merchant/invoices?clientId={clientId}", HttpMethod.Get, _currentClient.Id,
                _currentClient.Secret, null, ct);
            return response!;
        }
        public async Task<string> CreatePaymentButtonHtmlUsingMerchantClient(int amount,
            CancellationToken ct = default)
        {
            var body = CreateBuyNowButtonDto.CreateDefault(amount);
            var (_, response) = await AuthExecuteAsync(
                $"{API_URL}/merchant/invoices/buy-now-button", HttpMethod.Post, _currentClient.Id,
                _currentClient.Secret,
                body, ct);

            return response;
        }
    }
}
