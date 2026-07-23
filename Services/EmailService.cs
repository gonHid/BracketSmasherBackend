using Resend;

namespace BracketSmasherBackend.Services;

public class EmailService
{
    private readonly IResend _resend;

    public EmailService(IResend resend)
    {
        _resend = resend;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        var message = new EmailMessage
        {
            From = "BracketSmasher <onboarding@resend.dev>",
            To = new[] { to },
            Subject = subject,
            HtmlBody = $"<pre>{body}</pre>"
        };

        await _resend.EmailSendAsync(message);
    }
}