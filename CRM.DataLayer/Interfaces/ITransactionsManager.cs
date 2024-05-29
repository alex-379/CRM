using Microsoft.EntityFrameworkCore.Storage;

namespace CRM.DataLayer.Interfaces;

public interface ITransactionsManager
{
    IDbContextTransaction BeginTransaction();
    void CommitTransaction(IDbContextTransaction transactionContext);
}