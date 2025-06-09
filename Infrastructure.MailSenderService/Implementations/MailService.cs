using System.Net;
using System.Net.Mail;
using Core.Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;

namespace infrastructure.MailSenderService.Implementations;

public class MailService : IMailService
{
    private readonly IConfigurationRoot configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .Build();

    public async Task<bool> SendAuthMail(string to, string subject, string body)
    {
        try
        {
            var mail = new MailMessage()
            {
                From = new MailAddress(configuration["GmailOptions:Email"]!),
                Subject = subject,
                Body = $"your auth code is {body}"
            };
            mail.To.Add(to);
            using (var smtclinet = new SmtpClient())
            {
                smtclinet.Host = configuration["GmailOptions:Host"]!;
                smtclinet.Port = int.Parse(configuration["GmailOptions:Port"]!);
                smtclinet.Timeout = 1000;
                smtclinet.EnableSsl = true;
                smtclinet.Credentials = new NetworkCredential(configuration["GmailOptions:Email"]!,
                    configuration["GmailOptions:Password"]!);

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