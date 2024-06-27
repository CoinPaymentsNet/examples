using System.ComponentModel.DataAnnotations;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

const string CoinPaymentsApiClientHeaderName = "X-CoinPayments-Client";
const string CoinPaymentsApiSignatureHeaderName = "X-CoinPayments-Signature";
const string CoinPaymentsApiTimestampHeaderName = "X-CoinPayments-Timestamp";

var options = new JsonSerializerOptions
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
};
using var httpClient = new HttpClient();

var clientId = "your client id"; // e.g. "6cb54412be4748ca867468c9ecb849f1";
var clientSecret = "your client secret"; // e.g. "3HQu/qRqVswyETuFIa+xAD/+X2SBF/wT1PPcXoVKjcs=";

var body = new
{
    Items =
    new[]
    {
        new
        {
            CustomId = "11234112",
            Name = "Deposit 20 EUR currency",
            Quantity = new
            {
                Type = 2,
                Value = "1"
            },
            OriginalAmount = new
            {
                CurrencyId = "5195",
                Value = "2000"
            },
            Amount = new
            {
                CurrencyId = "5195",
                Value = "2000"
            }
        }
    },
    Amount = new
    {
        Breakdown = new
        {
            subtotal = new
            {
                CurrencyId = "5195",
                Value = "2000"
            }
        },
        CurrencyId = "5195",
        Value = "2000"
    }
};

var result = await MerchantExecuteAsync("https://api.coinpayments.com/api/v1/merchant/invoices/buy-now-button", "POST", clientId, clientSecret, body);

Console.WriteLine(result);
Console.ReadLine();

async Task<(int, string)> MerchantExecuteAsync(string url, string method, string clientId, string clientSecret, object? body = null)
{
    var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
    options.Converters.Add(new JsonStringEnumConverter());
    var json = JsonSerializer.Serialize(body, options);
    SignRequest(url, clientId, clientSecret, method, json);

    var request = new HttpRequestMessage(HttpMethod.Parse(method), url);
    if (body is not null)
    {
        request.Content = JsonContent.Create(body, options: options);
    }
    var result = await httpClient.SendAsync(request);
    var content = await result.Content.ReadAsStringAsync();
    return ((int)result.StatusCode, content);
}

void SignRequest(string url, string clientId, string clientSecret, string httpMethod, string? json = null)
{
    using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(clientSecret)))
    using (var stream = new MemoryStream())
    using (var writer = new StreamWriter(stream, Encoding.UTF8))
    {
        var timestampHeader = DateTime.UtcNow.ToString("s");
        writer.Write(httpMethod);
        writer.Write(url);
        writer.Write(clientId);
        writer.Write(timestampHeader);
        if (!string.IsNullOrEmpty(json))
            writer.Write(json);
        writer.Flush();

        stream.Position = 0;

        var hashBytes = hmac.ComputeHash(stream);
        var hash = Convert.ToBase64String(hashBytes);

        httpClient.DefaultRequestHeaders.Clear();
        httpClient.DefaultRequestHeaders.Add(CoinPaymentsApiClientHeaderName, clientId);
        httpClient.DefaultRequestHeaders.Add(CoinPaymentsApiSignatureHeaderName, hash);
        httpClient.DefaultRequestHeaders.Add(CoinPaymentsApiTimestampHeaderName, timestampHeader);
    }
}