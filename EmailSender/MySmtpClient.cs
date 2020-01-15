using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace EmailSender
{
    public class MySmtpClient : ISmtpClient
    {
        public async Task Send(MailMessage msg, EmailSettings emailSettings)
        {
            using (var client = new SmtpClient(emailSettings.Host, emailSettings.Port))
            {
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(emailSettings.UserName, emailSettings.Password);
                await client.SendMailAsync(msg);
            }
        }
    }
}