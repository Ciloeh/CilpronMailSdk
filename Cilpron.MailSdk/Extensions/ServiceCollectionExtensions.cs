using Cilpron.MailSdk.Clients;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cilpron.MailSdk.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCilpronMailClient(
            this IServiceCollection services, string apiKey)
        {
            services.AddHttpClient<CilpronMailClient>(client =>
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
            });
            return services;
        }
    }
}
