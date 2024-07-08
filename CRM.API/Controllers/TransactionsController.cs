using CRM.API.Configuration.Filters;
using CRM.API.Controllers.Constants;
using CRM.API.Controllers.Constants.Logs;
using CRM.Business.Configuration;
using CRM.Business.Configuration.HttpClients;
using CRM.Business.Interfaces;
using CRM.Business.Models.Transactions.Requests;
using CRM.Business.Models.Transactions.Responses;
using CRM.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace CRM.API.Controllers;

[Authorize]
[ApiController]
[Route($"{Routes.Api}{Routes.TransactionsController}")]
public class TransactionsController(IHttpClientService<TransactionStoreHttpClient> httpClientService, IAccountsService accountsService,
    ITransactionsService transactionsService, ServicesUrlSettings servicesUrlSettings) 
    : Controller
{
    private readonly Serilog.ILogger _logger = Log.ForContext<TransactionsController>();
    
    [AuthorizationFilterForTransactionByAccountId]
    [HttpPost(Routes.Deposit)]
    public async Task<ActionResult<Guid>> AddDepositTransaction([FromBody] TransactionRequest request)
    {
        var tStoreRequest = await transactionsService.CreateDepositWithdrawRequestTStore(request);
        var requestMessage = new HttpRequestMessage(HttpMethod.Post, Routes.DepositTStore);
        _logger.Information(TransactionsLogs.AddDepositTransaction, tStoreRequest.AccountId, tStoreRequest.Currency);
        var id = await httpClientService.SendAsync<DepositWithdrawRequest,Guid>(tStoreRequest, requestMessage);
        
        return Created($"{servicesUrlSettings.TransactionStore}{Routes.TransactionsController}/{id}", id);
    }
    
    [AuthorizationFilterForTransactionByAccountId]
    [HttpPost(Routes.Withdraw)]
    public async Task<ActionResult<Guid>> AddWithdrawTransaction([FromBody] TransactionRequest request)
    {
        var tStoreRequest = await transactionsService.CreateDepositWithdrawRequestTStore(request);
        var requestMessage = new HttpRequestMessage(HttpMethod.Post, Routes.WithdrawTStore);
        _logger.Information(TransactionsLogs.AddWithdrawTransaction, tStoreRequest.AccountId, tStoreRequest.Currency);
        var id = await httpClientService.SendAsync<DepositWithdrawRequest,Guid>(tStoreRequest, requestMessage);
        
        return Created($"{servicesUrlSettings.TransactionStore}{Routes.TransactionsController}/{id}", id);
    }
    
    [AuthorizationFilterForTransferByAccountsId]
    [HttpPost(Routes.Transfer)]
    public async Task<ActionResult<TransferGuidsResponse>> AddTransferTransaction([FromBody] CrmTransferRequest request)
    {
        var tStoreRequest = await transactionsService.CreateTransferRequestTStore(request);
        var requestMessage = new HttpRequestMessage(HttpMethod.Post, Routes.TransferTStore);
        _logger.Information(TransactionsLogs.AddTransferTransaction, tStoreRequest.AccountFromId, tStoreRequest.AccountToId);
        var response = await httpClientService.SendAsync<TransferRequest,TransferGuidsResponse>(tStoreRequest, requestMessage);
        
        return Created($"{servicesUrlSettings.TransactionStore}{Routes.TransactionsController}/{response}", response);
    }
    
    [Authorize(Roles = nameof(LeadStatus.Administrator))]
    [HttpGet(Routes.Id)]
    public async Task<ActionResult<FullTransactionResponse>> GetTransactionById(Guid id)
    {
        _logger.Information(TransactionsLogs.GetTransaction, id);
        var transactions = await httpClientService.GetAsync<FullTransactionResponse>(string.Format(Routes.TransactionsTStore, id));
        
        return Ok(transactions);
    }
}
