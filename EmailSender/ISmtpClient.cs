using System.Net.Mail;
using System.Threading.Tasks;

namespace EmailSender
{
    public interface ISmtpClient
    {
        Task Send(MailMessage msg, EmailSettings emailSettings);
    }
}