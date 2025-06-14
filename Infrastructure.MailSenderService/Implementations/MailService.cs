using System.Net;
using System.Net.Mail;
using Core.Application.Interfaces.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace infrastructure.MailSenderService.Implementations;

public class MailService(IWebHostEnvironment env, IConfiguration configuration, ILogger<IMailService> _logger) : IMailService
{
    private readonly string _apiKey = configuration["SendGridApiKey"] ??
                                      throw new ArgumentNullException("SendGridApiKey is not configured");
    private readonly string _email = configuration["GmailOptions:Email"] ??
                                      throw new ArgumentNullException("GmailOptions:Email is not configured");
    public async Task<bool> SendAuthMail(string to, string subject, string body)
    {
        try
        {
            // Console.WriteLine("Sending email to {Email} with subject {Subject}", to, subject);
            var filePath = Path.Combine(AppContext.BaseDirectory, "Templates", "AuthCodeView.html");
            string viewBody;
            await using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                var reader = new StreamReader(fileStream);
                viewBody = await reader.ReadToEndAsync();
            }

            viewBody = viewBody.Replace("AuthenticationCode", body);
            
            var client = new SendGridClient(_apiKey);
            var from = new EmailAddress(_email);
            var toEmail = new EmailAddress(to);
            var msg = MailHelper.CreateSingleEmail(from, toEmail, subject, plainTextContent: null,
                htmlContent: viewBody);
            var response = await client.SendEmailAsync(msg);
            return response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Accepted;
        }
        catch (Exception)
        {
            return false;
        }
    }
}