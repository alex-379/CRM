using CRM.Business.Models.Transactions.Requests;

namespace CRM.Business.Interfaces;

public interface ITransactionsService
{
    Task<DepositWithdrawRequest> CreateDepositWithdrawRequestTStore(TransactionRequest request);
    Task<TransferRequest> CreateTransferRequestTStore(CrmTransferRequest request);
}