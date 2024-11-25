using Shared;
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

        public Task<MerchantWallet[]> GetMerchantWallets()
        {
            return AuthExecuteAsync<MerchantWallet[]>($"{API_URL}/merchant/wallets", HttpMethod.Get, _currentClient.Id, _currentClient.Secret);
        }

        public Task<MerchantWallet> GetMerchantWalletById(Guid id)
        {
            return AuthExecuteAsync<MerchantWallet>($"{API_URL}/merchant/wallets/{id}", HttpMethod.Get, _currentClient.Id, _currentClient.Secret);
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
            var response = await AuthExecuteAsync<NewWalletResponse>(
                $"{API_URL}/merchant/wallets", HttpMethod.Post, _currentClient.Id, _currentClient.Secret, request);
            return response!;
        }

        public async Task<CreateWalletAddressResponseDto> CreateMerchantWalletAddress(Guid walletId, string label, AddressType type = AddressType.Personal, string? notificationUrl = null, CancellationToken ct = default)
        {
            var request = new CreateWalletAddressRequestDto
            { Label = label, NotificationUrl = notificationUrl, Type = type };
            var response = await AuthExecuteAsync<CreateWalletAddressResponseDto>(
                $"{API_URL}/merchant/wallets/{walletId}/addresses", HttpMethod.Post, _currentClient.Id, _currentClient.Secret,
                request);
            return response!;
        }
        public async Task<WalletAddressDto[]> GetMerchantWalletAddress(Guid walletId, CancellationToken ct = default)
        {
            var response = await AuthExecuteAsync<WalletAddressDto[]>(
                $"{API_URL}/merchant/wallets/{walletId}/addresses", HttpMethod.Get, _currentClient.Id, _currentClient.Secret);
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
            var response = await AuthExecuteAsync<WalletSpendResponseDto>(
                $"{API_URL}/merchant/wallets/{fromWalletId}/spend/request", HttpMethod.Post, _currentClient.Id, _currentClient.Secret, request);
            return response;
        }

        public async Task ConfirmSpendFromMerchantWallet(Guid fromWalletId, Guid spendRequestId, CancellationToken ct = default)
        {
            var request = new WalletSpendConfirmationRequestDto()
            {
                SpendRequestId = spendRequestId
            };
            var response = await AuthExecuteAsync(
                $"{API_URL}/merchant/wallets/{fromWalletId}/spend/confirmation", HttpMethod.Post, _currentClient.Id,
                _currentClient.Secret, request);
        }
    }
}
