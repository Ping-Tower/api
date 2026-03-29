namespace Application.Common.Interfaces;

public interface ITelegramLoginValidator
{
    bool IsValid(
        long telegramUserId,
        string firstName,
        string? username,
        string? photoUrl,
        long authDate,
        string hash);

    DateTimeOffset GetAuthDateUtc(long authDate);
}
