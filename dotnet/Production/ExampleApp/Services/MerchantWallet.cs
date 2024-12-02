using Shared;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExampleApp.Clients
{
    public  class MerchantWalletService(CoinPaymentsApiClient _client)
    {

        public Task<MerchantWallet[]> GetMerchantWallets()
        {
            return _client.AuthExecuteAsync<MerchantWallet[]>($"merchant/wallets", HttpMethod.Get, _client.CurrentClient.Id, _client.CurrentClient.Secret);
        }

        public Task<MerchantWallet> GetMerchantWalletById(Guid id)
        {
            return _client.AuthExecuteAsync<MerchantWallet>($"merchant/wallets/{id}", HttpMethod.Get, _client.CurrentClient.Id, _client.CurrentClient.Secret);
        }
        public async Task<NewWalletResponse> CreateMerchantWallet(string currencyId, string label, string? url, string? contractAddress, bool hasPermanentAddress, CancellationToken ct = default)
        {
            var request = new NewWalletRequest
            {
                CurrencyId = currencyId,
                Label = label,
                WebhookUrl = url,
                ContractAddress = contractAddress,
                UsePermanentAddresses = hasPermanentAddress,
            };
            var response = await _client.AuthExecuteAsync<NewWalletResponse>(
                $"merchant/wallets", HttpMethod.Post, _client.CurrentClient.Id, _client.CurrentClient.Secret, request);
            return response!;
        }

        public async Task<CreateWalletAddressResponseDto> CreateMerchantWalletAddress(Guid walletId, string label, AddressType type = AddressType.Personal, string? notificationUrl = null, CancellationToken ct = default)
        {
            var request = new CreateWalletAddressRequestDto
            { Label = label, NotificationUrl = notificationUrl, Type = type };
            var response = await _client.AuthExecuteAsync<CreateWalletAddressResponseDto>(
                $"merchant/wallets/{walletId}/addresses", HttpMethod.Post, _client.CurrentClient.Id, _client.CurrentClient.Secret,
                request);
            return response!;
        }
        public async Task<WalletAddressDto[]> GetMerchantWalletAddress(Guid walletId, CancellationToken ct = default)
        {
            var response = await _client.AuthExecuteAsync<WalletAddressDto[]>(
                $"merchant/wallets/{walletId}/addresses", HttpMethod.Get, _client.CurrentClient.Id, _client.CurrentClient.Secret);
            return response;
        }

        public async Task<WalletSpendResponseDto> RequestSpendFromMerchantWallet(int toCurrencyId, Guid fromWalletId, string toAddress, decimal amount, string? memo = null, CancellationToken ct = default, string? fromContractAddress = null, string? toContractAddress = null, bool receiverPaysFee = false)
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
            var response = await _client.AuthExecuteAsync<WalletSpendResponseDto>(
                $"merchant/wallets/{fromWalletId}/spend/request", HttpMethod.Post, _client.CurrentClient.Id, _client.CurrentClient.Secret, request);
            return response;
        }

        public async Task ConfirmSpendFromMerchantWallet(Guid fromWalletId, Guid spendRequestId, CancellationToken ct = default)
        {
            var request = new WalletSpendConfirmationRequestDto()
            {
                SpendRequestId = spendRequestId
            };
            var response = await _client.AuthExecuteAsync(
                $"merchant/wallets/{fromWalletId}/spend/confirmation", HttpMethod.Post, _client.CurrentClient.Id,
                _client.CurrentClient.Secret, request);
        }
    }
}
