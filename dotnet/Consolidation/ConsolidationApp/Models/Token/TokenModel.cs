using System.Text.Json.Serialization;
using static ConsolidationApp.Models.Wallet.Enums;

namespace ConsolidationApp.Models.Token
{
	public class AccessTokenResponseMessageDto
	{
		[JsonPropertyName("access_token")]
		public string AccessToken { get; set; }

		[JsonPropertyName("expires_in")]
		public int ExpiresIn { get; set; }

		[JsonPropertyName("token_type")]
		public string TokenType { get; set; }

		[JsonPropertyName("scope")]
		public string Scope { get; set; }
	}

	public class TokenRequestModel
	{
		public string ClientSecret { get; set; }
		public string ClientId { get; set; }
		public string OAuth2ClientId { get; set; } = "coinpayments-aphrodite";
		public string Scope { get; set; } = "orion";
		public TestingEnvironment Environment { get; set; }

	}
}
