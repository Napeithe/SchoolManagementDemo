namespace SchoolManagement.Infrastructure.Extensions
{
    public static class EmailExtenstion
    {
        public static string HashEmail(this string email)
        {
            int firstPartEmailIndex = email.IndexOf('@');

            string firstPartEmail = email.Substring(0, firstPartEmailIndex);
            string lastPartEmail = email.Substring(firstPartEmailIndex);

            string hashedEmail = "";
            if (firstPartEmail.Length >= 4)
            {
                hashedEmail = firstPartEmail.Substring(0, 2);
                for (int i = 2; i < firstPartEmail.Length  -2; i++)
                {
                    hashedEmail += '*';
                }

                hashedEmail += firstPartEmail[firstPartEmail.Length - 1];
            }
            else
            {

                hashedEmail = firstPartEmail.Substring(0, 1);
                for (int i = 1; i < firstPartEmail.Length - 2; i++)
                {
                    hashedEmail += '*';
                }

                hashedEmail += firstPartEmail[lastPartEmail.Length - 1];
            }

            hashedEmail += lastPartEmail;
            return hashedEmail;
        }
    }
}