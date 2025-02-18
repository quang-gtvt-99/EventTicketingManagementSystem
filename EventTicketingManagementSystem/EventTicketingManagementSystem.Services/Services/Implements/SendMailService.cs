using EventTicketingManagementSystem.Services.Services.Interfaces;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace EventTicketingManagementSystem.Services.Services.Implements
{
    public class SendMailService : ISendMailService
    {
        private readonly string _fromName;
        private readonly string _fromEmail;
        private readonly string _smtpServer;
        private readonly int _port;
        private readonly string _username;
        private readonly string _password;

        public SendMailService(IConfiguration configuration)
        {
            _fromName = GetConfigurationValue(configuration, "EmailSettings_FromName");
            _fromEmail = GetConfigurationValue(configuration, "EmailSettings_FromEmail");
            _smtpServer = GetConfigurationValue(configuration, "EmailSettings_SmtpServer");
            _port = int.Parse(GetConfigurationValue(configuration, "EmailSettings_Port"));
            _username = GetConfigurationValue(configuration, "EmailSettings_Username");
            _password = GetConfigurationValue(configuration, "EmailSettings_Password");
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body, bool isHtml = true)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(_fromName, _fromEmail));
            email.To.Add(new MailboxAddress(toEmail, toEmail));
            email.Subject = subject;

            var bodyBuilder = new BodyBuilder();
            if (isHtml)
            {
                bodyBuilder.HtmlBody = body;
            }
            else
            {
                bodyBuilder.TextBody = body;
            }
            email.Body = bodyBuilder.ToMessageBody();

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_smtpServer, _port, true);
            await smtp.AuthenticateAsync(_username, _password);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }

        private string GetConfigurationValue(IConfiguration configuration, string key)
        {
            var value = configuration[key];
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException($"Configuration value for '{key}' is missing or empty.");
            }
            return value;
        }
    }
}
