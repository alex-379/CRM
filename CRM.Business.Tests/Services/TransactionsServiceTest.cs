using AutoFixture;
using CRM.Business.Configuration.HttpClients;
using CRM.Business.Interfaces;
using CRM.Business.Models.Accounts.Responses;
using CRM.Business.Models.Leads.Responses;
using CRM.Business.Models.Transactions.Requests;
using CRM.Business.Models.Transactions.Responses;
using CRM.Business.Services;
using CRM.Business.Services.Constants.Exceptions;
using CRM.Core;
using CRM.Core.Enums;
using CRM.Core.Exceptions;
using CRM.Core.Fixture;
using FluentAssertions;
using Moq;

namespace CRM.Business.Tests.Services;

public class TransactionsServiceTest
{
    private readonly Mock<IHttpClientService<TransactionStoreHttpClient>> _httpClientServiceMock = new();
    private readonly Mock<IAccountsService> _accountsServiceMock = new();
    private readonly Mock<ILeadsService> _leadsServiceMock = new();
    private readonly CustomFixture _customFixture = new();

    [Fact]
    public async Task AddDepositTransactionAsync_TransactionValidRequestSent_GuidReceived()
    {
        //arrange
        var fixture = _customFixture.GetFixture();
        var depositTransactionRequest = fixture.Create<TransactionRequest>();
        var account = fixture.Create<AccountForTransactionResponse>();
        account.Currency = Currency.Rub;
        _accountsServiceMock.Setup(x => x.GetAccountByIdAsync<AccountForTransactionResponse>(It.IsAny<Guid>())).ReturnsAsync(account);
        _httpClientServiceMock.Setup(m => m.SendAsync<DepositWithdrawRequest,Guid>(It.IsAny<DepositWithdrawRequest>(), It.IsAny<HttpRequestMessage>()))
            .ReturnsAsync(new Guid());
        var sut = new TransactionsService(_accountsServiceMock.Object, null, _httpClientServiceMock.Object);

        //act
        await sut.AddDepositTransactionAsync(depositTransactionRequest);

        //assert
        _accountsServiceMock.Verify(m => m.GetAccountByIdAsync<AccountForTransactionResponse>(It.IsAny<Guid>()), Times.Once);
        _httpClientServiceMock.Verify(m => m.SendAsync<DepositWithdrawRequest,Guid>(It.IsAny<DepositWithdrawRequest>(), It.IsAny<HttpRequestMessage>()), Times.Once);
    }
    
    [Fact]
    public async Task AddDepositTransactionAsync_TransactionUnknownCurrencyRequestSent_ValidationErrorReceived()
    {
        //arrange
        var (_, allowedCurrencyNames) = AllowedCurrencies.GetAllowedCurrenciesForDepositWithdrawTransaction();
        var fixture = _customFixture.GetFixture();
        var depositTransactionRequest = fixture.Create<TransactionRequest>();
        var account = fixture.Create<AccountForTransactionResponse>();
        account.Currency = Currency.Unknown;
        _accountsServiceMock.Setup(x => x.GetAccountByIdAsync<AccountForTransactionResponse>(It.IsAny<Guid>())).ReturnsAsync(account);
        _httpClientServiceMock.Setup(m => m.SendAsync<DepositWithdrawRequest,Guid>(It.IsAny<DepositWithdrawRequest>(), It.IsAny<HttpRequestMessage>()))
            .ReturnsAsync(new Guid());
        var sut = new TransactionsService(_accountsServiceMock.Object, null, _httpClientServiceMock.Object);

        //act
        var act = async () => await sut.AddDepositTransactionAsync(depositTransactionRequest);

        //assert
        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage(string.Format(TransactionsServiceExceptions.CurrencyForDepositWithdrawTransaction, string.Join(",", allowedCurrencyNames)));
        _accountsServiceMock.Verify(m => m.GetAccountByIdAsync<AccountForTransactionResponse>(It.IsAny<Guid>()), Times.Once);
        _httpClientServiceMock.Verify(m => m.SendAsync<DepositWithdrawRequest,Guid>(It.IsAny<DepositWithdrawRequest>(), It.IsAny<HttpRequestMessage>()), Times.Never);
    }
    
