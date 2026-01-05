using Cilpron.MailSdk.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cilpron.MailSdk.Clients
{
    /// <summary>
    /// Interface for the Cilpron Mail client.
    /// </summary>
    public interface ICilpronMailClient
    {
        /// <summary>
        /// Sends an email asynchronously.
        /// </summary>
        /// <param name="request">The mail request model.</param>
        /// <returns>The send response.</returns>
        Task<SendMailResponse> SendEmailAsync(MailRequest request);
    }
}
