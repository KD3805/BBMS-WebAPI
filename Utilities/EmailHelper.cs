using MailKit.Net.Smtp;
using MimeKit;

namespace BBMS_WebAPI.Utilities
{
    public class EmailHelper
    {
        //private readonly string _emailUser = "dabhikishan358@gmail.com";
        //private readonly string _emailPassword = "wqzf xwul fjdt gfqu";
        private readonly string _emailUser = "teamkd101@gmail.com";
        private readonly string _emailPassword = "wjyz aywo kkaq yuqf";

        public async Task SendEmail(string toEmail, string subject, string body)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Red Vault", _emailUser));
            message.To.Add(new MailboxAddress("", toEmail));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder { HtmlBody = body };
            message.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("smtp.gmail.com", 587, false);
                await client.AuthenticateAsync(_emailUser, _emailPassword);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }
    }
}
