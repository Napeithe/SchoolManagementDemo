using System.Collections.Generic;
using Moq;
using System.Net.Mail;
using System.Threading.Tasks;
using EmailSender;
using Microsoft.Extensions.Options;
using Xunit;

namespace EmailSenderTest
{
    public class EmailSenderTest
    {
        [Fact]
        public async Task ShouldSendEmail()
        {
            //Arrange

            Moq.Mock<ISmtpClient> smtpClientMock = new Mock<ISmtpClient>();
            Mock<IOptions<EmailSettings>> optionMock = new Mock<IOptions<EmailSettings>>();
            EmailSettings emailSettings = new EmailSettings()
            {
                UserName = "test",
                Password = "test",
                From = "test@gg.pl",
                Host = "aaa",
                Port = 234
            };
            optionMock.SetupGet(x => x.Value).Returns(emailSettings);
       
            smtpClientMock.Setup(x => x.Send(It.IsAny<MailMessage>(), emailSettings));
            EmailSender.EmailSender emailSender = new EmailSender.EmailSender(optionMock.Object, smtpClientMock.Object);
            //Act
            await emailSender.SendEmailAsync("test@email.com", "subject", "message");
            //Assert
            smtpClientMock.Verify(x => x.Send(It.IsAny<MailMessage>(), emailSettings), Times.Once);
        }

        [Theory]
        [MemberData(nameof(Data))]
        public void ShouldThrowExceptionPortIsMissing(string message, EmailSettings emailSettings)
        {
            //Arrange
            Mock<IOptions<EmailSettings>> optionMock = new Mock<IOptions<EmailSettings>>();
            Moq.Mock<ISmtpClient> smtpClientMock = new Mock<ISmtpClient>();
            optionMock.SetupGet(x => x.Value).Returns(emailSettings);
            smtpClientMock.Setup(x => x.Send(It.IsAny<MailMessage>(), emailSettings));
            //Act
            EmailSenderException emailSenderException = Assert.Throws<EmailSenderException>(()=>new EmailSender.EmailSender(optionMock.Object, smtpClientMock.Object));
            //Assert
            Assert.Equal(message, emailSenderException.Message);
            smtpClientMock.Verify(x => x.Send(It.IsAny<MailMessage>(), emailSettings), Times.Never);
        }

        public static IEnumerable<object[]> Data =>
            new List<object[]>
            {
                new object[] { $"Missing email settings!!! {nameof(EmailSettings.Port)} is missing.",  new EmailSettings()
                    {
                    UserName = "test",
                    Password = "test",
                    From = "test@gg.pl",
                    Host = "aaa",
                    Port = 0
                    }},
                new object[] { $"Missing email settings!!! {nameof(EmailSettings.UserName)} is missing.",  new EmailSettings()
                    {
                    UserName = "",
                    Password = "test",
                    From = "test@gg.pl",
                    Host = "aaa",
                    Port = 232
                    }},
                new object[] { $"Missing email settings!!! {nameof(EmailSettings.Password)} is missing.",  new EmailSettings()
                    {
                    UserName = "Aa",
                    Password = "",
                    From = "test@gg.pl",
                    Host = "aaa",
                    Port = 232
                    }},
                new object[] { $"Missing email settings!!! {nameof(EmailSettings.From)} is missing.",  new EmailSettings()
                    {
                    UserName = "dsada",
                    Password = "test",
                    From = "",
                    Host = "aaa",
                    Port = 232
                    }},
                new object[] { $"Missing email settings!!! {nameof(EmailSettings.Host)} is missing.",  new EmailSettings()
                    {
                    UserName = "dsada",
                    Password = "test",
                    From = "dsad",
                    Host = "",
                    Port = 232
                    }},
                new object[] { $"Missing email settings!!! Check appsettings.",  null}
            };
    }
}