    [Fact]
    public async Task AddDepositTransactionAsync_TransactionUnallowedCurrencyRequestSent_ValidationErrorReceived()
    {
        //arrange
        var (_, allowedCurrencyNames) = AllowedCurrencies.GetAllowedCurrenciesForDepositWithdrawTransaction();
        var fixture = _customFixture.GetFixture();
        var depositTransactionRequest = fixture.Create<TransactionRequest>();
        var account = fixture.Create<AccountForTransactionResponse>();
        account.Currency = Currency.Jpy;
        _accountsServiceMock.Setup(x => x.GetAccountByIdAsync<AccountForTransactionResponse>(It.IsAny<Guid>())).ReturnsAsync(account);
        _httpClientServiceMock.Setup(m => m.SendAsync<DepositWithdrawRequest,Guid>(It.IsAny<DepositWithdrawRequest>(), It.IsAny<HttpRequestMessage>()))
            .ReturnsAsync(new Guid());
        var sut = new TransactionsService(_accountsServiceMock.Object, null, _httpClientServiceMock.Object);

        //act
        var act = async () => await sut.AddDepositTransactionAsync(depositTransactionRequest);

        //assert
        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage(string.Format(TransactionsServiceExceptions.CurrencyForDepositWithdrawTransaction, string.Join(",", allowedCurrencyNames)));
        _accountsServiceMock.Verify(m => m.GetAccountByIdAsync<AccountForTransactionResponse>(It.IsAny<Guid>()), Times.Once);
        _httpClientServiceMock.Verify(m => m.SendAsync<DepositWithdrawRequest,Guid>(It.IsAny<DepositWithdrawRequest>(), It.IsAny<HttpRequestMessage>()), Times.Never);
    }
    
    [Fact]
    public async Task AddWithdrawTransactionAsync_TransactionValidRequestAmountEqualBalanceSent_GuidReceived()
    {
        //arrange
        var fixture = _customFixture.GetFixture();
        var withdrawTransactionRequest = fixture.Create<TransactionRequest>();
        var account = fixture.Create<AccountForTransactionResponse>();
        var accountResponse = fixture.Create<AccountBalanceResponse>();
        account.Currency = Currency.Usd;
        _accountsServiceMock.Setup(x => x.GetAccountByIdAsync<AccountForTransactionResponse>(It.IsAny<Guid>())).ReturnsAsync(account);
        _httpClientServiceMock.Setup(m => m.GetAsync<AccountBalanceResponse>(It.IsAny<string>()))
            .ReturnsAsync(accountResponse);
        accountResponse.Balance = withdrawTransactionRequest.Amount;
        _httpClientServiceMock.Setup(m => m.SendAsync<DepositWithdrawRequest,Guid>(It.IsAny<DepositWithdrawRequest>(), It.IsAny<HttpRequestMessage>()))
            .ReturnsAsync(new Guid());
        var sut = new TransactionsService(_accountsServiceMock.Object, null, _httpClientServiceMock.Object);

        //act
        await sut.AddWithdrawTransactionAsync(withdrawTransactionRequest);

        //assert
        _httpClientServiceMock.Verify(m => m.GetAsync<AccountBalanceResponse>(It.IsAny<string>()), Times.Once);
        _accountsServiceMock.Verify(m => m.GetAccountByIdAsync<AccountForTransactionResponse>(It.IsAny<Guid>()), Times.Once);
        _httpClientServiceMock.Verify(m => m.SendAsync<DepositWithdrawRequest,Guid>(It.IsAny<DepositWithdrawRequest>(), It.IsAny<HttpRequestMessage>()), Times.Once);
    }
    
    [Fact]
    public async Task AddWithdrawTransactionAsync_TransactionValidRequestAmountLessBalanceSent_GuidReceived()
    {
        //arrange
        var fixture = _customFixture.GetFixture();
        var withdrawTransactionRequest = fixture.Create<TransactionRequest>();
        var account = fixture.Create<AccountForTransactionResponse>();
        var accountResponse = fixture.Create<AccountBalanceResponse>();
        account.Currency = Currency.Usd;
        _accountsServiceMock.Setup(x => x.GetAccountByIdAsync<AccountForTransactionResponse>(It.IsAny<Guid>())).ReturnsAsync(account);
        _httpClientServiceMock.Setup(m => m.GetAsync<AccountBalanceResponse>(It.IsAny<string>()))
            .ReturnsAsync(accountResponse);
        accountResponse.Balance = withdrawTransactionRequest.Amount + 1;
        _httpClientServiceMock.Setup(m => m.SendAsync<DepositWithdrawRequest,Guid>(It.IsAny<DepositWithdrawRequest>(), It.IsAny<HttpRequestMessage>()))
            .ReturnsAsync(new Guid());
        var sut = new TransactionsService(_accountsServiceMock.Object, null, _httpClientServiceMock.Object);

        //act
        await sut.AddWithdrawTransactionAsync(withdrawTransactionRequest);

        //assert
        _httpClientServiceMock.Verify(m => m.GetAsync<AccountBalanceResponse>(It.IsAny<string>()), Times.Once);
        _accountsServiceMock.Verify(m => m.GetAccountByIdAsync<AccountForTransactionResponse>(It.IsAny<Guid>()), Times.Once);
        _httpClientServiceMock.Verify(m => m.SendAsync<DepositWithdrawRequest,Guid>(It.IsAny<DepositWithdrawRequest>(), It.IsAny<HttpRequestMessage>()), Times.Once);
    }
    
