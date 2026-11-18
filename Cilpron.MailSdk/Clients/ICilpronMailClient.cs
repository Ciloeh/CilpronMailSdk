using System.Threading;
using System.Threading.Tasks;
using Cilpron.MailSdk.Models;

namespace Cilpron.MailSdk.Clients
{
    /// <summary>
    /// Interface for the Cilpron Mail client.
    /// </summary>
    public interface ICilpronMailClient
    {
        /// <summary>
        /// Sends an email asynchronously (immediately, or at MailRequest.SendAt when set).
        /// </summary>
        Task<SendMailResponse> SendEmailAsync(MailRequest request, CancellationToken cancellationToken = default);
    }
}