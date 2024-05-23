using CRM.Core.Constants.Logs.DataLayer;
using CRM.DataLayer.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;
using Serilog;

namespace CRM.DataLayer.Repositories;

public class TransactionsRepository(CrmContext context) : BaseRepository(context), ITransactionsRepository
{
    private readonly ILogger _logger = Log.ForContext<TransactionsRepository>();

    public IDbContextTransaction BeginTransaction()
    {
        var transactionContext = _ctx.Database.BeginTransaction();
        _logger.Information(TransactionsRepositoryLogs.BeginTransaction, _ctx);

        return transactionContext;
    }

    public void CommitTransaction(IDbContextTransaction transactionContext)
    {
        transactionContext.Commit();
        _logger.Information(TransactionsRepositoryLogs.CommitTransaction, _ctx);
    }
}
