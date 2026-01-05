using Cilpron.MailSdk.Exceptions;
using Cilpron.MailSdk.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;

namespace Cilpron.MailSdk.Internal
{
    /// <summary>
    /// Internal validator for mail requests.
    /// </summary>
    internal static class RequestValidator
    {
        public static void Validate(MailRequest request)
        {
            if (request == null)
                throw new ValidationException("MailRequest cannot be null.");

            if (string.IsNullOrEmpty(request.FromEmail))
                throw new ValidationException("From email is required.");

            ValidateEmail(request.FromEmail, "FromEmail");

            if (string.IsNullOrEmpty(request.Subject))
                throw new ValidationException("Subject is required.");

            if (!request.Personalizations.Any() || request.Personalizations.All(p => string.IsNullOrEmpty(p.ToEmail)))
                throw new ValidationException("At least one 'To' recipient is required.");

            // Validate emails in personalizations
            foreach (var p in request.Personalizations)
            {
                if (!string.IsNullOrEmpty(p.ToEmail))
                    ValidateEmail(p.ToEmail, "ToEmail in Personalization");

                if (!string.IsNullOrEmpty(p.CcEmail))
                    ValidateEmail(p.CcEmail, "CcEmail in Personalization");

                if (!string.IsNullOrEmpty(p.BccEmail))
                    ValidateEmail(p.BccEmail, "BccEmail in Personalization");
            }

            // Additional validations (e.g., check for content presence if required)
            if (string.IsNullOrEmpty(request.PlainTextContent) && string.IsNullOrEmpty(request.HtmlContent))
                throw new ValidationException("At least one of PlainTextContent or HtmlContent is required.");
        }

        private static void ValidateEmail(string email, string fieldName)
        {
            try
            {
                var addr = new MailAddress(email);
                if (addr.Address != email)
                    throw new ValidationException($"Invalid email format for {fieldName}: {email}");
            }
            catch
            {
                throw new ValidationException($"Invalid email format for {fieldName}: {email}");
            }
        }
    }
}