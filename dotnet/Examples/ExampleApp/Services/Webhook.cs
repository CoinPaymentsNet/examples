using ExampleApp.Clients;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExampleApp.Services
{
    public class Webhook(CoinPaymentsApiClient _client)
    {
        public async Task<MerchantClientWebhookEndpointsDto> GetMerchantClientWebhooks(CancellationToken ct = default)
        {
            var url = $"merchant/clients/{_client.CurrentClient.Id}/webhooks";

            return await _client.AuthExecuteAsync<MerchantClientWebhookEndpointsDto>(url, HttpMethod.Get, _client.CurrentClient.Id, _client.CurrentClient.Secret, ct: ct);
        }
        public async Task<CreateMerchantClientWebhookResponseDto> CreateMerchantClientWebhook(string notificationUrl, CancellationToken ct = default)
        {
            var request = new CreateMerchantClientWebhookRequestDto
            {
                NotificationsUrl = notificationUrl,
                Notifications = new[]
                {
                MerchantClientWebhookNotification.invoiceCreated
            }
            };

            var url = $"merchant/clients/{_client.CurrentClient.Id}/webhooks";

            var response = await _client.AuthExecuteAsync<CreateMerchantClientWebhookResponseDto>(url, HttpMethod.Post, _client.CurrentClient.Id, _client.CurrentClient.Secret, request, ct);
            return response;
        }


        public async Task UpdateMerchantClientWebhook(string webhookId,
            string notificationUrl,
            IEnumerable<MerchantClientWebhookNotification> notifications, CancellationToken ct = default)
        {
            var request = new UpdateMerchantClientWebhookDto
            {
                NotificationsUrl = notificationUrl,
                Notifications = notifications.ToArray()
            };

            var url = $"merchant/clients/{_client.CurrentClient.Id}/webhooks/{webhookId}";

            await _client.AuthExecuteAsync(url, HttpMethod.Put, _client.CurrentClient.Id, _client.CurrentClient.Secret, request, ct);
        }
    }
}
