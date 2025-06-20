
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using System.Security.Cryptography;
using System.Text;

public class EmailSender : IEmailSender
{
    private readonly string smtpServer;
    private readonly int smtpPort;
    private readonly string smtpUser;
    private readonly string smtpPass;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<EmailSender> _logger;

    public EmailSender(IHttpContextAccessor httpContextAccessor, IConfiguration config, ILogger<EmailSender> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
        smtpServer = config["EmailSettings:SmtpServer"];
        smtpPort = int.Parse(config["EmailSettings:SmtpPort"]);
        smtpUser = config["EmailSettings:SmtpUser"];
        smtpPass = config["EmailSettings:SmtpPass"];
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Mobi Pay", smtpUser));
        message.To.Add(MailboxAddress.Parse(toEmail));
        message.Subject = subject;

        var bodyBuilder = new BodyBuilder { HtmlBody = body };
        message.Body = bodyBuilder.ToMessageBody();

        using var client = new SmtpClient();
        await client.ConnectAsync(smtpServer, smtpPort, SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(smtpUser, smtpPass);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }

    public async Task SendOtpAsync(string email)
    {
        var otp = new Random().Next(100000, 999999).ToString();
        var subject = "Your OTP Code";
        var body = $"<p>Your OTP code is: <strong>{otp}</strong></p><p>This code will expire in 5 minutes.</p>";

        await SendEmailAsync(email, subject, body);

        var session = _httpContextAccessor.HttpContext.Session;
        session.SetString("OtpHash", HashOtp(otp));
        session.SetString("OtpExpiry", DateTime.UtcNow.AddMinutes(5).ToString("O"));
        session.SetInt32("OtpAttempts", 0);

        _logger.LogInformation("OTP sent to {Email}", email);
    }

    public bool VerifyOtp(string enteredOtp)
    {
        var session = _httpContextAccessor.HttpContext.Session;
        var storedHash = session.GetString("OtpHash");
        var expiryStr = session.GetString("OtpExpiry");
        var attempts = session.GetInt32("OtpAttempts") ?? 0;

        if (attempts >= 5)
        {
            _logger.LogWarning("Too many failed OTP attempts.");
            return false;
        }

        if (string.IsNullOrEmpty(storedHash) || !DateTime.TryParse(expiryStr, out var expiry) || DateTime.UtcNow > expiry)
        {
            _logger.LogWarning("OTP expired or missing.");
            return false;
        }

        if (HashOtp(enteredOtp) != storedHash)
        {
            session.SetInt32("OtpAttempts", attempts + 1);
            _logger.LogWarning("Invalid OTP entered.");
            return false;
        }

        _logger.LogInformation("OTP verified successfully.");
        return true;
    }










    public async Task<string> SendPasswordAsync(string email, string password)
    {
        string subject = "Your New Account Password";
        string body = $@"
        <p>Welcome,</p>
        <p>Your Account Has Been Created And Your account Username And password is: <strong>{password}</strong></p>";

        await SendEmailAsync(email, subject, body);

        _logger.LogInformation("Password sent to {Email}", email);
        return password; // Return it so it can be stored (e.g., hashed) by the caller
    }











    private string HashOtp(string otp)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(otp);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}
