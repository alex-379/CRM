using CRM.API.Controllers;
using CRM.Business.Configuration;
using CRM.Business.Configuration.HttpClients;
using CRM.Business.Interfaces;
using CRM.Business.Models.Transactions.Requests;
using CRM.Business.Models.Transactions.Responses;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CRM.API.Tests.Controllers;

public class TransactionsControllerTests
{
    private readonly Mock<ITransactionsService> _transactionsServiceMock = new();
    private readonly Mock<IHttpClientService<TransactionStoreHttpClient>> _httpClientServiceMock = new();
    private readonly ServicesUrlSettings _servicesUrlSettings = new();
    
    [Fact]
    public async Task AddDepositTransactionAsync_DepositTransactionRequestSent_CreatedResultReceived()
    {
        //arrange
        var depositTransactionRequest = new TransactionRequest();
        _transactionsServiceMock.Setup(x => x.AddDepositTransactionAsync(depositTransactionRequest)).ReturnsAsync(new Guid());
        var sut = new TransactionsController(null, _transactionsServiceMock.Object, _servicesUrlSettings);

        //act
        var actual = await sut.AddDepositTransactionAsync(depositTransactionRequest);

        //assert
        actual.Result.Should().BeOfType<CreatedResult>();
        _transactionsServiceMock.Verify(m => m.AddDepositTransactionAsync(depositTransactionRequest), Times.Once);
    }
    
    [Fact]
    public async Task AddWithdrawTransactionAsync_DepositTransactionRequestSent_CreatedResultReceived()
    {
        //arrange
        var withdrawTransactionRequest = new TransactionRequest();
        _transactionsServiceMock.Setup(x => x.AddWithdrawTransactionAsync(withdrawTransactionRequest)).ReturnsAsync(new Guid());
        var sut = new TransactionsController(null, _transactionsServiceMock.Object, _servicesUrlSettings);

        //act
        var actual = await sut.AddWithdrawTransactionAsync(withdrawTransactionRequest);

        //assert
        actual.Result.Should().BeOfType<CreatedResult>();
        _transactionsServiceMock.Verify(m => m.AddWithdrawTransactionAsync(withdrawTransactionRequest), Times.Once);
    }
    
    [Fact]
    public async Task AddTransferTransactionAsync_TransferTransactionRequestSent_CreatedResultReceived()
    {
        //arrange
        var transferTransactionRequest = new CrmTransferRequest();
        _transactionsServiceMock.Setup(x => x.AddTransferTransactionAsync(transferTransactionRequest)).ReturnsAsync(new TransferGuidsResponse());
        var sut = new TransactionsController(null, _transactionsServiceMock.Object, _servicesUrlSettings);

        //act
        var actual = await sut.AddTransferTransactionAsync(transferTransactionRequest);

        //assert
        actual.Result.Should().BeOfType<CreatedResult>();
        _transactionsServiceMock.Verify(m => m.AddTransferTransactionAsync(transferTransactionRequest), Times.Once);
    }
    
    [Fact]
    public async Task GetTransactionByIdAsync_GuidSent_OkResultReceived()
    {
        //arrange
        _httpClientServiceMock.Setup(x => x.GetAsync<FullTransactionResponse>(It.IsAny<string>())).ReturnsAsync(new FullTransactionResponse());
        var sut = new TransactionsController(_httpClientServiceMock.Object, null, null);

        //act
        var actual = await sut.GetTransactionByIdAsync(Guid.NewGuid());

        //assert
        actual.Result.Should().BeOfType<OkObjectResult>();
        _httpClientServiceMock.Verify(m => m.GetAsync<FullTransactionResponse>(It.IsAny<string>()), Times.Once);
    }
}