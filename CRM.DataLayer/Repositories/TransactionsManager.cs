using CRM.DataLayer.Interfaces;
using CRM.DataLayer.Repositories.Constants.Logs;
using Microsoft.EntityFrameworkCore.Storage;
using Serilog;

namespace CRM.DataLayer.Repositories;

public class TransactionsManager(CrmContext context) : BaseRepository(context), ITransactionsManager
{
    private readonly ILogger _logger = Log.ForContext<TransactionsManager>();

    public IDbContextTransaction BeginTransaction()
    {
        var transactionContext = _ctx.Database.BeginTransaction();
        _logger.Information(TransactionsManagerLogs.BeginTransaction, _ctx);

        return transactionContext;
    }

    public void CommitTransaction(IDbContextTransaction transactionContext)
    {
        transactionContext.Commit();
        _logger.Information(TransactionsManagerLogs.CommitTransaction, _ctx);
    }
}
