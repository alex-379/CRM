using Microsoft.EntityFrameworkCore.Storage;

namespace CRM.DataLayer.Interfaces;

public interface ITransactionsRepository
{
    IDbContextTransaction BeginTransaction();
    void CommitTransaction(IDbContextTransaction transactionContext);
}