using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Shared.Models
{

    public class PagedWalletsDto : PagedItemsDto<WalletDto>
    {
    }

    /// <summary>
    /// Represents a financial wallet on the CoinPayments® platform.
    /// </summary>
    public sealed class WalletDto : RootWallet
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
        public bool IsMainCurrency() => !ConfirmedTokens.Any();
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


    public class MoneyDto
    {
        public string DisplayValue { get; set; }
        public string Value { get; set; }
        public string CurrencyId { get; set; }
    }
    public class TokenDto : MoneyDto
    {
        /// <summary>
        /// Blockchain address of the contract representing the token
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public required string ContractAddress { get; set; }

        /// <summary>
        /// Token amount equivalent in native currency
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public required string NativeValue { get; set; }

        public int NativeCurrencyId { get; set; } = 5057; // USD
    }

    public class RootWallet
    {
        /// <summary>
        /// the unique id of the wallet
        /// </summary>
        /// <example>283b271a2db744c0b10c</example>
        public required string WalletId { get; set; }
        /// <summary>
        /// the user or system defined name of the wallet
        /// </summary>
        /// <example>My BitCoin Wallet 😀</example>
        public required string Label { get; set; }

        /// <summary>
        /// the confirmed amount of funds available
        /// </summary>
        public required MoneyDto ConfirmedBalance { get; set; }
        /// <summary>
        /// the id of the currency this wallet holds
        /// </summary>
        /// <example>1235</example>
        public int CurrencyId { get; set; }

        public virtual bool IsMain { get; set; }
    }
    public class MerchantWallet : RootWallet
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

        public Guid AddressId { get; set; }

        /// <summary>
        /// the current (last) deposit address for the wallet (for crypto currency wallets)
        /// </summary>
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
        public string? ContractAddress { get; set; }

        public override bool IsMain { get; set; } = false;
        public bool HasPermanentAddresses { get; set; }
    }

    public class NewWalletRequest
    {
        public string CurrencyId { get; set; }

        public string Label { get; set; } = null!;

        public string? WebhookUrl { get; set; }

        public string? ContractAddress { get; set; }
        public bool UsePermanentAddresses { get; set; }
    }

    public class NewWalletResponse
    {
        public required string WalletId { get; set; }

        public required string Address { get; set; }
    }

    public class UpdateWalletWebhookRequestDto
    {
        public string? NotificationUrl { get; set; }
    }

    public class BlockchainLatestBlockNumberDto
    {
        public ulong LatestBlockNumber { get; set; }
    }

    public class MigrateUserWalletRequestDto
    {
        public string CurrencyCode { get; set; }

        public string Label { get; set; }

        public uint UserId { get; set; }

        public uint WalletId { get; set; }

        public ulong StartFromBlock { get; set; }

        public string BalanceInSmallestUnits { get; set; }

        public MigratedWalletAddressDto[]? Addresses { get; set; }

        public MigrateUserWalletTransactionDto[]? Transactions { get; set; }
    }

    public class MigrateUserInvoiceDto
    {
        public string? SmartContract { get; set; }

        public DateTime? CompletedAt { get; set; }

        public DateTime? ConfirmedAt { get; set; }

        public string Description { get; set; } = null!;

        public string NativeCurrencyCode { get; set; } = null!;

        public decimal? AmountInSmallestUnitsInNativeCurrency { get; set; }

        public decimal? TaxInNativeCurrency { get; set; }

        public decimal? ShippingInNativeCurrency { get; set; }
    }

    public class CreateWalletAddressRequestDto
    {
        public string? Label { get; set; }

        public AddressType Type { get; set; }

        public string? NotificationUrl { get; set; }
    }

    public sealed class CreateWalletAddressResponseDto
    {
        public required string AddressId { get; set; }

        public required string NetworkAddress { get; set; }
        public string? WalletId { get; set; }
    }

    public class MigratedWalletAddressDto
    {
        public string? Label { get; set; } = null!;

        public required string Address { get; set; }

        public DateTime DateCreated { get; set; }

        public AddressType AddressType { get; set; }

        public string? NotificationUrl { get; set; }
    }

    public enum AddressType
    {
        /// <summary>
        /// General-purpose address used for common operations
        /// </summary>
        Personal = 0,

        /// <summary>
        /// Commercial address that can send webhooks and can have special fee rules applied
        /// </summary>
        Commercial = 1
    }

    public class MigrateUserWalletTransactionDto
    {
        public string? TxHash { get; set; }

        public required string ToAddress { get; set; }

        public string? FromAddress { get; set; }

        public required string AmountWithoutFeesInSmallestUnits { get; set; }

        public bool IsReceive { get; set; }

        public bool IsInternal { get; set; }

        public string? BlockchainFeeInSmallestUnits { get; set; }

        public string? CoinpaymentsFeeInSmallestUnits { get; set; }

        public int? OutputIndex { get; set; }

        public ulong? BlockNumberTxAppearedAt { get; set; }

        public DateTime DateCreated { get; set; }

        public string? Memo { get; set; }
        public required string TransactionId { get; set; }

        public bool IsInvoice { get; set; }

        public string? PaymentType { get; set; }
        public MigrateUserInvoiceDto? InvoiceInfo { get; set; }
    }

    public class ChangeAddress
    {
        public string Address { get; set; } = null!;
        public string AddedByUser { get; set; } = null!;
        public DateTime DateAdded { get; set; }
    }

    public class WalletSpendRequestDto
    {
        public string ToAddress { get; set; } = null!;

        public int ToCurrencyId { get; set; }

        public string? FromContractAddress { get; set; }

        public string? ToContractAddress { get; set; }

        public string AmountInSmallestUnits { get; set; }

        public decimal? BlockchainFeeOverrideInSmallestUnits { get; set; }

        public string? Memo { get; set; }

        public bool ReceiverPaysFee { get; set; }
    }

    public class WalletSpendResponseDto
    {
        public Guid SpendRequestId { get; set; }
        public decimal BlockchainFee { get; set; }
        public decimal CoinpaymentsFee { get; set; }
        public decimal FromAmount { get; set; }
        public decimal ToAmount { get; set; }
    }

    public class WalletSpendConfirmationRequestDto
    {
        public Guid SpendRequestId { get; set; }
    }

    public class WalletInfoDto
    {
        public Guid WalletId { get; set; }

        public string Label { get; set; } = null!;

        public int CurrencyId { get; set; }

        public string Balance { get; set; }

        public DateTime UpdatedAt { get; set; }

        public Dictionary<string, string> TokenBalances { get; set; } = null!;

        public string DepositAddress { get; set; } = null!;

        public WalletStatus WalletStatus { get; set; }

        public bool CanCreateAddresses { get; set; }
    }

    public class WalletTransactionDto
    {

        public Guid Id { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime? DateCompleted { get; set; }

        public Guid? FromOwnerId { get; set; }

        public Guid? FromWalletId { get; set; }

        public Guid? ToWalletId { get; set; }

        public Guid? SpendRequestId { get; set; }

        public int? FromCurrencyId { get; set; }

        public int ToCurrencyId { get; set; }

        public string FromAmount { get; set; }

        public string ToAmount { get; set; }

        public string CoinpaymentsFee { get; set; }

        public WalletTransactionStatus TransactionStatus { get; set; }

        public WalletTransactionType TransactionType { get; set; }

        public string Memo { get; set; }

        public string FromAddress { get; set; }

        public string ToAddress { get; set; } = null!;

        public string TxHash { get; set; }

        public int? OutputIndex { get; set; }

        public string BlockchainFee { get; set; }

        public string FromContractAddress { get; set; }

        public string ToContractAddress { get; set; }

        public string BlockchainFeeCurrency { get; set; } = null!;

        public string CoinPaymentsFeeCurrency { get; set; } = null!;

        public ulong? BlockNumberTxAppearedAt { get; set; }

        public required int Confirmations { get; set; }

        public required int RequiredConfirmations { get; set; }
        public string? FromAmountNative { get; set; }

        public string? ToAmountNative { get; set; }

        public string? CoinpaymentsFeeNative { get; set; }

        public string? BlockchainFeeNative { get; set; }

        public bool IsInvoicePaymentSend { get; set; }

        public string? PaymentType { get; set; }
    }

    public enum WalletTransactionType
    {
        Unknown = 0,
        InternalReceive = 1,
        UtxoExternalReceive = 2,
        AccountBasedExternalReceive = 3,
        ExternalSpend = 4,
        InternalSpend = 5,
        SameUserSpend = 6,
        SameUserReceive = 7,
        AccountBasedExternalTokenReceive = 8,
        AccountBasedTokenSpend = 9,
        Conversion = 10,
        Compensation = 11,
        Sweeping = 12,
        SweepingFunding = 13,
    }

    public enum WalletTransactionStatus
    {
        Unknown = 0,
        Created = 1,
        Pending = 2,
        Processing = 3,
        Completed = 4,
        Expired = 5,
        Failed = 6,
        ConfirmedOnBlockchain = 7,
        PendingReceive = 8,
        FailedOnBlockchain = 9,
    }

    public class UserSetting
    {
        public uint UserId { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }
    }

    public class UserSettingsDto
    {
        public string Key { get; set; }

        public string Value { get; set; }
    }

    public class MigratedUserSettingsDto : UserSettingsDto
    {
        public Guid UserId { get; set; }
    }

    public enum WalletStatus
    {
        Unknown = 0,

        Created = 1,

        Active = 2,

        Deactivated = 3,

        Deleted = 4
    }
    public sealed class WalletAddressDto
    {
        /// <summary>
        /// the unique id of the address
        /// </summary>
        public required string AddressId { get; set; }

        /// <summary>
        /// the crypto network deposit address
        /// </summary>
        public required string Address { get; set; }

        /// <summary>
        /// the user supplied label for the address
        /// </summary>
        public required string Label { get; set; }

        /// <summary>
        /// the timestamp when the address was created
        /// </summary>
        public DateTimeOffset Created { get; set; }

        /// <summary>
        /// when there is transaction involved with this address, we can send notification to this url
        /// </summary>
        public string? NotificationUrl { get; set; }

        /// <summary>
        /// Indicates a date, when this address is going to be returned to the pool
        /// (only relevant for AccountBased coins, that are using address pools)
        /// </summary>
        public DateTime? RentedTill { get; set; }
    }

    public class WalletConsolidationDto
    {
        public required string NewReceivedInternal { get; set; }

        public required string NewReceivedExternal { get; set; }

        public required string ActivationFee { get; set; }

        public required string TransferFee { get; set; }

        public required string TotalFee { get; set; }

        public required string Available { get; set; }

        public required AddressConsolidationDto[] Addresses { get; set; }
    }

    public class AddressConsolidationDto
    {
        public required string NewReceivedInternal { get; set; }

        public required string NewReceivedExternal { get; set; }

        public required string ActivationFee { get; set; }

        public required string TransferFee { get; set; }

        public required string TotalFee { get; set; }

        public required string Available { get; set; }

        public required string Address { get; set; }

        public required Guid AddressId { get; set; }

        public required Guid WalletId { get; set; }
    }

    public class WalletsConsolidationRequestDto
    {
        public required WalletConsolidationRequestDto[] Wallets { get; set; }
    }

    public class WalletConsolidationRequestDto
    {
        public required Guid WalletId { get; set; }
        public required Guid[]? Addresses { get; set; }
    }
}
