using Domain.Entities;

namespace Application.Common.Interfaces;

public interface ITelegramAccountRepository
{
    Task<List<TelegramAccount>> GetAllByUserIdAsync(string userId, CancellationToken cancellationToken);
    Task<TelegramAccount?> GetByUserIdAsync(string userId, CancellationToken cancellationToken);
    Task<TelegramAccount?> GetByTelegramUserIdAsync(long telegramUserId, CancellationToken cancellationToken);
    Task UpsertAsync(TelegramAccount account, CancellationToken cancellationToken);
    Task DeleteAsync(TelegramAccount account, CancellationToken cancellationToken);
}
