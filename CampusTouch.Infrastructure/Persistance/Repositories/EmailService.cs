using CampusTouch.Application.Interfaces;
using MimeKit;


//using System.Net.Mail;
using MailKit.Security;
using MailKit.Net.Smtp;



namespace CampusTouch.Infrastructure.Persistance.Repositories
{
    public class EmailService : IEmailService
    {
        
        public async Task SendAsync(string to, string subject, string body)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("CampusTouch", "jefin.s786@gmail.com"));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;

            email.Body = new TextPart("html")
            {
                Text = body
            };

            using var smtp = new SmtpClient();

            await smtp.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);

            await smtp.AuthenticateAsync("jefin.s786@gmail.com", "osmkvuazhjxfdtuc");

            await smtp.SendAsync(email);

            await smtp.DisconnectAsync(true);

        }
    }
}