    [Fact]
    public async Task AddWithdrawTransactionAsync_TransactionRequestAmountMoreBalanceSent_ValidationErrorReceived()
    {
        //arrange
        var fixture = _customFixture.GetFixture();
        var withdrawTransactionRequest = fixture.Create<TransactionRequest>();
        var account = fixture.Create<AccountForTransactionResponse>();
        var accountResponse = fixture.Create<AccountBalanceResponse>();
        account.Currency = Currency.Usd;
        _accountsServiceMock.Setup(x => x.GetAccountByIdAsync<AccountForTransactionResponse>(It.IsAny<Guid>())).ReturnsAsync(account);
        _httpClientServiceMock.Setup(m => m.GetAsync<AccountBalanceResponse>(It.IsAny<string>()))
            .ReturnsAsync(accountResponse);
        accountResponse.Balance = withdrawTransactionRequest.Amount -1;
        _httpClientServiceMock.Setup(m => m.SendAsync<DepositWithdrawRequest,Guid>(It.IsAny<DepositWithdrawRequest>(), It.IsAny<HttpRequestMessage>()))
            .ReturnsAsync(new Guid());
        var sut = new TransactionsService(_accountsServiceMock.Object, null, _httpClientServiceMock.Object);

        //act
        var act = async () => await sut.AddWithdrawTransactionAsync(withdrawTransactionRequest);

        //assert
        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage(TransactionsServiceExceptions.BalanceNotEnough);
        _httpClientServiceMock.Verify(m => m.GetAsync<AccountBalanceResponse>(It.IsAny<string>()), Times.Once);
        _accountsServiceMock.Verify(m => m.GetAccountByIdAsync<AccountForTransactionResponse>(It.IsAny<Guid>()), Times.Never);
        _httpClientServiceMock.Verify(m => m.SendAsync<DepositWithdrawRequest,Guid>(It.IsAny<DepositWithdrawRequest>(), It.IsAny<HttpRequestMessage>()), Times.Never);
    }
    
    [Fact]
    public async Task AddTransferTransactionAsync_TransactionValidRequestAmountEqualBalanceAccountFromSent_GuidReceived()
    {
        //arrange
        var fixture = _customFixture.GetFixture();
        var transferTransactionRequest = fixture.Create<CrmTransferRequest>();
        var accountFrom = new AccountForTransactionResponse
        {
            LeadId = Guid.NewGuid(),
            Currency = Currency.Eur
        };
        var accountTo = new AccountForTransactionResponse
        {
            LeadId = accountFrom.LeadId,
            Currency = Currency.Rub
        };
        var accountResponse = fixture.Create<AccountBalanceResponse>();
        accountResponse.Balance = transferTransactionRequest.Amount;
        var lead = fixture.Create<LeadFullResponse>();
        _accountsServiceMock.Setup(x => x.GetAccountByIdAsync<AccountForTransactionResponse>(transferTransactionRequest.AccountFromId)).ReturnsAsync(accountFrom);
        _accountsServiceMock.Setup(x => x.GetAccountByIdAsync<AccountForTransactionResponse>(transferTransactionRequest.AccountToId)).ReturnsAsync(accountTo);
        _httpClientServiceMock.Setup(m => m.GetAsync<AccountBalanceResponse>(It.IsAny<string>()))
            .ReturnsAsync(accountResponse);
        _leadsServiceMock.Setup(x => x.GetLeadByIdAsync(It.IsAny<Guid>())).ReturnsAsync(lead);
        _httpClientServiceMock.Setup(m => m.SendAsync<TransferRequest,TransferGuidsResponse>(It.IsAny<TransferRequest>(), It.IsAny<HttpRequestMessage>()))
            .ReturnsAsync(new TransferGuidsResponse());
        var sut = new TransactionsService(_accountsServiceMock.Object, _leadsServiceMock.Object, _httpClientServiceMock.Object);

        //act
        await sut.AddTransferTransactionAsync(transferTransactionRequest);

        //assert
        _httpClientServiceMock.Verify(m => m.GetAsync<AccountBalanceResponse>(It.IsAny<string>()), Times.Once);
        _accountsServiceMock.Verify(m => m.GetAccountByIdAsync<AccountForTransactionResponse>(It.IsAny<Guid>()), Times.Exactly(2));
        _leadsServiceMock.Verify(x => x.GetLeadByIdAsync(It.IsAny<Guid>()), Times.Once);
        _httpClientServiceMock.Verify(m => m.SendAsync<TransferRequest,TransferGuidsResponse>(It.IsAny<TransferRequest>(), It.IsAny<HttpRequestMessage>()), Times.Once);
    }
    
    
    [Fact]
    public async Task AddTransferTransactionAsync_TransactionRequestAccountFromNotEqualAccountToSent_ValidationErrorReceived()
    {
        //arrange
        var fixture = _customFixture.GetFixture();
        var transferTransactionRequest = fixture.Create<CrmTransferRequest>();
        var accountFrom = new AccountForTransactionResponse
        {
            LeadId = Guid.NewGuid(),
            Currency = Currency.Eur
        };
        var accountTo = new AccountForTransactionResponse
        {
            LeadId = Guid.NewGuid(),
            Currency = Currency.Rub
        };
        var accountResponse = fixture.Create<AccountBalanceResponse>();
        accountResponse.Balance = transferTransactionRequest.Amount;
        var lead = fixture.Create<LeadFullResponse>();
        _accountsServiceMock.Setup(x => x.GetAccountByIdAsync<AccountForTransactionResponse>(transferTransactionRequest.AccountFromId)).ReturnsAsync(accountFrom);
        _accountsServiceMock.Setup(x => x.GetAccountByIdAsync<AccountForTransactionResponse>(transferTransactionRequest.AccountToId)).ReturnsAsync(accountTo);
        _httpClientServiceMock.Setup(m => m.GetAsync<AccountBalanceResponse>(It.IsAny<string>()))
            .ReturnsAsync(accountResponse);
        _leadsServiceMock.Setup(x => x.GetLeadByIdAsync(It.IsAny<Guid>())).ReturnsAsync(lead);
        _httpClientServiceMock.Setup(m => m.SendAsync<TransferRequest,TransferGuidsResponse>(It.IsAny<TransferRequest>(), It.IsAny<HttpRequestMessage>()))
            .ReturnsAsync(new TransferGuidsResponse());
        var sut = new TransactionsService(_accountsServiceMock.Object, _leadsServiceMock.Object, _httpClientServiceMock.Object);

        //act
        var act = async () => await sut.AddTransferTransactionAsync(transferTransactionRequest);

        //assert
        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage(TransactionsServiceExceptions.AccountNotYour);
        _httpClientServiceMock.Verify(m => m.GetAsync<AccountBalanceResponse>(It.IsAny<string>()), Times.Once);
        _accountsServiceMock.Verify(m => m.GetAccountByIdAsync<AccountForTransactionResponse>(It.IsAny<Guid>()), Times.Exactly(2));
        _leadsServiceMock.Verify(x => x.GetLeadByIdAsync(It.IsAny<Guid>()), Times.Never);
        _httpClientServiceMock.Verify(m => m.SendAsync<TransferRequest,TransferGuidsResponse>(It.IsAny<TransferRequest>(), It.IsAny<HttpRequestMessage>()), Times.Never);
    }
    
