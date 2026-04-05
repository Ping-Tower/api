using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using Application.Common.Interfaces;
using Domain.Common.Exceptions;

namespace Infrastructure.TelegramManager;

public class TelegramLoginValidator : ITelegramLoginValidator
{
    private readonly TelegramAuthSettings _settings;

    public TelegramLoginValidator(TelegramAuthSettings settings)
    {
        _settings = settings;
    }

    public bool IsValid(
        long telegramUserId,
        string firstName,
        string? username,
        string? photoUrl,
        long authDate,
        string hash)
    {
        if (string.IsNullOrWhiteSpace(_settings.BotToken))
            throw new DomainException("telegram", ["Telegram bot token is not configured."]);

        var authDateUtc = GetAuthDateUtc(authDate);
        if (authDateUtc.AddMinutes(_settings.AuthLifetimeMinutes) < DateTimeOffset.UtcNow)
            return false;

        var payload = new SortedDictionary<string, string>(StringComparer.Ordinal)
        {
            ["auth_date"] = authDate.ToString(CultureInfo.InvariantCulture),
            ["first_name"] = firstName,
            ["id"] = telegramUserId.ToString(CultureInfo.InvariantCulture)
        };

        if (!string.IsNullOrWhiteSpace(username))
            payload["username"] = username;

        if (!string.IsNullOrWhiteSpace(photoUrl))
            payload["photo_url"] = photoUrl;

        var dataCheckString = string.Join('\n', payload.Select(kvp => $"{kvp.Key}={kvp.Value}"));

        using var sha256 = SHA256.Create();
        var secretKey = sha256.ComputeHash(Encoding.UTF8.GetBytes(_settings.BotToken));
        using var hmac = new HMACSHA256(secretKey);
        var computedHashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(dataCheckString));
        var computedHash = Convert.ToHexString(computedHashBytes).ToLowerInvariant();

        return CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(computedHash),
            Encoding.UTF8.GetBytes(hash.ToLowerInvariant()));
    }

    public DateTimeOffset GetAuthDateUtc(long authDate)
    {
        try
        {
            return DateTimeOffset.FromUnixTimeSeconds(authDate).ToUniversalTime();
        }
        catch (ArgumentOutOfRangeException)
        {
            throw new DomainException("telegram", ["Telegram auth_date is invalid."]);
        }
    }
}
