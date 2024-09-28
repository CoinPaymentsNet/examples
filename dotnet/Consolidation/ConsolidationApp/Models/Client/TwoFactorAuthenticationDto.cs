using System.Text.Json.Serialization;

namespace ConsolidationApp.Models.Client
{
    public class TwoFactorAuthenticationDto
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TwoFactorTokenProviderDto TwoFactorTokenProvider { get; set; }
    }
}
