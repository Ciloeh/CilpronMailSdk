using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Cilpron.MailSdk.Exceptions;
using Cilpron.MailSdk.Internal;
using Cilpron.MailSdk.Models;
using Polly;
using Polly.Extensions.Http;
using JsonSerializerOptions = Cilpron.MailSdk.Internal.JsonSerializerOptions;


namespace Cilpron.MailSdk.Clients
{
    /// <summary>
    /// Client for interacting with the Cilpron Mail API, with resilient HTTP handling using Polly.
    /// </summary>
    public class CilpronMailClient : ICilpronMailClient
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://mail.cilpron.com/v1/";

        /// <summary>
        /// Initializes the client with an API key for Bearer authentication.
        /// </summary>
        /// <param name="apiKey">The Cilpron API key.</param>
        public CilpronMailClient(string apiKey)
        {
            if (string.IsNullOrEmpty(apiKey))
                throw new ArgumentNullException(nameof(apiKey));

            var handler = new HttpClientHandler();
            _httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri(BaseUrl)
            };
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);
            _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }

        /// <summary>
        /// Initializes the client with a custom HttpClient (e.g., for DI or testing).
        /// </summary>
        /// <param name="httpClient">The pre-configured HttpClient.</param>
        public CilpronMailClient(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            if (_httpClient.BaseAddress == null)
                _httpClient.BaseAddress = new Uri(BaseUrl);
        }

        /// <summary>
        /// Sends an email asynchronously.
        /// </summary>
        /// <param name="request">The mail request model.</param>
        /// <returns>The send response.</returns>
        public async Task<SendMailResponse> SendEmailAsync(MailRequest request)
        {
            RequestValidator.Validate(request);

            var json = JsonSerializer.Serialize(request, JsonSerializerOptions.Default);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var policy = HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

            var response = await policy.ExecuteAsync(() => _httpClient.PostAsync("mail/send", content));

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new CilpronMailException($"Failed to send email: {response.StatusCode} - {errorContent}");
            }

            var responseJson = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<SendMailResponse>(responseJson, JsonSerializerOptions.Default);
        }
    }
}