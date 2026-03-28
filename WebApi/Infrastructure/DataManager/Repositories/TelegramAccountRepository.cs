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

    public async Task CreateAsync(TelegramAccount account, CancellationToken cancellationToken)
    {
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
