using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ThreadsProject.Bussiness.ExternalServices.Interfaces;

namespace ThreadsProject.Bussiness.ExternalServices.Implementations
{
    public class UserEmailSender : IUserEmailSender
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<UserEmailSender> _logger;

        public UserEmailSender(IConfiguration configuration, ILogger<UserEmailSender> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            try
            {
                var smtpClient = new SmtpClient(_configuration["EmailSettings:SmtpServer"])
                {
                    Port = int.Parse(_configuration["EmailSettings:Port"]),
                    Credentials = new NetworkCredential(_configuration["EmailSettings:Username"], _configuration["EmailSettings:Password"]),
                    EnableSsl = bool.Parse(_configuration["EmailSettings:EnableSsl"]),
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_configuration["EmailSettings:FromEmail"]),
                    Subject = subject,
                    Body = message,
                    IsBodyHtml = true,
                };

                mailMessage.To.Add(email);

                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (SmtpException smtpEx)
            {
                _logger.LogError(smtpEx, "SMTP error occurred while sending email.");
                throw new Exception($"SMTP error: {smtpEx.Message}", smtpEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while sending email.");
                throw new Exception($"Error: {ex.Message}", ex);
            }
        }
    }
}
