namespace CRM.Core.Constants.Logs.DataLayer;

public static class TransactionsRepositoryLogs
{
    public const string BeginTransaction = "Start transaction for the database {_ctx}]";
    public const string CommitTransaction = "Commiting transaction for the database  {_ctx}]";
}
