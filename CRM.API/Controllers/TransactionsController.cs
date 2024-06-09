using CRM.API.Controllers.Constants.Logs;
using CRM.Business.Interfaces;
using CRM.Business.Models.Transactions.Requests;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace CRM.API.Controllers;

public class TransactionsController(IHttpClientService httpClientService) : Controller
{
    private readonly Serilog.ILogger _logger = Log.ForContext<TransactionsController>();

    /*[HttpPost("deposit")]
    public async Task<ActionResult<Guid>> AddDepositTransaction([FromBody] DepositWithdrawRequest request)
    {
        _logger.Information(AccountsControllerLogs.GetAuthorizedAccount);
        var currentLeadId = InformationFromClaims.GetCurrentLeadFromClaims(HttpContext.User); 
        _logger.Information(TransactionsControllerLogs.AddDepositTransaction, request.AccountId, currentLeadId);
        var transactionId = await httpClientService.AddDepositWithdrawTransactionAsync(TransactionType.Deposit, request);
        
        _logger.Information(
            $"A deposit transaction has been added for the account with Id {request.AccountId}. / Для счёта с Id {request.AccountId} добавлена транзакция на пополнение.");
        var transactionId = await transactionsService.AddDepositWithdrawTransactionAsync(TransactionType.Deposit, request);
        return Created($"/api/transactions/{transactionId}", transactionId);
    }*/
    
    
}