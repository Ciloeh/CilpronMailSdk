using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Cilpron.MailSdk.Clients;
using Microsoft.Extensions.DependencyInjection;

namespace Cilpron.MailSdk.Helpers
{
    /// <summary>
    /// Extensions for adding CilpronMailClient to dependency injection.
    /// </summary>
    public static class CilpronMailClientExtensions
    {
        public static IServiceCollection AddCilpronMailClient(this IServiceCollection services, Action<CilpronMailOptions> configure)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            var options = new CilpronMailOptions();
            configure?.Invoke(options);

            var baseUrl = string.IsNullOrWhiteSpace(options.BaseUrl) ? CilpronMailClient.DefaultBaseUrl : options.BaseUrl;
            if (!baseUrl.EndsWith("/")) baseUrl += "/";

            services.AddHttpClient<ICilpronMailClient, CilpronMailClient>(client =>
            {
                client.BaseAddress = new Uri(baseUrl);
                client.Timeout = options.Timeout;
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                if (!string.IsNullOrEmpty(options.ApiKey))
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", options.ApiKey);
            });

            return services;
        }
    }
}