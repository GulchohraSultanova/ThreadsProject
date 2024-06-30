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
                    Body = $@"
                        <!DOCTYPE html>
                        <html>
                        <head>
                            <style>
                                body {{
                                    font-family: 'Helvetica Neue', Arial, sans-serif;
                                    margin: 0;
                                    padding: 0;
                                    background-color: #e0f7fa;
                                    display: flex;
                                    justify-content: center;
                                    align-items: center;
                                    height: 100vh;
                                }}
                                .container {{
                                    display: flex;
                                    justify-content: center;
                                    align-items: center;
                                    width: 100%;
                                    height: 100vh;
                                    background-color: #e0f7fa;
                                }}
                                .card {{
                                    background-color: #ffffff;
                                    border-radius: 15px;
                                    box-shadow: 0 10px 30px rgba(0, 0, 0, 0.1);
                                    max-width: 300px;
                                    padding: 40px;
                                    margin: 20px;
                                    text-align: center;
                                }}
                                .card h2 {{
                                    color: #34495e;
                                    font-size: 28px;
                                    margin-bottom: 20px;
                                }}
                                .card p {{
                                    color: #7f8c8d;
                                    font-size: 18px;
                                    line-height: 1.6;
                                    margin-bottom: 30px;
                                    font-style: italic;
                                }}
                                .button {{
                                    display: inline-block;
                                    padding: 15px 30px;
                                    font-size: 18px;
                                    color: white !important;
                                    background-color: #00A4EF;
                                    text-align: center;
                                    text-decoration: none;
                                    border-radius: 25px;
                                    transition: background-color 0.3s;
                                }}
                                .button:hover {{
                                    background-color: #c0392b;
                                }}
                                .verification {{
                                    margin: 20px 0;
                                }}
                                .verification img {{
                                    width: 200px;
                                    height: auto;
                                }}
                            </style>
                        </head>
                        <body>
                            <div class='container'>
                                <div class='card'>
                                    <h2>Threads.net</h2>
                                    <p>{message}</p>
                                    <div class='verification'>
                                        <img src='https://thewealthmosaic.s3.amazonaws.com/media/Logo_Verify.png' alt='Verified'>
                                    </div>
                                </div>
                            </div>
                        </body>
                        </html>",
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

        public async Task SendUrlEmailAsync(string email, string subject, string message)
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
                    Body = $@"
                        <!DOCTYPE html>
                        <html>
                        <head>
                            <style>
                                body {{
                                    font-family: 'Helvetica Neue', Arial, sans-serif;
                                    margin: 0;
                                    padding: 0;
                                    background-color: #e0f7fa;
                                    display: flex;
                                    justify-content: center;
                                    align-items: center;
                                    height: 100vh;
                                }}
                                .container {{
                                    display: flex;
                                    justify-content: center;
                                    align-items: center;
                                    width: 100%;
                                    height: 100vh;
                                    background-color: #e0f7fa;
                                }}
                                .card {{
                                    background-color: #ffffff;
                                    border-radius: 15px;
                                    box-shadow: 0 10px 30px rgba(0, 0, 0, 0.1);
                                    max-width: 300px;
                                    padding: 40px;
                                    margin: 20px;
                                    text-align: center;
                                }}
                                .card h2 {{
                                    color: #34495e;
                                    font-size: 28px;
                                    margin-bottom: 20px;
                                }}
                                .card p {{
                                    color: #7f8c8d;
                                    font-size: 18px;
                                    line-height: 1.6;
                                    margin-bottom: 30px;
                                    font-style: italic;
                                }}
                                .button {{
                                    display: inline-block;
                                    padding: 15px 30px;
                                    font-size: 18px;
                                    color: white !important;
                                    background-color: #00A4EF;
                                    text-align: center;
                                    text-decoration: none;
                                    border-radius: 25px;
                                    transition: background-color 0.3s;
                                }}
                                .button:hover {{
                                    background-color: #c0392b;
                                }}
                                .verification {{
                                    margin: 20px 0;
                                }}
                                .verification img {{
                                    width: 100px;
                                    height: auto;
                                }}
                            </style>
                        </head>
                        <body>
                            <div class='container'>
                                <div class='card'>
                                    <h2>Threads.net</h2>
                                    <p>{message}</p>
                                    <div class='verification'>
                                        <img src='https://static.vecteezy.com/system/resources/thumbnails/025/225/156/small_2x/3d-illustration-icon-of-phone-call-with-circular-or-round-podium-png.png' alt='Call'>
                                    </div>
                                </div>
                            </div>
                        </body>
                        </html>",
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
