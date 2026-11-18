using System;
using System.Linq;
using System.Net.Mail;
using Cilpron.MailSdk.Exceptions;
using Cilpron.MailSdk.Models;

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

            if (request.Personalizations == null ||
                !request.Personalizations.Any(p => !string.IsNullOrEmpty(p.ToEmail)))
                throw new ValidationException("At least one 'To' recipient is required.");

            foreach (var p in request.Personalizations)
            {
                if (!string.IsNullOrEmpty(p.ToEmail))
                    ValidateEmail(p.ToEmail, "ToEmail in Personalization");

                if (!string.IsNullOrEmpty(p.CcEmail))
                    ValidateEmail(p.CcEmail, "CcEmail in Personalization");

                if (!string.IsNullOrEmpty(p.BccEmail))
                    ValidateEmail(p.BccEmail, "BccEmail in Personalization");
            }

            if (string.IsNullOrEmpty(request.PlainTextContent) && string.IsNullOrEmpty(request.HtmlContent))
                throw new ValidationException("At least one of PlainTextContent or HtmlContent is required.");

            if (request.Attachments != null && request.Attachments.Any())
                throw new ValidationException("Attachments are not supported by the mail endpoint.");
        }

        private static void ValidateEmail(string email, string fieldName)
        {
            bool valid;
            try
            {
                valid = new MailAddress(email).Address == email;
            }
            catch (FormatException)
            {
                valid = false;
            }

            if (!valid)
                throw new ValidationException($"Invalid email format for {fieldName}: {email}");
        }
    }
}