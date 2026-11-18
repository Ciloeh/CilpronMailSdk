using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Cilpron.MailSdk.Clients;
using Cilpron.MailSdk.Exceptions;
using Cilpron.MailSdk.Helpers;
using Cilpron.MailSdk.Models;
using RichardSzalay.MockHttp;
using Xunit;

namespace Cilpron.MailSdk.Tests
{
    public class CilpronMailClientTests
    {
        private const string SendUrl = "https://swiftgrid.cilbox.com/api/swiftgrid/domains/send";

        private sealed class Capture
        {
            public string? Body { get; set; }
            public int Calls { get; set; }
            public HttpRequestMessage? Request { get; set; }
        }

        private static (CilpronMailClient Client, Capture Capture) CreateClient(
            HttpStatusCode status = HttpStatusCode.OK,
            string responseJson = "{\"messageId\":\"123\",\"status\":\"sent\"}")
        {
            var capture = new Capture();
            var mockHttp = new MockHttpMessageHandler();

            mockHttp.When(HttpMethod.Post, SendUrl)
                    .Respond(req =>
                    {
                        capture.Calls++;
                        capture.Request = req;
                        capture.Body = req.Content!.ReadAsStringAsync().GetAwaiter().GetResult();
                        return new HttpResponseMessage(status)
                        {
                            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
                        };
                    });

            return (new CilpronMailClient(new HttpClient(mockHttp)), capture);
        }

        private static MailRequest SimpleMessage() =>
            MailHelper.CreateSingleEmail(
                "from@example.com",
                "to@example.com",
                "Test Subject",
                plainTextContent: "Hello world",
                htmlContent: "<b>Hello world</b>");

        [Fact]
        public async Task SendEmailAsync_ReturnsSuccessResponse()
        {
            var (client, capture) = CreateClient();

            var response = await client.SendEmailAsync(SimpleMessage());

            Assert.Equal("sent", response.Status);
            Assert.Equal("123", response.MessageId);
            Assert.Equal(1, capture.Calls);
        }

        [Fact]
        public async Task SendEmailAsync_PostsSwiftGridPayload()
        {
            var (client, capture) = CreateClient();
            var msg = SimpleMessage();
            msg.FromName = "Sender Name";

            await client.SendEmailAsync(msg);

            using var doc = JsonDocument.Parse(capture.Body!);
            var root = doc.RootElement;

            Assert.Equal("from@example.com", root.GetProperty("from").GetString());
            Assert.Equal("Sender Name", root.GetProperty("fromName").GetString());
            Assert.Equal("Test Subject", root.GetProperty("subject").GetString());
            Assert.Equal("<b>Hello world</b>", root.GetProperty("htmlBody").GetString());
            Assert.Equal("Hello world", root.GetProperty("textBody").GetString());
            Assert.Equal(new[] { "to@example.com" },
                root.GetProperty("to").EnumerateArray().Select(e => e.GetString()).ToArray());
            Assert.False(root.GetProperty("sandboxMode").GetBoolean());
            Assert.False(root.TryGetProperty("sendAt", out _));
            Assert.False(root.TryGetProperty("cc", out _));
            Assert.False(root.TryGetProperty("bcc", out _));
        }

        [Fact]
        public async Task SendEmailAsync_SendsSandboxModeAndSendAt()
        {
            var (client, capture) = CreateClient();
            var msg = SimpleMessage();
            msg.SandboxMode = true;
            msg.SendAt = 1_800_000_000L;

            await client.SendEmailAsync(msg);

            using var doc = JsonDocument.Parse(capture.Body!);
            var root = doc.RootElement;

            Assert.True(root.GetProperty("sandboxMode").GetBoolean());
            Assert.Equal(DateTimeOffset.FromUnixTimeSeconds(1_800_000_000L),
                root.GetProperty("sendAt").GetDateTimeOffset());
        }

        [Fact]
        public async Task SendEmailAsync_MapsCcAndBcc()
        {
            var (client, capture) = CreateClient();
            var msg = SimpleMessage();
            var p = msg.Personalizations.First();
            p.CcEmail = "cc@example.com";
            p.BccEmail = "bcc@example.com";

            await client.SendEmailAsync(msg);

            using var doc = JsonDocument.Parse(capture.Body!);
            var root = doc.RootElement;

            Assert.Equal(new[] { "cc@example.com" },
                root.GetProperty("cc").EnumerateArray().Select(e => e.GetString()).ToArray());
            Assert.Equal(new[] { "bcc@example.com" },
                root.GetProperty("bcc").EnumerateArray().Select(e => e.GetString()).ToArray());
        }

        [Fact]
        public async Task SendEmailAsync_WithMultipleRecipients_SendsAllInTo()
        {
            var (client, capture) = CreateClient("{\"messageId\":\"789\",\"status\":\"sent\"}" is var _ ? HttpStatusCode.OK : HttpStatusCode.OK,
                "{\"messageId\":\"789\",\"status\":\"sent\"}");

            var recipients = new List<(string Email, string? Name)>
            {
                ("to1@example.com", "Recipient One"),
                ("to2@example.com", "Recipient Two")
            };

            var msg = MailHelper.CreateMultipleEmail(
                "from@example.com",
                recipients,
                "Test Multiple Recipients",
                plainTextContent: "Hello multiple",
                htmlContent: "<b>Hello multiple</b>");

            var response = await client.SendEmailAsync(msg);

            Assert.Equal("sent", response.Status);
            Assert.Equal("789", response.MessageId);

            using var doc = JsonDocument.Parse(capture.Body!);
            Assert.Equal(new[] { "to1@example.com", "to2@example.com" },
                doc.RootElement.GetProperty("to").EnumerateArray().Select(e => e.GetString()).ToArray());
        }

