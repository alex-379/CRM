using CRM.API.Configuration.Filters;
using CRM.API.Controllers.Constants;
using CRM.API.Controllers.Constants.Logs;
using CRM.Business.Configuration.HttpClients;
using CRM.Business.Interfaces;
using CRM.Business.Models.Accounts.Responses;
using CRM.Business.Models.Transactions.Requests;
using CRM.Business.Models.Transactions.Responses;
using CRM.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace CRM.API.Controllers;

[Authorize]
[ApiController]
[Route(Routes.TransactionsController)]
public class TransactionsController(IHttpClientService<TransactionStoreHttpClient> httpClientService, IAccountsService accountsService) : Controller
{
    private readonly Serilog.ILogger _logger = Log.ForContext<TransactionsController>();
    
    [AuthorizationFilterForTransactionByAccountId]
    [HttpPost(Routes.Deposit)]
    public async Task<ActionResult<Guid>> AddDepositTransaction([FromBody] TransactionRequest request)
    {
        var tStoreRequest = await CreateDepositWithdrawRequestTStore(request);
        var requestMessage = new HttpRequestMessage(HttpMethod.Post, Routes.DepositTStore);
        _logger.Information(TransactionsLogs.AddDepositTransaction, tStoreRequest.AccountId, tStoreRequest.CurrencyType);
        var id = await httpClientService.SendAsync<DepositWithdrawRequest,Guid>(tStoreRequest, requestMessage);
        
        return Created($"{Routes.HostTStore}{Routes.TransactionsController}/{id}", id);
    }
    
    [AuthorizationFilterForTransactionByAccountId]
    [HttpPost(Routes.Withdraw)]
    public async Task<ActionResult<Guid>> AddWithdrawTransaction([FromBody] TransactionRequest request)
    {
        var tStoreRequest = await CreateDepositWithdrawRequestTStore(request);
        var requestMessage = new HttpRequestMessage(HttpMethod.Post, Routes.WithdrawTStore);
        _logger.Information(TransactionsLogs.AddWithdrawTransaction, tStoreRequest.AccountId, tStoreRequest.CurrencyType);
        var id = await httpClientService.SendAsync<DepositWithdrawRequest,Guid>(tStoreRequest, requestMessage);
        
        return Created($"{Routes.HostTStore}{Routes.TransactionsController}/{id}", id);
    }
    
    [AuthorizationFilterForTransferByAccountsId]
    [HttpPost(Routes.Transfer)]
    public async Task<ActionResult<TransferGuidsResponse>> AddTransferTransaction([FromBody] CrmTransferRequest request)
    {
        var tStoreRequest = await CreateTransferRequestTStore(request);
        var requestMessage = new HttpRequestMessage(HttpMethod.Post, Routes.TransferTStore);
        _logger.Information(TransactionsLogs.AddTransferTransaction, tStoreRequest.AccountFromId, tStoreRequest.AccountToId);
        var response = await httpClientService.SendAsync<TransferRequest,TransferGuidsResponse>(tStoreRequest, requestMessage);
        
        return Created($"{Routes.HostTStore}{Routes.TransactionsController}/{response}", response);
    }
    
    [Authorize(Roles = nameof(LeadStatus.Administrator))]
    [HttpGet(Routes.Id)]
    public async Task<ActionResult<TransferGuidsResponse>> GetTransactionById(Guid id)
    {
        _logger.Information(TransactionsLogs.GetTransaction, id);
        var transactions = await httpClientService.GetAsync<List<TransactionWithAccountIdResponse>>(string.Format(Routes.TransactionsTStore, id));
        
        return Ok(transactions.FirstOrDefault());
    }
    
    private async Task<DepositWithdrawRequest> CreateDepositWithdrawRequestTStore(TransactionRequest request)
    {
        var account = await accountsService.GetAccountByIdAsync<AccountForTransactionResponse>(request.AccountId);
        var tStoreRequest = new DepositWithdrawRequest()
        {
            AccountId = request.AccountId,
            CurrencyType = account.Currency,
            Amount = request.Amount
        };

        return tStoreRequest;
    }
    
    private async Task<TransferRequest> CreateTransferRequestTStore(CrmTransferRequest request)
    {
        var accountFrom = await accountsService.GetAccountByIdAsync<AccountForTransactionResponse>(request.AccountFromId);
        var accountTo = await accountsService.GetAccountByIdAsync<AccountForTransactionResponse>(request.AccountToId);
        var tStoreRequest = new TransferRequest()
        {
            AccountToId = request.AccountToId,
            AccountFromId = request.AccountFromId,
            CurrencyToType = accountTo.Currency,
            CurrencyFromType = accountFrom.Currency,
            Amount = request.Amount
        };

        return tStoreRequest;
    }
}
