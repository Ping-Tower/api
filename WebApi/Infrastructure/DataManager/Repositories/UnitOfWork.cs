using Application.Common.Interfaces;
using Infrastructure.DataManager.Contexts;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infrastructure.DataManager.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private IDbContextTransaction? _dbTransaction;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }
    public async Task BeginTransactionAsync(CancellationToken cancellationToken)
    {
        _dbTransaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken)
    {
        if(_dbTransaction != null)
        {
            await _dbTransaction.CommitAsync(cancellationToken);
            await _dbTransaction.DisposeAsync();
            _dbTransaction = null;
        }
    }
    
    public async Task RollbackTransactionAsync(CancellationToken cancellationToken)
    {
        if(_dbTransaction != null)
        {
            await _dbTransaction.RollbackAsync(cancellationToken);
            await _dbTransaction.DisposeAsync();
            _dbTransaction = null;
        }
    }
}