        [Fact]
        public async Task SendEmailAsync_WithAttachment_ThrowsValidationException()
        {
            var (client, capture) = CreateClient();

            var attachments = new List<Attachment>
            {
                new Attachment
                {
                    FileName = "test.txt",
                    ContentType = "text/plain",
                    Data = Encoding.UTF8.GetBytes("Test attachment content"),
                    FileSize = 23,
                    Disposition = "attachment",
                    CreatedAt = DateTime.UtcNow
                }
            };

            var msg = MailHelper.CreateSingleEmail(
                "from@example.com",
                "to@example.com",
                "Test Subject with Attachment",
                plainTextContent: "Hello with attachment",
                htmlContent: "<b>Hello with attachment</b>",
                attachments: attachments);

            await Assert.ThrowsAsync<ValidationException>(() => client.SendEmailAsync(msg));
            Assert.Equal(0, capture.Calls);
        }

        [Fact]
        public async Task SendEmailAsync_ThrowsCilpronMailException_OnHttpFailure()
        {
            var (client, capture) = CreateClient(HttpStatusCode.BadRequest, "{\"error\":\"Invalid request\"}");

            var msg = MailHelper.CreateSingleEmail(
                "from@example.com",
                "to@example.com",
                "Bad Request",
                plainTextContent: "This is a test body to pass validation");

            var ex = await Assert.ThrowsAsync<CilpronMailException>(() => client.SendEmailAsync(msg));

            Assert.Contains("400", ex.Message);
            Assert.Contains("Invalid request", ex.Message);
            Assert.Equal(1, capture.Calls);
        }

        [Fact]
        public async Task SendEmailAsync_DoesNotRetry_OnServerError()
        {
            var (client, capture) = CreateClient(HttpStatusCode.InternalServerError, "{\"error\":\"boom\"}");

            await Assert.ThrowsAsync<CilpronMailException>(() => client.SendEmailAsync(SimpleMessage()));

            // A 500 may already have queued the email, so it must not be retried (no duplicate sends).
            Assert.Equal(1, capture.Calls);
        }

        [Fact]
        public async Task SendEmailAsync_ReturnsEmptyResponse_WhenBodyIsEmpty()
        {
            var (client, _) = CreateClient(HttpStatusCode.OK, "");

            var response = await client.SendEmailAsync(SimpleMessage());

            Assert.NotNull(response);
        }

        [Fact]
        public async Task SendEmailAsync_ThrowsValidationException_WhenNoContentProvided()
        {
            var (client, capture) = CreateClient();

            var msg = MailHelper.CreateSingleEmail(
                "from@example.com",
                "to@example.com",
                "No Content Test");  // No plainTextContent or htmlContent

            await Assert.ThrowsAsync<ValidationException>(() => client.SendEmailAsync(msg));
            Assert.Equal(0, capture.Calls);
        }

        [Fact]
        public async Task SendEmailAsync_ThrowsValidationException_ForInvalidEmail()
        {
            var (client, capture) = CreateClient();

            var msg = MailHelper.CreateSingleEmail(
                "from@example.com",
                "not-an-email",
                "Subject",
                plainTextContent: "Hello");

            await Assert.ThrowsAsync<ValidationException>(() => client.SendEmailAsync(msg));
            Assert.Equal(0, capture.Calls);
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_WhenApiKeyIsEmpty()
        {
            Assert.Throws<ArgumentNullException>(() => new CilpronMailClient(""));
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_WhenHttpClientIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new CilpronMailClient((HttpClient)null!));
        }

        [Fact]
        public async Task Constructor_DefaultsBaseAddress_WhenHttpClientHasNone()
        {
            var (client, capture) = CreateClient();

            await client.SendEmailAsync(SimpleMessage());

            Assert.Equal(SendUrl, capture.Request!.RequestUri!.ToString());
        }

        [Fact]
        public void CreateSingleEmail_ThrowsArgumentNullException_WhenFromEmailIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                MailHelper.CreateSingleEmail(
                    null!,
                    "to@example.com",
                    "Test Subject",
                    plainTextContent: "Hello"));
        }

        [Fact]
        public void CreateSingleEmail_ThrowsArgumentNullException_WhenToEmailIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                MailHelper.CreateSingleEmail(
                    "from@example.com",
                    null!,
                    "Test Subject",
                    plainTextContent: "Hello"));
        }

        [Fact]
        public void CreateSingleEmail_ThrowsArgumentNullException_WhenSubjectIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                MailHelper.CreateSingleEmail(
                    "from@example.com",
                    "to@example.com",
                    null!,
                    plainTextContent: "Hello"));
        }

        [Fact]
        public void CreateMultipleEmail_ThrowsArgumentException_WhenNoRecipients()
        {
            Assert.Throws<ArgumentException>(() =>
                MailHelper.CreateMultipleEmail(
                    "from@example.com",
                    new List<(string, string?)>(),
                    "Test Subject",
                    plainTextContent: "Hello"));
        }
    }
}