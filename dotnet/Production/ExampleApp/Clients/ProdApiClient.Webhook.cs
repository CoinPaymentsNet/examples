using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExampleApp.Clients
{
    public partial class ProdApiClient
    {
        public async Task<MerchantClientWebhookEndpointsDto> GetMerchantClientWebhooks(CancellationToken ct = default)
        {
            var url = $"{API_URL}/merchant/clients/{_currentClient.Id}/webhooks";

            return await AuthExecuteAsync<MerchantClientWebhookEndpointsDto>(url, HttpMethod.Get, _currentClient.Id, _currentClient.Secret, ct: ct);
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

            var url = $"{API_URL}/merchant/clients/{_currentClient.Id}/webhooks";

            var response = await AuthExecuteAsync<CreateMerchantClientWebhookResponseDto>(url, HttpMethod.Post, _currentClient.Id, _currentClient.Secret, request, ct);
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

            var url = $"{API_URL}/merchant/clients/{_currentClient.Id}/webhooks/{webhookId}";

            await AuthExecuteAsync(url, HttpMethod.Put, _currentClient.Id, _currentClient.Secret, request, ct);
        }
    }
}
