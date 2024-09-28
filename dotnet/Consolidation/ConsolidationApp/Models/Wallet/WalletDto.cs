using ConsolidationApp.Models;
using ConsolidationApp.Models.Money;
using System.Text.Json.Serialization;

namespace ConsolidationApp.Models.Wallet;

 public class PagedWalletsDto : PagedItemsDto<WalletDto>
 {
 }

 /// <summary>
 /// Represents a financial wallet on the CoinPayments® platform.
 /// </summary>
 public sealed class WalletDto:RootWallet
 {

     /// <summary>
     /// the type of the wallet
     /// </summary>
     public WalletType WalletType { get; set; }

     /// <summary>
     /// Indicates whether this wallet is active (not deleted or deactivated)
     /// </summary>
     /// <remarks>We need this property to support listing transactions to/from deactivated wallets</remarks>
     public bool IsActive { get; set; }

     /// <summary>
     /// Indicates whether this wallet is locked due to legal reasons
     /// </summary>        
     public bool IsLocked { get; set; }

     /// <summary>
     /// the currency this walllet holds
     /// </summary>
     [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
     public CurrencyDto? Currency { get; set; }

     /// <summary>
     /// the current (last) deposit address for the wallet (for crypto currency wallets)
     /// </summary>
     [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
     public string? DepositAddress { get; set; }

     /// <summary>
     /// the unconfirmed amount of received funds waiting for blockchain confirmations
     /// </summary>
     public required MoneyDto UnconfirmedBalance { get; set; }

     /// <summary>
     /// the unconfirmed balance in the wallet, in the users native currency
     /// </summary>
     public required MoneyDto UnconfirmedNativeBalance { get; set; }

     /// <summary>
     /// the amount of funds available for transactions, in the users native currency
     /// </summary>
     public required MoneyDto ConfirmedNativeBalance { get; set; }

     /// <summary>
     /// the amount of tokens (like ERC20) available for this wallet
     /// </summary>
     public required TokenDto[] ConfirmedTokens { get; set; }

     /// <summary>
     /// the amount of tokens (like ERC20) pending for this wallet
     /// </summary>
     public required TokenDto[] UnconfirmedTokens { get; set; }

     /// <summary>
     /// flag that determines ability to create multiple addresses for same wallet
     /// </summary>
     public bool CanCreateAddresses { get; set; }

     /// <summary>
     /// flag that determines if the wallet vault is locked or not
     /// </summary>
     public bool IsVaultLocked { get; set; }

     /// <summary>
     /// DateTime that specifies the planned date the vault will be unlocked
     /// </summary>
     public DateTime? VaultLockoutEndDateTime { get; set; }

     /// <summary>
     /// An optional address of the smart contract representing a token
     /// </summary>
     [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
     public string? ContractAddress { get; set; }

     public override bool IsMain { get; set; } = true;
     public bool IsMainCurrency()=>!ConfirmedTokens.Any();
 }
 public sealed class CurrencyDto
 {
     public string Id { get; set; }
 }
 /// <summary>
 /// Monetary value (an amount with a currency and contract address).
 /// </summary>

 [JsonConverter(typeof(JsonStringEnumConverter))]
 public enum WalletType
 {
     /// <summary>
     /// Single-Signature crypto currency wallet account, where CoinPayments holds private keys
     /// </summary>
     SingleSigCryptoWallet
 }
public sealed class PagedWalletAddressesDto : PagedItemsDto<WalletAddressDto>
{
}
public class WalletInfo
{
    public Guid OwnerId { get; set; }

    public Guid WalletId { get; set; }

    public string Label { get; set; } = null!;

    public int CurrencyId { get; set; }

    public decimal Balance { get; set; }

    public decimal UnconfirmedBalance { get; set; }

    public DateTime UpdatedAt { get; set; }

    public Dictionary<string, decimal> TokenBalances { get; set; } = null!;

    public string DepositAddress { get; set; } = null!;

    public WalletStatus WalletStatus { get; set; }

    public bool CanCreateAddresses { get; set; }
}