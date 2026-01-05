using Cilpron.MailSdk.Clients;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cilpron.MailSdk.Helpers
{
    /// <summary>
    /// Extensions for adding CilpronMailClient to dependency injection.
    /// </summary>
    public static class CilpronMailClientExtensions
    {
        /// <summary>
        /// Adds the CilpronMailClient to the service collection.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configure">Action to configure options.</param>
        /// <returns>The updated service collection.</returns>
        public static IServiceCollection AddCilpronMailClient(this IServiceCollection services, Action<CilpronMailOptions> configure)
        {
            var options = new CilpronMailOptions();
            configure?.Invoke(options);

            var httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://mail.cilpron.com/v1/")
            };
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", options.ApiKey);

            services.AddSingleton<ICilpronMailClient>(new CilpronMailClient(httpClient));

            return services;
        }
    }

    
}