    [Fact]
    public async Task AddTransferTransactionAsync_TransactionRequestAccountFromCurrencyEqualAccountToSent_ValidationErrorReceived()
    {
        //arrange
        var fixture = _customFixture.GetFixture();
        var transferTransactionRequest = fixture.Create<CrmTransferRequest>();
        var accountFrom = new AccountForTransactionResponse
        {
            LeadId = Guid.NewGuid(),
            Currency = Currency.Eur
        };
        var accountTo = new AccountForTransactionResponse
        {
            LeadId = accountFrom.LeadId,
            Currency = Currency.Eur
        };
        var accountResponse = fixture.Create<AccountBalanceResponse>();
        accountResponse.Balance = transferTransactionRequest.Amount;
        var lead = fixture.Create<LeadFullResponse>();
        _accountsServiceMock.Setup(x => x.GetAccountByIdAsync<AccountForTransactionResponse>(transferTransactionRequest.AccountFromId)).ReturnsAsync(accountFrom);
        _accountsServiceMock.Setup(x => x.GetAccountByIdAsync<AccountForTransactionResponse>(transferTransactionRequest.AccountToId)).ReturnsAsync(accountTo);
        _httpClientServiceMock.Setup(m => m.GetAsync<AccountBalanceResponse>(It.IsAny<string>()))
            .ReturnsAsync(accountResponse);
        _leadsServiceMock.Setup(x => x.GetLeadByIdAsync(It.IsAny<Guid>())).ReturnsAsync(lead);
        _httpClientServiceMock.Setup(m => m.SendAsync<TransferRequest,TransferGuidsResponse>(It.IsAny<TransferRequest>(), It.IsAny<HttpRequestMessage>()))
            .ReturnsAsync(new TransferGuidsResponse());
        var sut = new TransactionsService(_accountsServiceMock.Object, _leadsServiceMock.Object, _httpClientServiceMock.Object);

        //act
        var act = async () => await sut.AddTransferTransactionAsync(transferTransactionRequest);

        //assert
        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage(TransactionsServiceExceptions.AccountsCurrencyEqual);
        _httpClientServiceMock.Verify(m => m.GetAsync<AccountBalanceResponse>(It.IsAny<string>()), Times.Once);
        _accountsServiceMock.Verify(m => m.GetAccountByIdAsync<AccountForTransactionResponse>(It.IsAny<Guid>()), Times.Exactly(2));
        _leadsServiceMock.Verify(x => x.GetLeadByIdAsync(It.IsAny<Guid>()), Times.Never);
        _httpClientServiceMock.Verify(m => m.SendAsync<TransferRequest,TransferGuidsResponse>(It.IsAny<TransferRequest>(), It.IsAny<HttpRequestMessage>()), Times.Never);
    }
    
