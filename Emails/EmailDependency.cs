using PackageTrackerAPI.Entities;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace PackageTrackerAPI.Emails
{
    public class EmailDependency : IEmailDependency
    {
        private readonly ISendGridClient _sendGridClient;
        public EmailDependency(ISendGridClient sendGridclient)
        {
            _sendGridClient = sendGridclient;
        }

        public SendGridMessage AddSenderToMessage(SendGridMessage message, Package package)
        {
            message.AddTo(package.SenderEmail, package.SenderName);
            return message;
        }

        public SendGridMessage CreateMessage(string subject, string plainTextContent)
        {
            var message = new SendGridMessage
            {
                From = new EmailAddress("hgomes.andrade@gmail.com", "Henrique Andrade"),
                Subject = subject,
                PlainTextContent = plainTextContent
            };

            return message;
        }

        public Task<Response> SendMessage(SendGridMessage message)
        {
            return _sendGridClient.SendEmailAsync(message);
        }
    }
}
