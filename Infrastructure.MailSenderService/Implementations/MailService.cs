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
    public async Task<string> SendAuthMail(string to, string subject, string body)
    {
        try
        {
            var content = "";
            using (var reader = new StreamReader("main.html"))
            {
                content = await reader.ReadToEndAsync();
                content = content.Replace("AuthenticationCode", body);
            }
            MailMessage mail = new MailMessage()
            {
                From = new MailAddress(_options.Email),
                Subject = subject,
                Body = content
            };
            mail.To.Add(to);
            mail.IsBodyHtml = true;
            using (var smtclinet = new SmtpClient())
            {
                smtclinet.Host = _options.Host;
                smtclinet.Port = _options.Port;
                smtclinet.Timeout = 1000;
                smtclinet.EnableSsl = true;
                smtclinet.Credentials = new System.Net.NetworkCredential(_options.Email, _options.Password);

                await smtclinet.SendMailAsync(mail);
                return "";
            }
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
}