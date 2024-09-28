using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using ApiClientModels = ConsolidationApp.Models.Client;
using ConsolidationApp.Models.Wallet;
using ConsolidationApp.Models.Token;
using ConsolidationApp.Models.Client;

namespace ConsolidationApp.Services
{
    public class UserService 
    {
        public async Task<PagedWalletsDto?> GetWallets(string email, string password, string currentEnvironmentRootUrl, int? currencyId = null)
        {
            var accessTokenResponse = await GetAccessToken(email, password, currentEnvironmentRootUrl);

            var url = $"{currentEnvironmentRootUrl}/api/v1/wallets";

            if (currencyId != null)
                url += $"?currencyId={currencyId}";

            var response = await RequestWithAuth<PagedWalletsDto>(url, accessTokenResponse!.AccessToken, HttpMethod.Get);
            return response;
        }


        public async Task<ApiClientModels.SpendRequestValidateResponseDto> ValidateSpendFromWalletAsync(
            string email,
            string password,
            string walletId,
            string address,
            string amount,
            string? blockchainFee,
            string? contractAddress,
            string? toContractAddress,
            int? toCurrencyId,
            string? memo,
            string currentEnvironmentRootUrl,
            bool feeSubtractedFromAmount = true)
        {
            var token = await GetAccessToken(email, password, currentEnvironmentRootUrl);
            var url = $"{currentEnvironmentRootUrl}/api/v1/wallets/{walletId}/spend/validate";

            var request = new ApiClientModels.WalletSpendRequestDto
            {
                Recipients = new ApiClientModels.WalletSpendRequestRecipientDto[]
                {
                    new ApiClientModels.WalletSpendRequestRecipientDto
                    {
                        Address = address,
                        Amount = amount,
                        BlockChainFee = blockchainFee
                    }
                },
                ToCurrency = toCurrencyId,
                ContractAddress = contractAddress,
                ToContractAddress = toContractAddress,
                Memo = memo,
                FeeSubtractedFromAmount = feeSubtractedFromAmount
            };

            var response = await RequestWithAuth<ApiClientModels.SpendRequestValidateResponseDto>(url, token!.AccessToken, HttpMethod.Post, request);
            return response;
        }


        public async Task<PagedWalletAddressesDto?> GetAddresses(Guid walletId, string email, string password, string currentEnvironmentUrl, CancellationToken ct = default)
        {
            var token = await GetAccessToken(email, password, currentEnvironmentUrl);

            var result = await RequestWithAuth<PagedWalletAddressesDto?>($"{currentEnvironmentUrl}/api/v1/wallets/{walletId}/addresses", token!.AccessToken, HttpMethod.Get);
            return result;
        }
        public async Task<AccessTokenResponseMessageDto?> GetAccessToken(string email, string password, string environmentRootUrl)
        {
            var url = $"{environmentRootUrl}/api/v1/user/accesstoken";
            var request = new
            {
                Email = email,
                Password = password,
            };

            using var client = new HttpClient();
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json) { Headers = { ContentType = new MediaTypeHeaderValue("application/json") } };
            using var response = await client.PostAsync(url, content);
            if (!response.IsSuccessStatusCode)
            {
                var strContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"POST {url}. Returned {response.StatusCode}, with content: '{strContent}'");
            }

            var accessTokenResponseMessage = await response.Content.ReadFromJsonAsync<AccessTokenResponseMessageDto>();

