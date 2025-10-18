using System.Net;
using System.Net.Mail;
using Core.Application.Interfaces.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
namespace infrastructure.MailSenderService.Implementations;

public class MailService(IWebHostEnvironment env, IConfiguration configuration, ILogger<IMailService> _logger)
    : IMailService
{
    private readonly string _email = configuration["GmailOptions:Email"] ??
                                     throw new ArgumentNullException("GmailOptions:Email is not configured");

    private readonly string _host = configuration["GmailOptions:Host"] ??
                                    throw new ArgumentNullException("GmailOptions:Host is not configured");

    private readonly int _port = int.Parse(configuration["GmailOptions:Port"] ?? "587");

    private readonly string _password = configuration["GmailOptions:Password"] ??
                                        throw new ArgumentNullException("GmailOptions:Password is not configured");

    public async Task<bool> SendAuthMail(string to, string subject, string body)
    {
        try
        {
            var filePath = Path.Combine(AppContext.BaseDirectory, "Templates", "AuthCodeView.html");
            string viewBody;
            await using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                var reader = new StreamReader(fileStream);
                viewBody = await reader.ReadToEndAsync();
            }

            viewBody = viewBody.Replace("AuthenticationCode", body);
            var mail = new MailMessage();
            mail.From = new MailAddress(_email);
            mail.To.Add(to);
            mail.Subject = subject;
            mail.IsBodyHtml = true;
            mail.Body = viewBody;

            var smtpClient = new SmtpClient(_host)
            {
                Port = _port,
                Credentials = new NetworkCredential(_email, _password),
                EnableSsl = true,
            };
            await smtpClient.SendMailAsync(mail);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}