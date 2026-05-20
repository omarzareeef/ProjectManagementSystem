using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using PMS.Application.Interfaces.EmailService;
using PMS.Infrastructure.Settings;

namespace PMS.Infrastructure.Services.EmailService;

public class EmailService : IEmailService
{
    private readonly EmailSettings _emailSettings;

    public EmailService(IOptions<EmailSettings> emailSettings)
    {
        _emailSettings = emailSettings.Value;
    }


    public async Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
    {
        var email = new MimeMessage();

        email.From.Add(new MailboxAddress(_emailSettings.DisplayName, _emailSettings.From));
        email.To.Add(MailboxAddress.Parse(to));
        email.Subject = subject;

        email.Body = new TextPart(MimeKit.Text.TextFormat.Html)
        {
            Text = body
        };

        using var smtp = new SmtpClient();

        await smtp.ConnectAsync(
            _emailSettings.Host,
            _emailSettings.Port,
            SecureSocketOptions.StartTls,
            cancellationToken
        );

        await smtp.AuthenticateAsync(
            _emailSettings.Username,
            _emailSettings.Password,
            cancellationToken
        );

        await smtp.SendAsync(email, cancellationToken);
        await smtp.DisconnectAsync(true, cancellationToken);
    }
}
