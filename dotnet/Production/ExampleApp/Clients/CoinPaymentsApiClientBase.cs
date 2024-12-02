using Shared;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ExampleApp.Clients
{
    public partial class CoinPaymentsApiClient
    {
        const string CoinPaymentsApiClientHeaderName = "X-CoinPayments-Client";
        const string CoinPaymentsApiSignatureHeaderName = "X-CoinPayments-Signature";
        const string CoinPaymentsApiTimestampHeaderName = "X-CoinPayments-Timestamp";
        private string API_URL;
        private JsonSerializerOptions Options;
        private HttpClient HttpClient;
        public readonly ClientEnvironmentModel CurrentClient;
        public CoinPaymentsApiClient(string apiRootUrl, ClientEnvironmentModel currentClient)
        {
            Options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            HttpClient = new HttpClient()
            {
                //Timeout = TimeSpan.FromMilliseconds(1)
            };
            CurrentClient = currentClient;
            API_URL = apiRootUrl ??
                      throw new ArgumentException("Production API URL not defined");
        }

        public async Task<(int StatusCode, string Content)> ExecuteAsync(string url, HttpMethod method,
            object? body = null)
        {
            url = $"{API_URL}/{url}";
            try
            {
                if (!Options.Converters.Any(x => x.GetType() == typeof(JsonStringEnumConverter)))
                    Options.Converters.Add(new JsonStringEnumConverter());
                var json = JsonSerializer.Serialize(body, Options);
                var request = new HttpRequestMessage(method, url);
                if (body is not null)
                {
                    request.Content = JsonContent.Create(body, options: Options);
                }

                var result = await HttpClient.SendAsync(request);
                result.EnsureSuccessStatusCode();
                var content = await result.Content.ReadAsStringAsync();
                return ((int)result.StatusCode, content);
            }
            catch (HttpRequestException e)
            {
                var ex = new HttpRequestException($"Task canceled on url {url} with Status {e.StatusCode}", e);
                throw ex;
            }
            catch (TaskCanceledException e)
            {
                var ex = new TaskCanceledException($"Task canceled with url {url}", e);
                throw ex;
            }
        }

        public async Task<(int StatusCode, string Content)> AuthExecuteAsync(string url, HttpMethod method,
            string clientId, string clientSecret, object? body = null, CancellationToken ct = default)
        {
            HttpResponseMessage? result = null;
            url = $"{API_URL}/{url}";
            try
            {
                if (!Options.Converters.Any(x => x.GetType() == typeof(JsonStringEnumConverter)))
                    Options.Converters.Add(new JsonStringEnumConverter());
                var json = body != null ? JsonSerializer.Serialize(body, Options) : null;
                SignRequest(url, clientId, clientSecret, method, json);

                var request = new HttpRequestMessage(method, url);
                if (body is not null)
                {
                    request.Content = JsonContent.Create(body, options: Options);
                }

                result = await HttpClient.SendAsync(request, ct);
                result.EnsureSuccessStatusCode();
                var content = await result.Content.ReadAsStringAsync();
                return ((int)result.StatusCode, content);
            }
            catch (HttpRequestException e)
            {
                var errorMsg = $"Task canceled on url {url} with Status {e?.StatusCode}\n";
                if (result != null)
                {
                    errorMsg += $"Body: {await result?.Content?.ReadAsStringAsync() ?? ""}";
                }

                throw new HttpRequestException(errorMsg, e);
            }
            catch (TaskCanceledException e)
            {
                throw new TaskCanceledException($"Task canceled with url {url}", e);
            }
        }

        public async Task<T> AuthExecuteAsync<T>(string url, HttpMethod method, string clientId, string clientSecret,
            object? body = null, CancellationToken ct = default)
        {
            var result = await AuthExecuteAsync(url, method, clientId, clientSecret, body, ct);
            return JsonSerializer.Deserialize<T?>(result.Content, Options)!;
        }

        public async Task<T> ExecuteAsync<T>(string url, HttpMethod method, object? body = null)
        {
            var result = await ExecuteAsync(url, method, body);
            return JsonSerializer.Deserialize<T?>(result.Content, Options)!;
        }

        void SignRequest(string url, string clientId, string clientSecret, HttpMethod httpMethod, string? json = null)
        {
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(clientSecret)))
            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream, Encoding.UTF8))
            {
                var timestampHeader = DateTime.UtcNow.ToString("s");
                writer.Write(httpMethod.ToString());
                writer.Write(url);
                writer.Write(clientId);
                writer.Write(timestampHeader);
                if (!string.IsNullOrEmpty(json))
                    writer.Write(json);
                writer.Flush();

                stream.Position = 0;

                var hashBytes = hmac.ComputeHash(stream);
                var hash = Convert.ToBase64String(hashBytes);

                HttpClient.DefaultRequestHeaders.Clear();
                HttpClient.DefaultRequestHeaders.Add(CoinPaymentsApiClientHeaderName, clientId);
                HttpClient.DefaultRequestHeaders.Add(CoinPaymentsApiSignatureHeaderName, hash);
                HttpClient.DefaultRequestHeaders.Add(CoinPaymentsApiTimestampHeaderName, timestampHeader);
            }
        }
    }

}