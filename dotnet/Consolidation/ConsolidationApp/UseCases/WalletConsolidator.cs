using ConsolidationApp.Helpers;
using ConsolidationApp.Models;
using ConsolidationApp.Models.Currencies;
using ConsolidationApp.Models.Wallet;
using ConsolidationApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace ConsolidationApp.UseCases
{
    public class WalletConsolidator
    {
        private UserService _userService { get; set; }
        private MerchantService _merchantService { get; set; }
        public WalletConsolidator(ClientEnvironmentModel clientModel)
        {
            ClientModel = clientModel;
            _userService = new UserService();
            _merchantService = new MerchantService();
        }
        private ClientEnvironmentModel? ClientModel { get; set; }
        private PagedWalletsDto CurrentUserWallets { get; set; } = new() { Items = Array.Empty<WalletDto>() };
        private MerchantWallet[] CurrentMerchantWallets { get; set; } = Array.Empty<MerchantWallet>();
        private HashSet<RootWallet> UsableSoruceWallets = [];

        async Task GetAllWallets()
        {
            CurrentMerchantWallets = (await _merchantService.GetAllMerchantWallets(ClientModel!.Id,
                ClientModel!.Secret, ClientModel.ToUseEndpoints.ORION_API_URL)).OrderByDescending(x => long.TryParse(x.Label, out var _)).ToArray();
            CurrentUserWallets = await _userService.GetWallets(ClientModel.Email, ClientModel.Password, ClientModel.ToUseEndpoints.ORION_API_URL);

            UsableSoruceWallets = (CurrentMerchantWallets.Where(x => Convert.ToInt64(x.ConfirmedBalance.Value) > 0 && (x.CurrencyId == Currencies.LTCT.Id || x.CurrencyId == Currencies.ETH.Id)).Select(x => new RootWallet()
            {
                WalletId = x.WalletId,
                Label = x.Label,
                CurrencyId = x.CurrencyId,
                ConfirmedBalance = x.ConfirmedBalance,
                IsMain = x.IsMain
            }).Union(CurrentUserWallets?.Items?.Where(x => Convert.ToInt64(x.ConfirmedBalance.Value) > 0 && (x.CurrencyId == Currencies.LTCT.Id || x.CurrencyId == Currencies.ETH.Id))?.Select(x => new RootWallet()
            {
                WalletId = x.WalletId,
                Label = x.Label,
                CurrencyId = x.CurrencyId,
                ConfirmedBalance = x.ConfirmedBalance,
                IsMain = x.IsMain
            }) ?? [])).ToHashSet();
        }

        public async Task CreatePermanentAddress_TransferFromMain_ConsolidateAndSpendETH(int currencyId)
        {
            Utilites.AddInfo("Starting consolidation flow..");
            var transferAmount = 374_007_000_000_000; // approximately 1$ amount as ETH
            var merchantService = new MerchantService();
            await GetAllWallets();
            var defaultMainWallet = CurrentUserWallets.Items.FirstOrDefault(x => x.CurrencyId == currencyId);
            var mainWalletId = Guid.Parse(defaultMainWallet.WalletId);

            var permanentAddress = await CreateMerchantPermanentAddress(currencyId);
            await Task.Delay(1000);
            await GetAllWallets();
            var targetPermanentAddress = GetMerchantWalletBy(x => x.DepositAddress == permanentAddress.NetworkAddress);

            var validationResult = await _userService.ValidateSpendFromWalletAsync(ClientModel.Email, ClientModel.Password, mainWalletId.ToString(), permanentAddress.NetworkAddress, transferAmount.ToString(),
                null,
                null,
                null,
                null,
                null,
                ClientModel.ToUseEndpoints.ORION_API_URL);
            if (validationResult is null || !validationResult.IsValid)
            {
                Utilites.AddInfo($"Validation for spend request from wallet {mainWalletId} failed", InfoType.Error);
                return;
            }
            var spendRequestResponse = await _userService.RequestSpendFromWalletAsync(
                ClientModel.Email, ClientModel.Password,
                mainWalletId.ToString(),
                permanentAddress.NetworkAddress,
                transferAmount.ToString(),
                null,
                null,
                null,
                null,
                null,
                ClientModel.ToUseEndpoints.ORION_API_URL);
            if (spendRequestResponse is null)
            {
                Utilites.AddInfo($"Request for spend request from wallet {mainWalletId} failed", InfoType.Error);
                return;
            }

            var twoFaChallengeResponse = await _userService.SpendRequest2FaChallengeAsync(
                ClientModel.Email, ClientModel.Password,
                spendRequestResponse.Id,
                mainWalletId.ToString(),
                spendRequestResponse.ToAddress,
                spendRequestResponse.WalletLabel,
                ClientModel.ToUseEndpoints.ORION_API_URL,
                null);

            if (twoFaChallengeResponse is null)
            {
                Utilites.AddInfo($"SpendRequest2FaChallengeAsync for spend request from wallet {mainWalletId} failed", InfoType.Error);
                return;
            }

            Utilites.AddInfo($"2FA Challenge created successfully with SpendID:{twoFaChallengeResponse.SpendRequestId} from with Token:{twoFaChallengeResponse.SpendRequestToken.Substring(0, 10) + "..."}");

            int otpAttemptCount = 2;
            bool spendRequestBroadcastedResponse;
            do
            {
                try
                {
                    spendRequestBroadcastedResponse = await _userService.SubmitSignedSpendRequestAsync(
                        ClientModel.Email, ClientModel.Password,
                        mainWalletId.ToString(),
                        twoFaChallengeResponse.SpendRequestId,
                        twoFaChallengeResponse.SpendRequestToken,
                        ClientModel.ToUseEndpoints.ORION_API_URL,
                        GetOTPFromUser());
                }
                catch (Exception e)
                {
                    Utilites.AddInfo("Failed OTP verification please retry", InfoType.Error);

                    spendRequestBroadcastedResponse = false;
                }
            } while (!spendRequestBroadcastedResponse && --otpAttemptCount >= 0);

            if (!spendRequestBroadcastedResponse)
            {
                Utilites.AddInfo("Failed OTP verification", InfoType.Error);
                return;
            }

            Utilites.AddInfo($"Balances before transfer");
            Utilites.AddInfo($"Spend request successfully completed. \n SpendID:{twoFaChallengeResponse.SpendRequestId} \n MainWalletId: {mainWalletId} \n MerchantWalletAddress:{permanentAddress.NetworkAddress}");

            await Task.Delay(2000);

            var actualSourceBalance = Convert.ToInt64(defaultMainWallet.ConfirmedBalance.Value) - (transferAmount + Convert.ToInt64(spendRequestResponse.BlockchainFee) + Convert.ToInt64(spendRequestResponse.CoinPaymentsFee));
            var actualTargetBalance = Convert.ToInt64(targetPermanentAddress.ConfirmedBalance.Value) + transferAmount;

            await GetAllWallets();
            targetPermanentAddress = GetMerchantWalletBy(x => x.DepositAddress == permanentAddress.NetworkAddress);
            defaultMainWallet = GetUserWalletBy(x => x.WalletId == mainWalletId.ToString());


            if (Convert.ToDecimal(defaultMainWallet.ConfirmedBalance.Value) == actualSourceBalance
                && Convert.ToDecimal(targetPermanentAddress.ConfirmedBalance.Value) == actualTargetBalance)
            {
                //some log for successfully consolidation
            }
            else
            {
                throw new Exception("Transfer main to merchant failed!");
            }

            var temporaryWallet = await CreateWalletWithAddressAndRefreshData(currencyId); //Temporary wallet


            await GetAllWallets();

            var currentMainWalletsAmount = Convert.ToInt64(defaultMainWallet.ConfirmedBalance.Value);

            var mainWalletOldAmount = Convert.ToInt64(defaultMainWallet.ConfirmedBalance.Value);

            var expectedAmount = await ConsolidateWallet(permanentAddress.WalletId, permanentAddress.NetworkAddress, Guid.Parse(temporaryWallet.WalletId));


            await Task.Delay(1000);

            await GetAllWallets();

            var temporaryWalletAfterConsolidation = CurrentMerchantWallets.FirstOrDefault(x => x.WalletId == temporaryWallet.WalletId);
            if (Convert.ToInt64(temporaryWalletAfterConsolidation.ConfirmedBalance.Value) == expectedAmount)
            {
                Utilites.AddInfo($"Consolidated wallets successfuly to {temporaryWalletAfterConsolidation.WalletId} with {Currencies.CoinValueFromSmallestUnit(temporaryWalletAfterConsolidation.CurrencyId, expectedAmount)}");
            }
            else
            {
                Utilites.AddInfo($"Consolidation failed.", InfoType.Error);
            }


            //If want to consolidate to main wallet=>

            await ConsolidateWallet(temporaryWallet.WalletId, temporaryWallet.NetworkAddress, null); //If leave as empty toWalletId amount will consolidate main wallet.

            // consolidated temporary wallet to external=>

            //var spendRequest = await _merchantService.RequestSpendFromMerchantWallet(
            //    ClientModel.Id,
            //    ClientModel.Secret,
            //    currencyId,
            //    Guid.Parse(temporaryWallet.WalletId),
            //    "******", //to wallet address
            //    transferAmount,
            //    ClientModel.ToUseEndpoints.ORION_API_URL);

            //await _merchantService.ConfirmSpendFromMerchantWallet(
            //    ClientModel.Id,
            //    ClientModel.Secret,
            //    Guid.Parse(temporaryWallet.WalletId),
            //    spendRequest.SpendRequestId,
            //    ClientModel.ToUseEndpoints.ORION_API_URL
            //);

        }


        string GetOTPFromUser()
        {
            Console.WriteLine("Please enter OTP code:");
            return Console.ReadLine();
        }

        async Task<decimal> ConsolidateWallet(string permanentWalletId, string networkAddress, Guid? toWalletId = null, bool forceToPermanent = false)
        {
            var formattedOpt = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            var toConsolidateWallet = CurrentMerchantWallets.FirstOrDefault(x => x.WalletId == permanentWalletId);
            if (!toConsolidateWallet.HasPermanentAddresses && !forceToPermanent) // This flow force move balance to main wallet.
            {
                var mainWallet = CurrentUserWallets.Items.FirstOrDefault(x => x.IsMain && x.CurrencyId == toConsolidateWallet.CurrencyId && x.ContractAddress == toConsolidateWallet.ContractAddress);

                var consolidateAmount = Convert.ToDecimal(toConsolidateWallet.ConfirmedBalance.Value);
                var spendRequest = await _merchantService.RequestSpendFromMerchantWallet(
                ClientModel.Id,
                ClientModel.Secret,
                toConsolidateWallet.CurrencyId,
                Guid.Parse(toConsolidateWallet.WalletId),
                mainWallet.DepositAddress,
                consolidateAmount,
                ClientModel.ToUseEndpoints.ORION_API_URL);

                await _merchantService.ConfirmSpendFromMerchantWallet(
                    ClientModel.Id,
                    ClientModel.Secret,
                    Guid.Parse(toConsolidateWallet.WalletId),
                    spendRequest.SpendRequestId,
                    ClientModel.ToUseEndpoints.ORION_API_URL
                );
                await GetAllWallets();
                Utilites.AddInfo($"Consolidation completed to main {Currencies.CoinValueFromSmallestUnit(toConsolidateWallet.CurrencyId, Convert.ToDecimal(toConsolidateWallet.ConfirmedBalance.Value))} with {mainWallet.DepositAddress} address");
                return consolidateAmount;
            }
            Utilites.AddInfo($"Starting consolidation process with {Currencies.CoinValueFromSmallestUnit(toConsolidateWallet.CurrencyId, Convert.ToDecimal(toConsolidateWallet.ConfirmedBalance.Value))}");
            var targetConsolidatedWalletAmountBefore = CurrentUserWallets.Items.FirstOrDefault(x => x.CurrencyId == toConsolidateWallet.CurrencyId && x.IsMainCurrency()).ConfirmedBalance.Value;

            var walletId = Guid.Parse(permanentWalletId);
            var address = (await _merchantService.GetMerchantWalletAddresses(ClientModel.Id, ClientModel.Secret, walletId, ClientModel.ToUseEndpoints.ORION_API_URL)).FirstOrDefault(x => x.Address == networkAddress);

            var previewConsolidation = await _merchantService.GetMultiWalletConsolidation(ClientModel.Id, ClientModel.Secret, ClientModel.ToUseEndpoints.ORION_API_URL,
            new WalletsConsolidationRequestDto() { Wallets = [new() { WalletId = walletId, Addresses = [Guid.Parse(address.AddressId)] }] });
            Utilites.AddInfo($"Previewing consolidation \n {JsonSerializer.Serialize(previewConsolidation, formattedOpt)}");

            var consolidationResponse = await _merchantService.ExecuteMultiWalletConsolidation(ClientModel.Id, ClientModel.Secret, ClientModel.ToUseEndpoints.ORION_API_URL,
                new WalletsConsolidationRequestDto() { Wallets = [new() { WalletId = walletId, Addresses = [Guid.Parse(address.AddressId)] }] }, toWalletId);
            Utilites.AddInfo($"Consolidation execution result \n {JsonSerializer.Serialize(consolidationResponse, formattedOpt)}");


            await GetAllWallets();
            return Convert.ToDecimal(consolidationResponse.Available);
        }
        async Task<CreateWalletAddressResponseDto> CreateMerchantPermanentAddress(int currencyId)
        {
            var newWallet = await _merchantService.CreateMerchantWallet(ClientModel.Id,
                ClientModel.Secret, currencyId, StringExtensions.GetUnixTimeOffset(),
                $"https://{StringExtensions.Random(6)}.{StringExtensions.Random(5)}", null, ClientModel.ToUseEndpoints.ORION_API_URL, true);
            var walletAddress = await _merchantService.CreateMerchantWalletAddress(
                ClientModel.Id,
                ClientModel.Secret,
                Guid.Parse(newWallet.WalletId),
                StringExtensions.Random(6),
                ClientModel.ToUseEndpoints.ORION_API_URL,
                AddressType.Commercial);
            await Task.Delay(2000);

            await GetAllWallets();
            walletAddress.WalletId = newWallet.WalletId;
            return walletAddress;
        }

        async Task<CreateWalletAddressResponseDto> CreateWalletWithAddressAndRefreshData(int currencyId)
        {

            var newWallet = await _merchantService.CreateMerchantWallet(ClientModel.Id,
                ClientModel.Secret, currencyId, StringExtensions.GetUnixTimeOffset(),
                $"https://{StringExtensions.Random(6)}.{StringExtensions.Random(5)}", null, ClientModel.ToUseEndpoints.ORION_API_URL, false);
            var walletAddress = await _merchantService.CreateMerchantWalletAddress(
                ClientModel.Id,
                ClientModel.Secret,
                Guid.Parse(newWallet.WalletId),
                StringExtensions.Random(6),
                ClientModel.ToUseEndpoints.ORION_API_URL,
                AddressType.Commercial);
            await Task.Delay(2000);

            await GetAllWallets();
            walletAddress.WalletId = newWallet.WalletId;
            return walletAddress;
        }
        ///

        MerchantWallet? GetMerchantWalletBy(Expression<Func<MerchantWallet, bool>> exp) => CurrentMerchantWallets.FirstOrDefault(exp.Compile());
        WalletDto? GetUserWalletBy(Expression<Func<WalletDto, bool>> exp) => CurrentUserWallets?.Items.FirstOrDefault(exp.Compile());

    }
}
