using Container_App.Core.Interface.Emails;
using Container_App.Core.Model.Email;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;

namespace Container_App.Service.Services.Emails
{
    public class EmailService : IEmailService
    {
        private readonly MailSettings _settings;

        public EmailService(IOptions<MailSettings> settings)
        {
            _settings = settings.Value;
        }
        public async Task SendEmailAsync(MailRequest request)
        {
            var email = new MimeMessage();

            email.From.Add(
                new MailboxAddress(
                    _settings.DisplayName,
                    _settings.Mail));

            email.To.Add(MailboxAddress.Parse(request.ToEmail));

            email.Subject = request.Subject;

            var builder = new BodyBuilder();

            if (request.IsHtml)
                builder.HtmlBody = request.Body;
            else
                builder.TextBody = request.Body;

            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();

            await smtp.ConnectAsync(
                _settings.Host,
                _settings.Port,
                SecureSocketOptions.StartTls);

            await smtp.AuthenticateAsync(
                _settings.Mail,
                _settings.Password);

            await smtp.SendAsync(email);

            await smtp.DisconnectAsync(true);
        }
    }
}
