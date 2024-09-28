namespace ConsolidationApp.Models.Currencies;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.AccessControl;

public static class Currencies
{
    public static Currency LTCT = new Currency
    {
        Id = 1002,
        Name = "Litecoin",
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
    public static Dictionary<int,Currency> All { get; set; } = new()
    {
        {LTCT.Id,LTCT},
        {ETH.Id,ETH},
    };
    
    public static (string Symbol, int Decimals) ToSymbolWithDecimals(this int id)
    {
        var exists = Currencies.All.TryGetValue(id, out var result);
        return (exists ? (result.Symbol,result.DecimalPlaces) : throw new ArgumentException($"Unknown currency id {id}"))!;
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
        return  $"{Math.Round(value / multiplier, coin.Decimals)} {coin.Symbol}";
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
    public bool IsUTXO {  get; set; }
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