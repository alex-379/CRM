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
public class TransactionsController(IHttpClientService httpClientService, ILeadsService leadsService, IAccountsService accountsService) : Controller
{
    private readonly Serilog.ILogger _logger = Log.ForContext<TransactionsController>();

    [HttpPost(Routes.Deposit)]
    public async Task<ActionResult<Guid>> AddDepositTransaction([FromBody] TransactionRequest request)
    {
        _logger.Information(LeadsLogs.GetAuthorizedLead);
        var currentLeadId = InformationCurrentLead.GetCurrentLeadFromClaims(HttpContext.User);
        await leadsService.CheckLeadRightsOnAccount(currentLeadId, request.AccountId);
        var url = Request.GetEncodedUrl();
        var account = await accountsService.GetAccountByIdAsync(request.AccountId);
        _logger.Information(TransactionsLogs.AddDepositTransaction, request.AccountId, account.Currency, currentLeadId);
        var tStoreRequest = new DepositWithdrawRequest()
        {
            AccountId = request.AccountId,
            Currency = account.Currency,
            Amount = request.Amount
        };
        var transactionId = await httpClientService.AddAsync(tStoreRequest, url);
        
        
        
        
        

        
        
        // var transactionId = await httpClientService.AddDepositWithdrawTransactionAsync(Transaction.Deposit, request);
        //
        // _logger.Information(
        //     $"A deposit transaction has been added for the account with Id {request.AccountId}. / Для счёта с Id {request.AccountId} добавлена транзакция на пополнение.");
        // var transactionId = await transactionsService.AddDepositWithdrawTransactionAsync(TransactionType.Deposit, request);
        // return Created($"/api/transactions/{transactionId}", transactionId);
        return null;
    }

}
