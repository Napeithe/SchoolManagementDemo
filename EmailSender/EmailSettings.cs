using System;

namespace EmailSender
{
    public class EmailSettings
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string From { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public static void ValidSettings(EmailSettings emailSettings)
        {
            if (emailSettings is null)
            {
                throw new EmailSenderException("Missing email settings!!! Check appsettings.");
            }

            if (string.IsNullOrEmpty(emailSettings.Host))
            {
                throw new EmailSenderException(MissingSettingsMessage(nameof(Host)));
            }

            if (string.IsNullOrEmpty(emailSettings.From))
            {
                throw new EmailSenderException(MissingSettingsMessage(nameof(From)));
            }

            if (string.IsNullOrEmpty(emailSettings.UserName))
            {
                throw new EmailSenderException(MissingSettingsMessage(nameof(UserName)));
            }

            if (string.IsNullOrEmpty(emailSettings.Password))
            {
                throw new EmailSenderException(MissingSettingsMessage(nameof(Password)));
            }

            if (emailSettings.Port == 0)
            {
                throw new EmailSenderException(MissingSettingsMessage(nameof(Port)));
            }
        }

        private static string MissingSettingsMessage(string variableName)
        {
            return $"Missing email settings!!! {variableName} is missing.";
        }
    }

    public class EmailSenderException : Exception
    {
        public EmailSenderException(string message) : base(message)
        {
        }
    }
}