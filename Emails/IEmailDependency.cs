using PackageTrackerAPI.Entities;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace PackageTrackerAPI.Emails
{
    public interface IEmailDependency
    {
        public SendGridMessage AddSenderToMessage(SendGridMessage message, Package package);
        public Task<Response> SendMessage(SendGridMessage message);

        public SendGridMessage CreateMessage(string subject, string plainTextContent);
    }
}
