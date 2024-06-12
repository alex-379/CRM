using CRM.API.Controllers.Constants;
using CRM.API.Controllers.Constants.Logs;
using CRM.Business.Interfaces;
using CRM.Business.Models.Transactions.Requests;
using CRM.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace CRM.API.Controllers;

[Authorize]
[ApiController]
[Route(Routes.TransactionsController)]
public class TransactionsController(IHttpClientService httpClientService, ILeadsService leadsService) : Controller
{
    private readonly Serilog.ILogger _logger = Log.ForContext<TransactionsController>();

    [HttpPost(Routes.Deposit)]
    public async Task<ActionResult<Guid>> AddDepositTransaction([FromBody] DepositWithdrawRequest request)
    {
        var url = Request.GetEncodedUrl();
        _logger.Information(AccountsControllerLogs.GetAuthorizedAccount);
        var currentLeadId = InformationCurrentLead.GetCurrentLeadFromClaims(HttpContext.User); 
        _logger.Information(TransactionsControllerLogs.AddDepositTransaction, request.AccountId, currentLeadId);
        
        
        // var transactionId = await httpClientService.AddDepositWithdrawTransactionAsync(Transaction.Deposit, request);
        //
        // _logger.Information(
        //     $"A deposit transaction has been added for the account with Id {request.AccountId}. / Для счёта с Id {request.AccountId} добавлена транзакция на пополнение.");
        // var transactionId = await transactionsService.AddDepositWithdrawTransactionAsync(TransactionType.Deposit, request);
        // return Created($"/api/transactions/{transactionId}", transactionId);
        return null;
    }

}
