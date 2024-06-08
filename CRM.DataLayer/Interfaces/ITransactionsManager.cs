using Microsoft.EntityFrameworkCore.Storage;

namespace CRM.DataLayer.Interfaces;

public interface ITransactionsManager
{
    Task<IDbContextTransaction> BeginTransactionAsync();
    Task CommitTransactionAsync(IDbContextTransaction transactionContext);
    Task RollbackTransactionAsync(IDbContextTransaction transactionContext, Exception ex);
}