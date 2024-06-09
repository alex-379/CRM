using CRM.DataLayer.Interfaces;
using CRM.DataLayer.Repositories.Constants.Logs;
using Microsoft.EntityFrameworkCore.Storage;
using Serilog;

namespace CRM.DataLayer.Repositories;

public class TransactionsManager(CrmContext context) : BaseRepository(context), ITransactionsManager
{
    private readonly ILogger _logger = Log.ForContext<TransactionsManager>();

    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        var transactionContext = await _ctx.Database.BeginTransactionAsync();
        _logger.Information(TransactionsManagerLogs.BeginTransaction, _ctx);

        return transactionContext;
    }

    public async Task CommitTransactionAsync(IDbContextTransaction transactionContext)
    {
        await transactionContext.CommitAsync();
        _logger.Information(TransactionsManagerLogs.CommitTransaction, _ctx);
    }
    
    public async Task RollbackTransactionAsync(IDbContextTransaction transactionContext, Exception ex)
    {
        await transactionContext.RollbackAsync();
        _logger.Error(ex, ex.Message);
    }
}
