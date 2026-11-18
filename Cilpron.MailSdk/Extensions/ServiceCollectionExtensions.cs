using System;
using Cilpron.MailSdk.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace Cilpron.MailSdk.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCilpronMailClient(this IServiceCollection services, string apiKey)
        {
            if (string.IsNullOrEmpty(apiKey))
                throw new ArgumentNullException(nameof(apiKey));

            return CilpronMailClientExtensions.AddCilpronMailClient(services, o => o.ApiKey = apiKey);
        }
    }
}