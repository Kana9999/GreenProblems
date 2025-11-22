using System.Diagnostics;
using System.Threading.Tasks;

namespace AgriEnergy.Services
{
    public class DummyEmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            Debug.WriteLine($"Sending email to: {email}");
            Debug.WriteLine($"Subject: {subject}");
            Debug.WriteLine($"Message: {htmlMessage}");
            return Task.CompletedTask;
        }
    }
}
