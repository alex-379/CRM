using CRM.Business.Models.Transactions.Requests;
using CRM.Business.Models.Transactions.Responses;

namespace CRM.Business.Interfaces;

public interface ITransactionsService
{
    Task<Guid> AddDepositTransaction(TransactionRequest request);
    Task<Guid> AddWithdrawTransaction(TransactionRequest request);
    Task<TransferGuidsResponse> AddTransferTransaction(CrmTransferRequest request);
    Task<List<TransactionResponse>> GetTransactionsByAccountId(Guid id);
    Task<AccountBalanceResponse> GetBalanceByAccountId(Guid id);
}