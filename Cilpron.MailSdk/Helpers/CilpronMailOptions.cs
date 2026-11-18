using System;
using Cilpron.MailSdk.Clients;

namespace Cilpron.MailSdk.Helpers
{
    /// <summary>
    /// Options for configuring the CilpronMailClient.
    /// </summary>
    public class CilpronMailOptions
    {
        /// <summary>
        /// The API key, sent as a Bearer token. Leave empty if the endpoint does not require one.
        /// </summary>
        public string? ApiKey { get; set; }

        /// <summary>
        /// Base URL of the API. Defaults to the Swift Grid endpoint.
        /// </summary>
        public string BaseUrl { get; set; } = CilpronMailClient.DefaultBaseUrl;

        /// <summary>
        /// HTTP timeout per attempt.
        /// </summary>
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
    }
}