    [Fact]
    public async Task AddTransferTransactionAsync_TransactionLeadRegularAllowedCurrencyFromRequestSent_GuidReceived()
    {
        //arrange
        var fixture = _customFixture.GetFixture();
        var transferTransactionRequest = fixture.Create<CrmTransferRequest>();
        var accountFrom = new AccountForTransactionResponse
        {
            LeadId = Guid.NewGuid(),
            Currency = Currency.Eur
        };
        var accountTo = new AccountForTransactionResponse
        {
            LeadId = accountFrom.LeadId,
            Currency = Currency.Rub
        };
        var accountResponse = fixture.Create<AccountBalanceResponse>();
        accountResponse.Balance = transferTransactionRequest.Amount;
        var lead = fixture.Create<LeadFullResponse>();
        lead.Status = LeadStatus.Regular;
        _accountsServiceMock.Setup(x => x.GetAccountByIdAsync<AccountForTransactionResponse>(transferTransactionRequest.AccountFromId)).ReturnsAsync(accountFrom);
        _accountsServiceMock.Setup(x => x.GetAccountByIdAsync<AccountForTransactionResponse>(transferTransactionRequest.AccountToId)).ReturnsAsync(accountTo);
        _httpClientServiceMock.Setup(m => m.GetAsync<AccountBalanceResponse>(It.IsAny<string>()))
            .ReturnsAsync(accountResponse);
        _leadsServiceMock.Setup(x => x.GetLeadByIdAsync(It.IsAny<Guid>())).ReturnsAsync(lead);
        _httpClientServiceMock.Setup(m => m.SendAsync<TransferRequest,TransferGuidsResponse>(It.IsAny<TransferRequest>(), It.IsAny<HttpRequestMessage>()))
            .ReturnsAsync(new TransferGuidsResponse());
        var sut = new TransactionsService(_accountsServiceMock.Object, _leadsServiceMock.Object, _httpClientServiceMock.Object);

        //act
        await sut.AddTransferTransactionAsync(transferTransactionRequest);

        //assert
        _httpClientServiceMock.Verify(m => m.GetAsync<AccountBalanceResponse>(It.IsAny<string>()), Times.Once);
        _accountsServiceMock.Verify(m => m.GetAccountByIdAsync<AccountForTransactionResponse>(It.IsAny<Guid>()), Times.Exactly(2));
        _leadsServiceMock.Verify(x => x.GetLeadByIdAsync(It.IsAny<Guid>()), Times.Once);
        _httpClientServiceMock.Verify(m => m.SendAsync<TransferRequest,TransferGuidsResponse>(It.IsAny<TransferRequest>(), It.IsAny<HttpRequestMessage>()), Times.Once);
    }
    
    [Fact]
    public async Task AddTransferTransactionAsync_TransactionLeadRegularUnallowedCurrencyFromToRubRequestSent_GuidReceived()
    {
        //arrange
        var fixture = _customFixture.GetFixture();
        var transferTransactionRequest = fixture.Create<CrmTransferRequest>();
        var accountFrom = new AccountForTransactionResponse
        {
            LeadId = Guid.NewGuid(),
            Currency = Currency.Jpy
        };
        var accountTo = new AccountForTransactionResponse
        {
            LeadId = accountFrom.LeadId,
            Currency = Currency.Rub
        };
        var accountResponse = fixture.Create<AccountBalanceResponse>();
        accountResponse.Balance = transferTransactionRequest.Amount;
        var lead = fixture.Create<LeadFullResponse>();
        lead.Status = LeadStatus.Regular;
        _accountsServiceMock.Setup(x => x.GetAccountByIdAsync<AccountForTransactionResponse>(transferTransactionRequest.AccountFromId)).ReturnsAsync(accountFrom);
        _accountsServiceMock.Setup(x => x.GetAccountByIdAsync<AccountForTransactionResponse>(transferTransactionRequest.AccountToId)).ReturnsAsync(accountTo);
        _httpClientServiceMock.Setup(m => m.GetAsync<AccountBalanceResponse>(It.IsAny<string>()))
            .ReturnsAsync(accountResponse);
        _leadsServiceMock.Setup(x => x.GetLeadByIdAsync(It.IsAny<Guid>())).ReturnsAsync(lead);
        _httpClientServiceMock.Setup(m => m.SendAsync<TransferRequest,TransferGuidsResponse>(It.IsAny<TransferRequest>(), It.IsAny<HttpRequestMessage>()))
            .ReturnsAsync(new TransferGuidsResponse());
        var sut = new TransactionsService(_accountsServiceMock.Object, _leadsServiceMock.Object, _httpClientServiceMock.Object);

        //act
        await sut.AddTransferTransactionAsync(transferTransactionRequest);

        //assert
        _httpClientServiceMock.Verify(m => m.GetAsync<AccountBalanceResponse>(It.IsAny<string>()), Times.Once);
        _accountsServiceMock.Verify(m => m.GetAccountByIdAsync<AccountForTransactionResponse>(It.IsAny<Guid>()), Times.Exactly(2));
        _leadsServiceMock.Verify(x => x.GetLeadByIdAsync(It.IsAny<Guid>()), Times.Once);
        _httpClientServiceMock.Verify(m => m.SendAsync<TransferRequest,TransferGuidsResponse>(It.IsAny<TransferRequest>(), It.IsAny<HttpRequestMessage>()), Times.Once);
    }
    