            return accessTokenResponseMessage;
        }
        public async Task<WalletSpendRequestResponseDto> RequestSpendFromWalletAsync(
        string email,
        string password,
        string walletId,
        string address,
        string amount,
        string? blockchainFee,
        string? contractAddress,
        string? toContractAddress,
        int? toCurrencyId,
        string? memo,
        string environmentRootUrl,
        bool feeSubtractedFromAmount = false)
        {
            var token = await GetAccessToken(email, password, environmentRootUrl);
            var url = $"{environmentRootUrl}/api/v1/wallets/{walletId}/spend";

            var request = new Models.Spend.WalletSpendRequestDto
            {
                Recipients = new Models.Spend.WalletSpendRequestRecipientDto[]
                {
                new Models.Spend.WalletSpendRequestRecipientDto
                {
                    Address = address,
                    Amount = amount,
                    BlockChainFee = blockchainFee
                }
                },
                ToCurrency = toCurrencyId,
                ContractAddress = contractAddress,
                ToContractAddress = toContractAddress,
                Memo = memo,
                FeeSubtractedFromAmount = feeSubtractedFromAmount
            };

            return await RequestWithAuth<WalletSpendRequestResponseDto>(url, token!.AccessToken, HttpMethod.Post, request);
        }

        public async Task<bool> SubmitSignedSpendRequestAsync(
            string email,
            string password,
            string walletId,
            string spendRequestId,
            string spendRequestToken,
            string environmentRootUrl,
            string? otp)
        {
            var token = await GetAccessToken(email, password, environmentRootUrl);
            var url = $"{environmentRootUrl}/api/v1/wallets/{walletId}/spend/{spendRequestId}/broadcast";

            var request = new ApiClientModels.WalletSpendRequestBroadcastDto
            {
                Otp = otp ?? "000000",
                SignedTxHex = walletId,
                SpendRequestToken = spendRequestToken
            };

            var result = await RequestWithAuth(url, token!.AccessToken, HttpMethod.Post, request);
            return result;
        }
        public async Task<ApiClientModels.WalletSpend2FAChallengeResponseDto> SpendRequest2FaChallengeAsync(
            string email,
            string password,
            string spendRequestId,
            string walletId,
            string address,
            string walletLabel,
            string environmentRootUrl,
            string? withdrawalDisplayAmount)
        {
            var token = await GetAccessToken(email, password, environmentRootUrl);
            var url = $"{environmentRootUrl}/api/v1/wallets/twofa/{spendRequestId}";

            var request = new ApiClientModels.WalletSpendRequest2FADto
            {
                Addresses = new string[]
                {
                    address
                },
                WalletId = Guid.Parse(walletId),
                WalletLabel = walletLabel,
                WithdrawalDisplayAmount = withdrawalDisplayAmount
            };

            var response = await RequestWithAuth<ApiClientModels.WalletSpend2FAChallengeResponseDto>(url, token!.AccessToken, HttpMethod.Post, request);
            return response;
        }
        private async Task<T> RequestWithAuth<T>(string url, string accessToken, HttpMethod method, object? rawContent = null)
        {
            using var httpClient = new HttpClient();
            HttpResponseMessage response = null;

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            options.Converters.Add(new JsonStringEnumConverter());
            var json = rawContent is null ? null : JsonSerializer.Serialize(rawContent, options);
            var content = json is null ? new StringContent(string.Empty) : new StringContent(json) { Headers = { ContentType = new MediaTypeHeaderValue("application/json") } };

            using (var requestMessage = new HttpRequestMessage(method, url))
            {
                requestMessage.Headers.Add("Authorization", $"Bearer {accessToken}");
                requestMessage.Content = content;

                response = await httpClient.SendAsync(requestMessage);
                if (!response.IsSuccessStatusCode)
                {
                    var strContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"{method.Method} {url}. Returned {response.StatusCode}, with content: '{strContent}'");
                }
            }

            return JsonSerializer.Deserialize<T>(await response.Content.ReadAsStringAsync(), options)!;
        }

        private async Task<bool> RequestWithAuth(string url, string accessToken, HttpMethod method, object? rawContent = null)
        {
            using var httpClient = new HttpClient();
            HttpResponseMessage response = null;

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            options.Converters.Add(new JsonStringEnumConverter());
            var json = JsonSerializer.Serialize(rawContent, options);
            var content = new StringContent(json) { Headers = { ContentType = new MediaTypeHeaderValue("application/json") } };

            using (var requestMessage = new HttpRequestMessage(method, url))
            {
                requestMessage.Headers.Add("Authorization", $"Bearer {accessToken}");
                requestMessage.Content = content;

                response = await httpClient.SendAsync(requestMessage);
                if (!response.IsSuccessStatusCode)
                {
                    var strContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"{method.Method} {url}. Returned {response.StatusCode}, with content: '{strContent}'");
                }
            }

            return true;
        }

    }
}
