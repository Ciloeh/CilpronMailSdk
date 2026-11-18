using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Cilpron.MailSdk.Models;

namespace Cilpron.MailSdk.Internal
{
    /// <summary>
    /// Wire format of POST /api/swiftgrid/domains/send.
    /// </summary>
    internal sealed class SwiftGridSendPayload
    {
        internal static readonly System.Text.Json.JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        [JsonPropertyName("from")] public string From { get; set; } = string.Empty;
        [JsonPropertyName("to")] public List<string> To { get; set; } = new();
        [JsonPropertyName("subject")] public string Subject { get; set; } = string.Empty;
        [JsonPropertyName("htmlBody")] public string? HtmlBody { get; set; }
        [JsonPropertyName("textBody")] public string? TextBody { get; set; }
        [JsonPropertyName("fromName")] public string? FromName { get; set; }
        [JsonPropertyName("cc")] public List<string>? Cc { get; set; }
        [JsonPropertyName("bcc")] public List<string>? Bcc { get; set; }
        [JsonPropertyName("sandboxMode")] public bool SandboxMode { get; set; }
        [JsonPropertyName("sendAt")] public DateTimeOffset? SendAt { get; set; }

        internal static SwiftGridSendPayload FromRequest(MailRequest request)
        {
            static List<string> Distinct(IEnumerable<string?> values) =>
                values.Where(v => !string.IsNullOrWhiteSpace(v))
                      .Select(v => v!.Trim())
                      .Distinct(StringComparer.OrdinalIgnoreCase)
                      .ToList();

            var cc = Distinct(request.Personalizations.Select(p => p.CcEmail));
            var bcc = Distinct(request.Personalizations.Select(p => p.BccEmail));

            return new SwiftGridSendPayload
            {
                From = request.FromEmail,
                FromName = request.FromName,
                Subject = request.Subject,
                HtmlBody = request.HtmlContent,
                TextBody = request.PlainTextContent,
                To = Distinct(request.Personalizations.Select(p => p.ToEmail)),
                Cc = cc.Count > 0 ? cc : null,
                Bcc = bcc.Count > 0 ? bcc : null,
                SandboxMode = request.SandboxMode,
                SendAt = request.SendAt?.ToUniversalTime()
            };
        }
    }
}