    [Fact]
    public async Task AddTransferTransactionAsync_TransactionLeadRegularUnallowedCurrencyFromToUnallowedCurrencyRequestSent_ValidationErrorReceived()
    {
        //arrange
        var (_, allowedCurrencyNames) = AllowedCurrencies.GetAllowedCurrenciesForRegularLead();
        var fixture = _customFixture.GetFixture();
        var transferTransactionRequest = fixture.Create<CrmTransferRequest>();
        var accountFrom = new AccountForTransactionResponse
        {
            LeadId = Guid.NewGuid(),
            Currency = Currency.Jpy
        };
        var accountTo = new AccountForTransactionResponse
        {
            LeadId = accountFrom.LeadId,
            Currency = Currency.Cny
        };
        var accountResponse = fixture.Create<AccountBalanceResponse>();
        accountResponse.Balance = transferTransactionRequest.Amount;
        var lead = fixture.Create<LeadFullResponse>();
        lead.Status = LeadStatus.Regular;
        _accountsServiceMock.Setup(x => x.GetAccountByIdAsync<AccountForTransactionResponse>(transferTransactionRequest.AccountFromId)).ReturnsAsync(accountFrom);
        _accountsServiceMock.Setup(x => x.GetAccountByIdAsync<AccountForTransactionResponse>(transferTransactionRequest.AccountToId)).ReturnsAsync(accountTo);
        _httpClientServiceMock.Setup(m => m.GetAsync<AccountBalanceResponse>(It.IsAny<string>()))
            .ReturnsAsync(accountResponse);
        _leadsServiceMock.Setup(x => x.GetLeadByIdAsync(It.IsAny<Guid>())).ReturnsAsync(lead);
        _httpClientServiceMock.Setup(m => m.SendAsync<TransferRequest,TransferGuidsResponse>(It.IsAny<TransferRequest>(), It.IsAny<HttpRequestMessage>()))
            .ReturnsAsync(new TransferGuidsResponse());
        var sut = new TransactionsService(_accountsServiceMock.Object, _leadsServiceMock.Object, _httpClientServiceMock.Object);

        //act
        var act = async () => await sut.AddTransferTransactionAsync(transferTransactionRequest);

        //assert
        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage(string.Format(TransactionsServiceExceptions.CurrencyForRegularLead,
                string.Join(",", allowedCurrencyNames)));
        _httpClientServiceMock.Verify(m => m.GetAsync<AccountBalanceResponse>(It.IsAny<string>()), Times.Once);
        _accountsServiceMock.Verify(m => m.GetAccountByIdAsync<AccountForTransactionResponse>(It.IsAny<Guid>()), Times.Exactly(2));
        _leadsServiceMock.Verify(x => x.GetLeadByIdAsync(It.IsAny<Guid>()), Times.Once);
        _httpClientServiceMock.Verify(m => m.SendAsync<TransferRequest,TransferGuidsResponse>(It.IsAny<TransferRequest>(), It.IsAny<HttpRequestMessage>()), Times.Never);
    }
    
