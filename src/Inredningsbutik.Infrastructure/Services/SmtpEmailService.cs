using System.Net;
using System.Net.Mail;
using Inredningsbutik.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Inredningsbutik.Infrastructure.Services
{
    public class SmtpEmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<SmtpEmailService> _logger;

        public SmtpEmailService(IConfiguration configuration,
        ILogger<SmtpEmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendOrderConfirmationAsync(string toEmail, string customerName, int orderId)
        {
            try
            {
var smtpHost = _configuration["EmailSettings:SmtpHost"]
               ?? throw new InvalidOperationException("SMTP Host saknas");

var smtpUser = _configuration["EmailSettings:Username"]
               ?? throw new InvalidOperationException("SMTP Användarnamn saknas");

var smtpPass = _configuration["EmailSettings:Password"]
               ?? throw new InvalidOperationException("SMTP Lösenord saknas");

var smtpPort = _configuration.GetValue<int>("EmailSettings:SmtpPort");

                var client = new SmtpClient(smtpHost, smtpPort)
                {
                    Credentials = new NetworkCredential(smtpUser, smtpPass),
                    EnableSsl = true
                };

                var message = new MailMessage
                {
                    From = new MailAddress(smtpUser),
                    Subject = $"Orderbekräftelse #{orderId}",
                    Body = $"Tack {customerName}, din order #{orderId} är mottagen och kommer behandlas inom kort.",
                    IsBodyHtml = false
                };

                message.To.Add(toEmail);

                await client.SendMailAsync(message);

                _logger.LogInformation("Mail har skickats för order {OrderId}", orderId);
            } catch (Exception ex)
            {
                _logger.LogError(ex, "Fel uppstod vid försök att skicka mail. Försök igen.");
            }
        }
    }
}