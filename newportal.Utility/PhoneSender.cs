using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

public class PhoneSender
{
    private readonly string _accountSid;
    private readonly string _authToken;
    private readonly string _fromPhoneNumber;
    private readonly ILogger<PhoneSender> _logger;

    private readonly ConcurrentDictionary<string, OtpEntry> _otpStore = new();
    private readonly TimeSpan _otpTtl = TimeSpan.FromMinutes(5);
    private readonly int _maxAttempts = 5;
    private readonly int _otpLength = 6;

    public PhoneSender(IConfiguration configuration, ILogger<PhoneSender> logger)
    {
        _accountSid = configuration["Twilio:AccountSid"] ?? throw new ArgumentNullException("Twilio:AccountSid");
        _authToken = configuration["Twilio:AuthToken"] ?? throw new ArgumentNullException("Twilio:AuthToken");
        _fromPhoneNumber = configuration["Twilio:FromPhoneNumber"] ?? throw new ArgumentNullException("Twilio:FromPhoneNumber");
        _logger = logger;

        TwilioClient.Init(_accountSid, _authToken);
    }

    public async Task<bool> SendOtpAsync(string phoneNumber)
    {
        if (!IsValidPhoneNumber(phoneNumber))
        {
            _logger.LogWarning("Invalid phone number format: {Phone}", phoneNumber);
            return false;
        }

        var otp = GenerateSecureOtp(_otpLength);
        var hashedOtp = HashOtp(otp);

        _otpStore[phoneNumber] = new OtpEntry
        {
            HashedOtp = hashedOtp,
            ExpiryTime = DateTime.UtcNow.Add(_otpTtl),
            Attempts = 0
        };

        try
        {
            var message = await MessageResource.CreateAsync(
                body: $"Your OTP Verifiication is: {otp}",
                from: new Twilio.Types.PhoneNumber(_fromPhoneNumber),
                to: new Twilio.Types.PhoneNumber(phoneNumber)
            );

            return message.ErrorCode == null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send OTP to {Phone}", phoneNumber);
            return false;
        }
    }

    public bool VerifyOtp(string phoneNumber, string inputOtp)
    {
        if (!_otpStore.TryGetValue(phoneNumber, out var entry))
            return false;

        if (DateTime.UtcNow > entry.ExpiryTime || entry.Attempts >= _maxAttempts)
        {
            _otpStore.TryRemove(phoneNumber, out _);
            return false;
        }

        entry.Attempts++;

        if (VerifyHashedOtp(entry.HashedOtp, inputOtp))
        {
            _otpStore.TryRemove(phoneNumber, out _); // OTP is single-use
            return true;
        }

        if (entry.Attempts >= _maxAttempts)
        {
            _otpStore.TryRemove(phoneNumber, out _);
        }

        return false;
    }

    // Secure OTP generator using RNGCryptoServiceProvider
    private string GenerateSecureOtp(int length)
    {
        const string digits = "0123456789";
        var otp = new StringBuilder(length);
        using var rng = RandomNumberGenerator.Create();

        for (int i = 0; i < length; i++)
        {
            var randomNumber = GetRandomNumber(rng, digits.Length);
            otp.Append(digits[randomNumber]);
        }

        return otp.ToString();
    }

    private int GetRandomNumber(RandomNumberGenerator rng, int max)
    {
        byte[] bytes = new byte[4];
        int value;
        do
        {
            rng.GetBytes(bytes);
            value = BitConverter.ToInt32(bytes, 0) & int.MaxValue;
        } while (value >= max * (int.MaxValue / max));

        return value % max;
    }

    private string HashOtp(string otp)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(otp));
        return Convert.ToBase64String(bytes);
    }

    private bool VerifyHashedOtp(string hashedOtp, string inputOtp)
    {
        var inputHash = HashOtp(inputOtp);
        return CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(hashedOtp),
            Encoding.UTF8.GetBytes(inputHash)
        );
    }

    private bool IsValidPhoneNumber(string phone)
    {
        return Regex.IsMatch(phone, @"^\+[1-9]\d{9,14}$");
    }

    private class OtpEntry
    {
        public string HashedOtp { get; set; }
        public DateTime ExpiryTime { get; set; }
        public int Attempts { get; set; }
    }
}
