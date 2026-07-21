using MailKit.Net.Smtp;
using MimeKit;

namespace BracketSmasherBackend.Services;

public class EmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }


    public async Task SendEmailAsync(
        string to,
        string subject,
        string body)
    {
        var email = new MimeMessage();

        email.From.Add(
            new MailboxAddress(
                "BracketSmasher",
                _config["Email:From"]
            )
        );

        email.To.Add(
            MailboxAddress.Parse(to)
        );


        email.Subject = subject;


        email.Body =
            new TextPart("plain")
            {
                Text = body
            };


        using var smtp = new SmtpClient();


        await smtp.ConnectAsync(
            _config["Email:SmtpServer"],
            int.Parse(
                _config["Email:Port"]!
            ),
            MailKit.Security.SecureSocketOptions.StartTls
        );


        await smtp.AuthenticateAsync(
            _config["Email:Username"],
            _config["Email:Password"]
        );


        await smtp.SendAsync(email);


        await smtp.DisconnectAsync(true);
    }
}