namespace CRM.Core.Constants.Logs.DataLayer;

public static class TransactionsRepositoryLogs
{
    public const string BeginTransaction = "Starting transaction for the database {_ctx}]";
    public const string CommitTransaction = "Committing transaction for the database {_ctx}]";
}
