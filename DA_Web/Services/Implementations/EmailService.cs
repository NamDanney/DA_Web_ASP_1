using DA_Web.Configurations;
using DA_Web.Services.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Threading.Tasks;

namespace DA_Web.Services.Implementations
{
    public class EmailService : IEmailService
    {
        // Đã sửa lỗi: dùng EmailConfig từ namespace đúng
        private readonly EmailConfig _emailConfig;

        public EmailService(IOptions<EmailConfig> emailConfig)
        {
            _emailConfig = emailConfig.Value;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            var emailMessage = new MimeMessage();

            // Đã sửa lỗi: sử dụng SenderName và SenderEmail
            emailMessage.From.Add(new MailboxAddress(_emailConfig.SenderName, _emailConfig.SenderEmail));
            emailMessage.To.Add(new MailboxAddress("", toEmail));
            emailMessage.Subject = subject;

            var bodyBuilder = new BodyBuilder { HtmlBody = message };
            emailMessage.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                // Đã sửa lỗi: sử dụng SmtpServer và SmtpPort
                await client.ConnectAsync(_emailConfig.SmtpServer, _emailConfig.SmtpPort, SecureSocketOptions.StartTls);
                // Đã sửa lỗi: sử dụng SenderEmail và SenderPassword
                await client.AuthenticateAsync(_emailConfig.SenderEmail, _emailConfig.SenderPassword);
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }
        }
    }
}