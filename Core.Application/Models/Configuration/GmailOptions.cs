namespace Core.Application.Models.Configuration;

public class GmailOptions
{
    public const string GmailOptionsKey = "GmailOptions";
    
    public string Email { get; set; }
    public string Password { get; set; }
    public string Host { get; set; }
    public int Port { get; set; }
}