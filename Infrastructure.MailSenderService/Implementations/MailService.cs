using System.Net;
using System.Net.Mail;
using Core.Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace infrastructure.MailSenderService.Implementations;

public class MailService(IConfiguration configuration, ILogger<IMailService> _logger) : IMailService
{
    public async Task<bool> SendAuthMail(string to, string subject, string body)
    {
        try
        {
            var fileName = Path.Combine(Directory.GetCurrentDirectory(), "AuthCodeView.html");
            string viewBody;
            await using (var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                var reader = new StreamReader(fileStream);
                viewBody = await reader.ReadToEndAsync();
            }
            viewBody = viewBody.Replace("AuthenticationCode", body);
            var mail = new MailMessage()
            {
                From = new MailAddress(configuration["GmailOptions:Email"]!),
                Subject = subject,
                Body = viewBody,
                IsBodyHtml =  true
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
            _logger.LogInformation("Failed to send email to {Email} with subject {Subject}. Exception : {ex}", to, subject, ex);
            return false;
        }
    }
}