    [Fact]
    public async Task AddTransferTransactionAsync_TransactionLeadRegularAllowedCurrencyFromToUnallowedCurrencyRequestSent_ValidationErrorReceived()
    {
        //arrange
        var (_, allowedCurrencyNames) = AllowedCurrencies.GetAllowedCurrenciesForRegularLead();
        var fixture = _customFixture.GetFixture();
        var transferTransactionRequest = fixture.Create<CrmTransferRequest>();
        var accountFrom = new AccountForTransactionResponse
        {
            LeadId = Guid.NewGuid(),
            Currency = Currency.Rub
        };
        var accountTo = new AccountForTransactionResponse
        {
            LeadId = accountFrom.LeadId,
            Currency = Currency.Cny
        };
        var accountResponse = fixture.Create<AccountBalanceResponse>();
        accountResponse.Balance = transferTransactionRequest.Amount;
        var lead = fixture.Create<LeadFullResponse>();
        lead.Status = LeadStatus.Regular;
        _accountsServiceMock.Setup(x => x.GetAccountByIdAsync<AccountForTransactionResponse>(transferTransactionRequest.AccountFromId)).ReturnsAsync(accountFrom);
        _accountsServiceMock.Setup(x => x.GetAccountByIdAsync<AccountForTransactionResponse>(transferTransactionRequest.AccountToId)).ReturnsAsync(accountTo);
        _httpClientServiceMock.Setup(m => m.GetAsync<AccountBalanceResponse>(It.IsAny<string>()))
            .ReturnsAsync(accountResponse);
        _leadsServiceMock.Setup(x => x.GetLeadByIdAsync(It.IsAny<Guid>())).ReturnsAsync(lead);
        _httpClientServiceMock.Setup(m => m.SendAsync<TransferRequest,TransferGuidsResponse>(It.IsAny<TransferRequest>(), It.IsAny<HttpRequestMessage>()))
            .ReturnsAsync(new TransferGuidsResponse());
        var sut = new TransactionsService(_accountsServiceMock.Object, _leadsServiceMock.Object, _httpClientServiceMock.Object);

        //act
        var act = async () => await sut.AddTransferTransactionAsync(transferTransactionRequest);

        //assert
        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage(string.Format(TransactionsServiceExceptions.CurrencyForRegularLead,
                string.Join(",", allowedCurrencyNames)));
        _httpClientServiceMock.Verify(m => m.GetAsync<AccountBalanceResponse>(It.IsAny<string>()), Times.Once);
        _accountsServiceMock.Verify(m => m.GetAccountByIdAsync<AccountForTransactionResponse>(It.IsAny<Guid>()), Times.Exactly(2));
        _leadsServiceMock.Verify(x => x.GetLeadByIdAsync(It.IsAny<Guid>()), Times.Once);
        _httpClientServiceMock.Verify(m => m.SendAsync<TransferRequest,TransferGuidsResponse>(It.IsAny<TransferRequest>(), It.IsAny<HttpRequestMessage>()), Times.Never);
    }
    
        
    [Fact]
    public async Task AddTransferTransactionAsync_TransactionLeadVipUnallowedCurrencyFromToUnallowedCurrencyRequestSent_GuidReceived()
    {
        //arrange
        var fixture = _customFixture.GetFixture();
        var transferTransactionRequest = fixture.Create<CrmTransferRequest>();
        var accountFrom = new AccountForTransactionResponse
        {
            LeadId = Guid.NewGuid(),
            Currency = Currency.Jpy
        };
        var accountTo = new AccountForTransactionResponse
        {
            LeadId = accountFrom.LeadId,
            Currency = Currency.Cny
        };
        var accountResponse = fixture.Create<AccountBalanceResponse>();
        accountResponse.Balance = transferTransactionRequest.Amount;
        var lead = fixture.Create<LeadFullResponse>();
        lead.Status = LeadStatus.Vip;
        _accountsServiceMock.Setup(x => x.GetAccountByIdAsync<AccountForTransactionResponse>(transferTransactionRequest.AccountFromId)).ReturnsAsync(accountFrom);
        _accountsServiceMock.Setup(x => x.GetAccountByIdAsync<AccountForTransactionResponse>(transferTransactionRequest.AccountToId)).ReturnsAsync(accountTo);
        _httpClientServiceMock.Setup(m => m.GetAsync<AccountBalanceResponse>(It.IsAny<string>()))
            .ReturnsAsync(accountResponse);
        _leadsServiceMock.Setup(x => x.GetLeadByIdAsync(It.IsAny<Guid>())).ReturnsAsync(lead);
        _httpClientServiceMock.Setup(m => m.SendAsync<TransferRequest,TransferGuidsResponse>(It.IsAny<TransferRequest>(), It.IsAny<HttpRequestMessage>()))
            .ReturnsAsync(new TransferGuidsResponse());
        var sut = new TransactionsService(_accountsServiceMock.Object, _leadsServiceMock.Object, _httpClientServiceMock.Object);

        //act
        await sut.AddTransferTransactionAsync(transferTransactionRequest);

        //assert
        _httpClientServiceMock.Verify(m => m.GetAsync<AccountBalanceResponse>(It.IsAny<string>()), Times.Once);
        _accountsServiceMock.Verify(m => m.GetAccountByIdAsync<AccountForTransactionResponse>(It.IsAny<Guid>()), Times.Exactly(2));
        _leadsServiceMock.Verify(x => x.GetLeadByIdAsync(It.IsAny<Guid>()), Times.Once);
        _httpClientServiceMock.Verify(m => m.SendAsync<TransferRequest,TransferGuidsResponse>(It.IsAny<TransferRequest>(), It.IsAny<HttpRequestMessage>()), Times.Once);
    }
    
