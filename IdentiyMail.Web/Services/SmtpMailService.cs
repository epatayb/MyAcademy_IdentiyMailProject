using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace IdentiyMail.Web.Services
{
    public class SmtpMailService(IOptions<MailSettings> options) : IMailService
    {
        private readonly MailSettings _settings = options.Value;

        public async Task SendAsync(string receiverEmail, string subject, string htmlBody)
        {
            using var message = new MailMessage
            {
                From = new MailAddress(_settings.FromEmail, _settings.FromName),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true
            };

            message.To.Add(receiverEmail);

            using var smtpClient = new SmtpClient(_settings.Host, _settings.Port)
            {
                Credentials = new NetworkCredential(_settings.UserName, _settings.Password),
                EnableSsl = _settings.EnableSsl
            };

            await smtpClient.SendMailAsync(message);
        }

    }
}
