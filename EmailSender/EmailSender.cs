using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;

namespace EmailSender
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailSettings _settings;
        private readonly ISmtpClient _client;
        private readonly MailAddress _from;

        public EmailSender(IOptions<EmailSettings> emailSettings, ISmtpClient client)
        {
            EmailSettings emailSettingsValue = emailSettings.Value;
            EmailSettings.ValidSettings(emailSettingsValue);

            _settings = emailSettingsValue;
            _client = client;
            _from = new MailAddress(_settings.From);
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            MailMessage mailMessage = new MailMessage(_from, new MailAddress(email));
            mailMessage.Body = htmlMessage;
            mailMessage.Subject = subject;

            await _client.Send(mailMessage, _settings);
        }
    }
}
