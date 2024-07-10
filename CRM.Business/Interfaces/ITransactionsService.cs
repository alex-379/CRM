using CRM.Business.Models.Transactions.Requests;
using CRM.Business.Models.Transactions.Responses;

namespace CRM.Business.Interfaces;

public interface ITransactionsService
{
    Task<Guid> AddDepositTransactionAsync(TransactionRequest request);
    Task<Guid> AddWithdrawTransactionAsync(TransactionRequest request);
    Task<TransferGuidsResponse> AddTransferTransactionAsync(CrmTransferRequest request);
    Task<List<TransactionResponse>> GetTransactionsByAccountIdAsync(Guid id);
    Task<AccountBalanceResponse> GetBalanceByAccountIdAsync(Guid id);
}