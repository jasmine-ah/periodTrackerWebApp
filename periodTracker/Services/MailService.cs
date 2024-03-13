using System.Threading.Tasks;
using System.Threading;
using MailKit;
using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Options;


namespace periodTracker.Services
{
    public class MailService : IMailService
    {
        private readonly MailSettings _mailSettings;
        private readonly ILogger<MailService> _logger;

        public MailService(IOptions<MailSettings> mailSettingsOptions, ILogger<MailService> logger)
        {
            _mailSettings = mailSettingsOptions.Value;
            _logger = logger;
        }

        public async Task<bool> SendAsync(MailData mailData)
        {
           try
             {
        using (var emailMessage = new MimeMessage())
        {
            // Set up the email message
            emailMessage.From.Add(new MailboxAddress(_mailSettings.SenderName, _mailSettings.SenderEmail));
            emailMessage.To.Add(new MailboxAddress("", mailData.Email));
            emailMessage.Subject = mailData.Subject;
            emailMessage.Body = new TextPart("plain") { Text = mailData.Body };

            using (var mailClient = new SmtpClient())
            {
                // Connect to the SMTP server
                await mailClient.ConnectAsync(_mailSettings.Server, _mailSettings.Port, true);

                // Authenticate with the SMTP server
                if (!string.IsNullOrEmpty(_mailSettings.UserName) && !string.IsNullOrEmpty(_mailSettings.Password))
                {
                    await mailClient.AuthenticateAsync(_mailSettings.UserName, _mailSettings.Password);
                }

                // Send the email
                await mailClient.SendAsync(emailMessage);

                // Disconnect from the SMTP server
                await mailClient.DisconnectAsync(true);
            }
        }

        return true;
    }
    catch (Exception ex)
    {
        //Log the exception
                _logger.LogError(ex, "Failed to send email");
                return false;
            }
}

    }
}


