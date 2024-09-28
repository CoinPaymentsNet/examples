using System.Text.Json.Serialization;

namespace ConsolidationApp.Models.Money
{
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
}
