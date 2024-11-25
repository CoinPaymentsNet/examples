namespace Shared.Models
{

    public static class Currencies
    {
        public static Currency LTCT = new Currency
        {
            Id = 1002,
            Name = "Litecoin Test",
            DecimalPlaces = 8,
            Symbol = "LTCT",
            IsUTXO = true,
        };

        public static Currency ETH = new Currency
        {
            Id = 4,
            Name = "Ethereum",
            DecimalPlaces = 18,
            Symbol = "ETH"
        };

        public static Currency BTC = new Currency()
        {
            Id = 1,
            Name = "Bitcoin",
            DecimalPlaces = 8,
            Symbol = "BTC"
        };

        public static Currency USDT_ERC20 = new Currency()
        {
            Id = 4,
            ContractAddress = "0xdac17f958d2ee523a2206206994597c13d831ec7",
            Name = "Tether USD",
            Symbol = "USDT.ERC20",
            DecimalPlaces = 6
        };

        public static Currency USD = new Currency
        {
            Id = 5057,
            Name = "United States Dollar",
            Symbol = "USD",
            DecimalPlaces = 2,
            // HasTokens = false,
            // IsBtcClone = false,
            // IsCrypto = false,
            // IsUtxo = false,
            // IsMainnet = true,
            // SupportsIdempotence = false,
        };

        public static Dictionary<int, Currency> All { get; set; } = new()
    {
        { LTCT.Id, LTCT },
        { ETH.Id, ETH },
        { BTC.Id, BTC },
        { USD.Id, USD }
    };

        public static string GetId(this Currency c) =>
            string.IsNullOrEmpty(c.ContractAddress) ? c.Id.ToString() : $"{c.Id}:{c.ContractAddress}";

        public static (string Symbol, int Decimals) ToSymbolWithDecimals(this int id)
        {
            var exists = All.TryGetValue(id, out var result);
            return (exists
                ? (result.Symbol, result.DecimalPlaces)
                : throw new ArgumentException($"Unknown currency id {id}"))!;
        }

        public static decimal ToFormattedValueFromSmallestUnit(int currencyId, decimal value)
        {
            var decimalPlaces = currencyId.ToSymbolWithDecimals().Decimals;
            var multiplier = (decimal)Math.Pow(10, decimalPlaces);
            return Math.Round(value / multiplier, decimalPlaces);
        }

        public static string CoinValueFromSmallestUnit(int currencyId, decimal value)
        {
            var coin = currencyId.ToSymbolWithDecimals();
            var multiplier = (decimal)Math.Pow(10, coin.Decimals);
            return $"{Math.Round(value / multiplier, coin.Decimals)} {coin.Symbol}";
        }

        public static string CoinValueFromDisplayUnit(int currencyId, decimal value)
        {
            var coin = currencyId.ToSymbolWithDecimals();
            var multiplier = (decimal)Math.Pow(10, coin.Decimals);
            return $"{Math.Round(value * multiplier, coin.Decimals)}";
        }

        public static string CoinValueWithSymbolFromDisplayUnit(int currencyId, decimal value)
        {
            var coin = currencyId.ToSymbolWithDecimals();
            var multiplier = (decimal)Math.Pow(10, coin.Decimals);
            return $"{Math.Round(value * multiplier, coin.Decimals)} {coin.Symbol}";
        }

        public static string CoinValueFromSmallestUnit(int currencyId, string value) =>
            CoinValueFromSmallestUnit(currencyId, Convert.ToDecimal(value));

        /// <summary>
        /// Converts from the smallest monetary unit back to a formatted value 
        /// </summary>
        public static decimal ToFormattedTokenValueFromSmallestUnit(int tokenDecimals, decimal value)
        {
            var multiplier = (decimal)Math.Pow(10, tokenDecimals);
            return Math.Round(value / multiplier, tokenDecimals);
        }
    }

    public class RequiredConfirmationDto
    {
        public int CurrencyId { get; set; }
        public ulong ConfirmationsCount { get; set; }
    }

    public sealed class Currency
    {
        public int Id { get; set; }

        public CurrencyType Type { get; set; }

        public string Symbol { get; set; } = null!;

        public string Name { get; set; } = null!;

        public int DecimalPlaces { get; set; }

        public int Rank { get; set; }

        public CurrencyStatus Status { get; set; }

        public ulong RequiredConfirmations { get; set; }
        public bool IsUTXO { get; set; }
        public string ContractAddress { get; set; }
    }

    public sealed class AppCurrency
    {
        public string Id { get; set; } = null!;

        public CurrencyType Type { get; set; }

        public string Symbol { get; set; } = null!;

        public string Name { get; set; } = null!;

        public int DecimalPlaces { get; set; }

        public int Rank { get; set; }

        public CurrencyStatus Status { get; set; }

        public ulong RequiredConfirmations { get; set; }
    }

    public enum CurrencyStatus
    {
        Active,
        UnderMaintenance,
        Deleted,
    }

    public enum CurrencyType
    {
        Crypto,
        Token,
        Fiat
    }
}
