using Application.Common.Interfaces;
using Domain.Entities;
using Infrastructure.DataManager.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataManager.Repositories;

public class TelegramAccountRepository : ITelegramAccountRepository
{
    private readonly AppDbContext _dbContext;

    public TelegramAccountRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<TelegramAccount>> GetAllByUserIdAsync(string userId, CancellationToken cancellationToken)
    {
        return await _dbContext.TelegramAccount
            .Where(t => t.UserId == userId && !t.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<TelegramAccount?> GetByUserIdAsync(string userId, CancellationToken cancellationToken)
    {
        return await _dbContext.TelegramAccount
            .FirstOrDefaultAsync(t => t.UserId == userId, cancellationToken);
    }

    public async Task<TelegramAccount?> GetByTelegramUserIdAsync(long telegramUserId, CancellationToken cancellationToken)
    {
        return await _dbContext.TelegramAccount
            .FirstOrDefaultAsync(t => t.TelegramUserId == telegramUserId, cancellationToken);
    }

    public async Task UpsertAsync(TelegramAccount account, CancellationToken cancellationToken)
    {
        var exists = await _dbContext.TelegramAccount
            .AnyAsync(t => t.Id == account.Id, cancellationToken);

        if (exists)
            _dbContext.TelegramAccount.Update(account);
        else
            await _dbContext.TelegramAccount.AddAsync(account, cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(TelegramAccount account, CancellationToken cancellationToken)
    {
        account.IsDeleted = true;
        _dbContext.TelegramAccount.Update(account);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
