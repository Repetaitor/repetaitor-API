using System.Net.Mail;
using Core.Application.Interfaces.Services;
using Core.Application.Models.Configuration;
using Microsoft.Extensions.Options;

namespace infrastructure.MailService.Implementations;

public class MailService : IMailService
{
    private readonly GmailOptions _options = new()
    {
        Email = "eventplannerauthentication@gmail.com",
        Password = "myhx avws mjpw gaiw",
        Host = "smtp.gmail.com",
        Port = 587
    };

    public async Task<bool> SendAuthMail(string to, string subject, string body)
    {
        try
        {
            var mail = new MailMessage()
            {
                From = new MailAddress(_options.Email),
                Subject = subject,
                Body = $"your auth code is {body}"
            };
            mail.To.Add(to);
            using (var smtclinet = new SmtpClient())
            {
                smtclinet.Host = _options.Host;
                smtclinet.Port = _options.Port;
                smtclinet.Timeout = 1000;
                smtclinet.EnableSsl = true;
                smtclinet.Credentials = new System.Net.NetworkCredential(_options.Email, _options.Password);

                await smtclinet.SendMailAsync(mail);
                return true;
            }
        }
        catch (Exception ex)
        {
            return false;
        }
    }
}