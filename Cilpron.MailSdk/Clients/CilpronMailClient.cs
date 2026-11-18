using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Cilpron.MailSdk.Exceptions;
using Cilpron.MailSdk.Internal;
using Cilpron.MailSdk.Models;
using Polly;
using Polly.Retry;

namespace Cilpron.MailSdk.Clients
{
    /// <summary>
    /// Client for the Cilpron Mail API (Swift Grid), with resilient HTTP handling using Polly.
    /// </summary>
    public class CilpronMailClient : ICilpronMailClient
    {
        public const string DefaultBaseUrl = "https://swiftgrid.cilbox.com/api/swiftgrid/";
        private const string SendPath = "domains/send";

        // Retry only when the request provably did not complete (network failure) or the server
        // asked us to back off (429/503). Retrying other 5xx could deliver the same email twice.
        private static readonly AsyncRetryPolicy<HttpResponseMessage> RetryPolicy =
            Policy.Handle<HttpRequestException>()
                .OrResult<HttpResponseMessage>(r =>
                    r.StatusCode == (HttpStatusCode)429 ||
                    r.StatusCode == HttpStatusCode.ServiceUnavailable)
                .WaitAndRetryAsync(3, attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)));

        private readonly HttpClient _httpClient;

        /// <summary>
        /// Initializes the client with an API key (sent as a Bearer token) against the default endpoint.
        /// </summary>
        public CilpronMailClient(string apiKey)
        {
            if (string.IsNullOrEmpty(apiKey))
                throw new ArgumentNullException(nameof(apiKey));

            _httpClient = new HttpClient { BaseAddress = new Uri(DefaultBaseUrl) };
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        /// <summary>
        /// Initializes the client with a pre-configured HttpClient (DI or testing).
        /// </summary>
        public CilpronMailClient(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            if (_httpClient.BaseAddress == null)
                _httpClient.BaseAddress = new Uri(DefaultBaseUrl);
        }

        /// <inheritdoc />
        public async Task<SendMailResponse> SendEmailAsync(MailRequest request, CancellationToken cancellationToken = default)
        {
            RequestValidator.Validate(request);

            var payload = SwiftGridSendPayload.FromRequest(request);
            var json = JsonSerializer.Serialize(payload, SwiftGridSendPayload.JsonOptions);

            using var response = await RetryPolicy.ExecuteAsync(
                ct => _httpClient.SendAsync(
                    new HttpRequestMessage(HttpMethod.Post, SendPath)
                    {
                        Content = new StringContent(json, Encoding.UTF8, "application/json")
                    }, ct),
                cancellationToken).ConfigureAwait(false);

            var body = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
                throw new CilpronMailException($"Failed to send email: {(int)response.StatusCode} {response.StatusCode} - {body}");

            if (string.IsNullOrWhiteSpace(body))
                return new SendMailResponse();

            try
            {
                return JsonSerializer.Deserialize<SendMailResponse>(body, SwiftGridSendPayload.JsonOptions)
                       ?? new SendMailResponse();
            }
            catch (JsonException ex)
            {
                throw new CilpronMailException($"Email was accepted but the response could not be read: {ex.Message} - {body}");
            }
        }
    }
}