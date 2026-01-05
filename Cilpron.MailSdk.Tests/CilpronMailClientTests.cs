using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
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
        [Fact]
        public async Task SendEmailAsync_ReturnsSuccessResponse()
        {
            // Arrange: mock HTTP response
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When("https://mail.cilpron.com/v1/mail/send")
                    .Respond("application/json", "{\"messageId\":\"123\",\"status\":\"sent\"}");

            var client = new CilpronMailClient(new HttpClient(mockHttp));

            var msg = MailHelper.CreateSingleEmail(
                "from@example.com",
                "to@example.com",
                "Test Subject",
                plainTextContent: "Hello world",
                htmlContent: "<b>Hello world</b>"
            );

            // Act
            var response = await client.SendEmailAsync(msg);

            // Assert
            Assert.Equal("sent", response.Status);
            Assert.Equal("123", response.MessageId);
        }

        [Fact]
        public async Task SendEmailAsync_WithAttachment_ReturnsSuccessResponse()
        {
            // Arrange: mock HTTP response
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When("https://mail.cilpron.com/v1/mail/send")
                    .Respond("application/json", "{\"messageId\":\"456\",\"status\":\"sent\"}");

            var client = new CilpronMailClient(new HttpClient(mockHttp));

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
                attachments: attachments
            );

            // Act
            var response = await client.SendEmailAsync(msg);

            // Assert
            Assert.Equal("sent", response.Status);
            Assert.Equal("456", response.MessageId);
        }

        [Fact]
        public async Task SendEmailAsync_WithMultipleRecipients_ReturnsSuccessResponse()
        {
            // Arrange: mock HTTP response
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When("https://mail.cilpron.com/v1/mail/send")
                    .Respond("application/json", "{\"messageId\":\"789\",\"status\":\"sent\"}");

            var client = new CilpronMailClient(new HttpClient(mockHttp));

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
                htmlContent: "<b>Hello multiple</b>"
            );

            // Act
            var response = await client.SendEmailAsync(msg);

            // Assert
            Assert.Equal("sent", response.Status);
            Assert.Equal("789", response.MessageId);
        }

        [Fact]
        public async Task SendEmailAsync_ThrowsCilpronMailException_OnHttpFailure()
        {
            // Arrange: mock HTTP error
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When("https://mail.cilpron.com/v1/mail/send")
                    .Respond(HttpStatusCode.BadRequest, "application/json", "{\"error\":\"Invalid request\"}");

            var client = new CilpronMailClient(new HttpClient(mockHttp));

            var msg = MailHelper.CreateSingleEmail(
                "from@example.com",
                "to@example.com",
                "Bad Request",
                plainTextContent: "This is a test body to pass validation"
            );

            // Act & Assert
            await Assert.ThrowsAsync<CilpronMailException>(
                () => client.SendEmailAsync(msg)
            );
        }

        [Fact]
        public async Task SendEmailAsync_ThrowsValidationException_WhenNoContentProvided()
        {
            // Arrange
            var mockHttp = new MockHttpMessageHandler();
            var client = new CilpronMailClient(new HttpClient(mockHttp));

            var msg = MailHelper.CreateSingleEmail(
                "from@example.com",
                "to@example.com",
                "No Content Test"
            );  // No plainTextContent or htmlContent

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(
                () => client.SendEmailAsync(msg)
            );
        }

        [Fact]
        public async Task SendEmailAsync_ThrowsArgumentNullException_WhenFromEmailIsNull()
        {
            // Arrange
            var mockHttp = new MockHttpMessageHandler();
            var client = new CilpronMailClient(new HttpClient(mockHttp));

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                MailHelper.CreateSingleEmail(
                    null!,
                    "to@example.com",
                    "Test Subject",
                    plainTextContent: "Hello"
                )
            );
        }

        [Fact]
        public async Task SendEmailAsync_ThrowsArgumentNullException_WhenToEmailIsNull()
        {
            // Arrange
            var mockHttp = new MockHttpMessageHandler();
            var client = new CilpronMailClient(new HttpClient(mockHttp));

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                MailHelper.CreateSingleEmail(
                    "from@example.com",
                    null!,
                    "Test Subject",
                    plainTextContent: "Hello"
                )
            );
        }

        [Fact]
        public async Task SendEmailAsync_ThrowsArgumentNullException_WhenSubjectIsNull()
        {
            // Arrange
            var mockHttp = new MockHttpMessageHandler();
            var client = new CilpronMailClient(new HttpClient(mockHttp));

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                MailHelper.CreateSingleEmail(
                    "from@example.com",
                    "to@example.com",
                    null!,
                    plainTextContent: "Hello"
                )
            );
        }

        [Fact]
        public async Task SendEmailAsync_ForMultipleEmails_ThrowsArgumentException_WhenNoRecipients()
        {
            // Arrange
            var mockHttp = new MockHttpMessageHandler();
            var client = new CilpronMailClient(new HttpClient(mockHttp));

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                MailHelper.CreateMultipleEmail(
                    "from@example.com",
                    new List<(string, string?)>(),
                    "Test Subject",
                    plainTextContent: "Hello"
                )
            );
        }
    }
}