    [Fact]
    public async Task AddTransferTransactionAsync_TransactionLeadVipAllowedCurrencyFromToUnallowedCurrencyRequestSent_GuidReceived()
    {
        //arrange
        var fixture = _customFixture.GetFixture();
        var transferTransactionRequest = fixture.Create<CrmTransferRequest>();
        var accountFrom = new AccountForTransactionResponse
        {
            LeadId = Guid.NewGuid(),
            Currency = Currency.Rub
        };
        var accountTo = new AccountForTransactionResponse
        {
            LeadId = accountFrom.LeadId,
            Currency = Currency.Cny
        };
        var accountResponse = fixture.Create<AccountBalanceResponse>();
        accountResponse.Balance = transferTransactionRequest.Amount;
        var lead = fixture.Create<LeadFullResponse>();
        lead.Status = LeadStatus.Vip;
        _accountsServiceMock.Setup(x => x.GetAccountByIdAsync<AccountForTransactionResponse>(transferTransactionRequest.AccountFromId)).ReturnsAsync(accountFrom);
        _accountsServiceMock.Setup(x => x.GetAccountByIdAsync<AccountForTransactionResponse>(transferTransactionRequest.AccountToId)).ReturnsAsync(accountTo);
        _httpClientServiceMock.Setup(m => m.GetAsync<AccountBalanceResponse>(It.IsAny<string>()))
            .ReturnsAsync(accountResponse);
        _leadsServiceMock.Setup(x => x.GetLeadByIdAsync(It.IsAny<Guid>())).ReturnsAsync(lead);
        _httpClientServiceMock.Setup(m => m.SendAsync<TransferRequest,TransferGuidsResponse>(It.IsAny<TransferRequest>(), It.IsAny<HttpRequestMessage>()))
            .ReturnsAsync(new TransferGuidsResponse());
        var sut = new TransactionsService(_accountsServiceMock.Object, _leadsServiceMock.Object, _httpClientServiceMock.Object);

        //act
        await sut.AddTransferTransactionAsync(transferTransactionRequest);

        //assert
        _httpClientServiceMock.Verify(m => m.GetAsync<AccountBalanceResponse>(It.IsAny<string>()), Times.Once);
        _accountsServiceMock.Verify(m => m.GetAccountByIdAsync<AccountForTransactionResponse>(It.IsAny<Guid>()), Times.Exactly(2));
        _leadsServiceMock.Verify(x => x.GetLeadByIdAsync(It.IsAny<Guid>()), Times.Once);
        _httpClientServiceMock.Verify(m => m.SendAsync<TransferRequest,TransferGuidsResponse>(It.IsAny<TransferRequest>(), It.IsAny<HttpRequestMessage>()), Times.Once);
    }
    
    [Fact]
    public async Task GetTransactionsByAccountIdAsync_GuidSent_TransactionsResponseReceived()
    {
        //arrange
        var fixture = _customFixture.GetFixture();
        var transactionsResponse = fixture.Create<List<TransactionResponse>>();
        _httpClientServiceMock.Setup(m => m.GetAsync<List<TransactionResponse>>(It.IsAny<string>()))
            .ReturnsAsync(transactionsResponse);
        _accountsServiceMock.Setup(x => x.GetAccountByIdAsync<AccountForTransactionResponse>(It.IsAny<Guid>())).ReturnsAsync(new AccountForTransactionResponse());
        var sut = new TransactionsService(_accountsServiceMock.Object, null, _httpClientServiceMock.Object);

        //act
        await sut.GetTransactionsByAccountIdAsync(new Guid());

        //assert
        _httpClientServiceMock.Verify(m => m.GetAsync<List<TransactionResponse>>(It.IsAny<string>()), Times.Once);
        _accountsServiceMock.Verify(m => m.GetAccountByIdAsync<AccountForTransactionResponse>(It.IsAny<Guid>()), Times.Exactly(transactionsResponse.Count));
    }
    
    [Fact]
    public async Task GetBalanceByAccountIdAsync_GuidSent_BalanceResponseReceived()
    {
        //arrange
        var fixture = _customFixture.GetFixture();
        var transactionsResponse = fixture.Create<AccountBalanceResponse>();
        _httpClientServiceMock.Setup(m => m.GetAsync<AccountBalanceResponse>(It.IsAny<string>()))
            .ReturnsAsync(transactionsResponse);
        _accountsServiceMock.Setup(x => x.GetAccountByIdAsync<AccountForTransactionResponse>(It.IsAny<Guid>())).ReturnsAsync(new AccountForTransactionResponse());
        var sut = new TransactionsService(_accountsServiceMock.Object, null, _httpClientServiceMock.Object);

        //act
        await sut.GetBalanceByAccountIdAsync(new Guid());

        //assert
        _httpClientServiceMock.Verify(m => m.GetAsync<AccountBalanceResponse>(It.IsAny<string>()), Times.Once);
        _accountsServiceMock.Verify(m => m.GetAccountByIdAsync<AccountForTransactionResponse>(It.IsAny<Guid>()), Times.Once);
    }
}