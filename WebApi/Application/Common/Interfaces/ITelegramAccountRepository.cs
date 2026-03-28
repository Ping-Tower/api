using Domain.Entities;

namespace Application.Common.Interfaces;

public interface ITelegramAccountRepository
{
    Task<List<TelegramAccount>> GetAllByUserIdAsync(string userId, CancellationToken cancellationToken);
    Task CreateAsync(TelegramAccount account, CancellationToken cancellationToken);
    Task DeleteAsync(TelegramAccount account, CancellationToken cancellationToken);
}
