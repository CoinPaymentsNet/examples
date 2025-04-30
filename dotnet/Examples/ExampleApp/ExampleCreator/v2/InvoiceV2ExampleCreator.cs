using ExampleApp.Clients;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExampleApp.ExampleCreator.v2
{
    public class InvoiceV2ExampleCreator(CoinPaymentsApiClient _client)
    {
        /// <summary>
        /// In this version, the invoice amount to be created must be sent in full value. If a fraction in cents is desired, it should be sent as 10.50
        /// - <paramref name="amountAsUsd"/> Valid values must be integers or imperfects separated by `.` => (10 or 10.50)
        /// </summary>
        /// <param name="amountAsUsd">the amount value to be sent as integer or fraction</param>
        /// <returns></returns>
        public async Task CreateInvoiceInUSD(decimal amountAsUsd = 10_00)
        {
            var clientWebhooks = await GetMerchantClientWebhooks();
            var callbackWebhookUrl = "https://webhook.site/2fdf7c45-5c29-4fa1-a096-7a4a4114153a"; //Must change for own webhook address!
            var autoTestWebhook =
                clientWebhooks.Items.FirstOrDefault(x => x.NotificationsUrl == callbackWebhookUrl);
            var allNotificationTypes = Enum.GetValues<MerchantClientWebhookNotification>().ToArray();
            if (autoTestWebhook is null)
            {
                Console.WriteLine($"Creating webhook for test");

                var createClientWebhookResponse =
                    await CreateMerchantClientWebhook(callbackWebhookUrl);
            }
            var amountAsString = amountAsUsd.ToString();
            var simpleRequest = new
            {
                Currency="5057",
                ClientId = _client.CurrentClient.Id,
                Items = new object[]{
                    new {
                        Name="test item",
                        Quantity=new{Value=1,Type=2},
                        Amount=amountAsString,
                        }
                },
                Amount = new
                {
                    Total = amountAsString,
                    Breakdown = new
                    {
                        Subtotal = amountAsString,
                    }
                },
                MerchantOptions = new
                {
                    ShowEmail = true,
                }
            };
            var newInvoice =
                await _client.AuthExecuteAsync<CreateMerchantInvoiceResponseV2Dto>(
                    "https://api.coinpayments.com/api/v2/merchant/invoices",
                    HttpMethod.Post, _client.CurrentClient.Id, _client.CurrentClient.Secret, simpleRequest);
            await Task.Delay(2000);
            var invoice = newInvoice.Invoices.First();
            var invoiceId = Guid.Parse(invoice.Id);

            var request = new { RefundEmail = $"{Random.Shared.Next()}@in.crypto" };
            var payment = await _client.AuthExecuteAsync<InvoicePaymentDto>(
                $"https://api.coinpayments.com/api/v1/invoices/{invoiceId}/payments", HttpMethod.Post, _client.CurrentClient.Id,
                _client.CurrentClient.Secret, request);
            await Task.Delay(2000);

            var targetCurrencyId = Currencies.LTCT.GetId();
            var paymentDetails = await _client.AuthExecuteAsync<InvoicePaymentCurrencyPaymentDetailsDto>(
                $"https://api.coinpayments.com/api/v1/invoices/{invoiceId}/payment-currencies/{targetCurrencyId}", HttpMethod.Get,
                _client.CurrentClient.Id, _client.CurrentClient.Secret, null);
            await Task.Delay(2000);

            Console.WriteLine($"Awaiting {payment.PaymentCurrencies.FirstOrDefault(x => x.Currency.Id == targetCurrencyId).RemainingAmount.DisplayValue} " +
                $"LTCT amount on {paymentDetails?.Addresses.Address}");

            var payoutSetting = await _client.AuthExecuteAsync<InvoicePayoutsDetailsDto>(
                $"https://api.coinpayments.com/api/v2/merchant/invoices/{invoiceId}/payouts", HttpMethod.Get, _client.CurrentClient.Id,
                _client.CurrentClient.Secret, null);
            await Task.Delay(2000);

            var invoiceStatusResponse = await _client.AuthExecuteAsync<InvoiceStatusDtoResponseDto>(
                $"https://api.coinpayments.com/api/v1/invoices/{invoiceId}/payment-currencies/{targetCurrencyId}/status", HttpMethod.Get, _client.CurrentClient.Id,
                _client.CurrentClient.Secret, null);

        }

        private async Task<MerchantClientWebhookEndpointsDto> GetMerchantClientWebhooks(CancellationToken ct = default)
        {
            var url = $"https://api.coinpayments.com/api/v1/merchant/clients/{_client.CurrentClient.Id}/webhooks";

            return await _client.AuthExecuteAsync<MerchantClientWebhookEndpointsDto>(url, HttpMethod.Get, _client.CurrentClient.Id, _client.CurrentClient.Secret, ct: ct);
        }
        private async Task<CreateMerchantClientWebhookResponseDto> CreateMerchantClientWebhook(string notificationUrl, CancellationToken ct = default)
        {
            var request = new CreateMerchantClientWebhookRequestDto
            {
                NotificationsUrl = notificationUrl,
                Notifications = new[]
                {
                MerchantClientWebhookNotification.invoiceCreated
            }
            };

            var url = $"https://api.coinpayments.com/api/v1/merchant/clients/{_client.CurrentClient.Id}/webhooks";

            var response = await _client.AuthExecuteAsync<CreateMerchantClientWebhookResponseDto>(url, HttpMethod.Post, _client.CurrentClient.Id, _client.CurrentClient.Secret, request, ct);
            return response;
        }
    }
}
