using AS_ASSN2_Rachel.Model;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace AS_ASSN2_Rachel.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var smtpClient = new SmtpClient(_configuration["EmailSettings:SMTPServer"])
            {
                Port = int.Parse(_configuration["EmailSettings:Port"]),
                Credentials = new NetworkCredential(_configuration["EmailSettings:Username"], _configuration["EmailSettings:Password"]),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_configuration["EmailSettings:Username"]),
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true,
            };
            mailMessage.To.Add(email);

            try
            {
                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (SmtpException ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
                throw new InvalidOperationException("Failed to send email.", ex);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
                throw;
            }
        }

        public Task SendConfirmationLinkAsync(string email, string subject, string link)
        {
            return SendEmailAsync(email, subject, $"Click here to confirm: <a href='{link}'>Confirm</a>");
        }

        public Task SendPasswordResetLinkAsync(string email, string subject, string link)
        {
            return SendEmailAsync(email, subject, $"Click here to reset your password: <a href='{link}'>Reset Password</a>");
        }

        public Task SendPasswordResetCodeAsync(string email, string subject, string code)
        {
            return SendEmailAsync(email, subject, $"Your password reset code is: {code}");
        }
    }
}
