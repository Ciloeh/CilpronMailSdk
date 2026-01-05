using Cilpron.MailSdk.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using Attachment = Cilpron.MailSdk.Models.Attachment;

namespace Cilpron.MailSdk.Helpers
{
    /// <summary>
    /// Helper methods for creating mail requests, inspired by Twilio SendGrid's MailHelper for simplicity and fluency.
    /// </summary>
    public static class MailHelper
    {
        /// <summary>
        /// Creates a simple single email request with optional names and attachments.
        /// </summary>
        /// <param name="fromEmail">The sender's email address.</param>
        /// <param name="toEmail">The recipient's email address.</param>
        /// <param name="subject">The email subject.</param>
        /// <param name="fromName">The sender's display name (optional).</param>
        /// <param name="toName">The recipient's display name (optional).</param>
        /// <param name="plainTextContent">The plain text content (optional).</param>
        /// <param name="htmlContent">The HTML content (optional).</param>
        /// <param name="attachments">Optional list of attachments.</param>
        /// <returns>A configured MailRequest for a single recipient.</returns>
        public static MailRequest CreateSingleEmail(
            string fromEmail,
            string toEmail,
            string subject,
            string? fromName = null,
            string? toName = null,
            string? plainTextContent = null,
            string? htmlContent = null,
            IEnumerable<Attachment>? attachments = null)
        {
            if (string.IsNullOrEmpty(toEmail))
                throw new ArgumentNullException(nameof(toEmail));

            var request = new MailRequest
            {
                FromEmail = fromEmail ?? throw new ArgumentNullException(nameof(fromEmail)),
                FromName = fromName,
                Subject = subject ?? throw new ArgumentNullException(nameof(subject)),
                PlainTextContent = plainTextContent,
                HtmlContent = htmlContent,
                CreatedAt = DateTime.UtcNow,  // Align with schema defaults
                Status = "pending"  // Assume default status for new requests
            };

            // Use Personalizations for structured recipient handling
            var personalization = new Personalization
            {
                ToEmail = toEmail,
                ToName = toName
            };
            request.Personalizations = new List<Personalization> { personalization };

            // Fallback: Set Recipients as a string for single recipient
            request.Recipients = toEmail;

            // Handle attachments if provided
            if (attachments != null)
            {
                request.Attachments = new List<Attachment>(attachments);
            }

            // Optional: Set defaults for tracking or other settings
            request.TrackingSettings = new TrackingSetting { ClickTrackingEnabled = true };

            return request;
        }

        // Overload for multiples: Creates an email request for multiple recipients using personalizations
        // Each recipient gets their own personalization to ensure privacy (recipients don't see each other's addresses)
        public static MailRequest CreateMultipleEmail(
            string fromEmail,
            IEnumerable<(string Email, string? Name)> toRecipients,
            string subject,
            string? fromName = null,
            string? plainTextContent = null,
            string? htmlContent = null,
            IEnumerable<Attachment>? attachments = null)
        {
            if (toRecipients == null || !toRecipients.Any())
                throw new ArgumentException("At least one recipient is required.", nameof(toRecipients));

            var request = new MailRequest
            {
                FromEmail = fromEmail ?? throw new ArgumentNullException(nameof(fromEmail)),
                FromName = fromName,
                Subject = subject ?? throw new ArgumentNullException(nameof(subject)),
                PlainTextContent = plainTextContent,
                HtmlContent = htmlContent,
                CreatedAt = DateTime.UtcNow,  // Align with schema defaults
                Status = "pending"  // Assume default status for new requests
            };

            // Add personalizations for each recipient
            request.Personalizations = toRecipients.Select(r => new Personalization
            {
                ToEmail = r.Email ?? throw new ArgumentNullException(nameof(r.Email)),
                ToName = r.Name
            }).ToList();

            // If the API requires "recipients" as a concatenated string (fallback), set it
            request.Recipients = string.Join(",", toRecipients.Select(r => r.Email));

            // Handle attachments if provided
            if (attachments != null)
            {
                request.Attachments = new List<Attachment>(attachments);
            }

            // Optional: Set defaults for tracking or other settings
            request.TrackingSettings = new TrackingSetting { ClickTrackingEnabled = true };

            return request;
        }
    }
}