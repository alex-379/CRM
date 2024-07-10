using AutoFixture;
using AutoMapper;
using CRM.Business.Interfaces;
using CRM.Business.Models.Accounts;
using CRM.Business.Models.Accounts.Requests;
using CRM.Business.Models.Accounts.Responses;
using CRM.Business.Models.Leads.Responses;
using CRM.Business.Services;
using CRM.Business.Services.Constants.Exceptions;
using CRM.Core;
using CRM.Core.Dtos;
using CRM.Core.Enums;
using CRM.Core.Exceptions;
using CRM.Core.Fixture;
using CRM.DataLayer.Interfaces;
using FluentAssertions;
using Moq;

namespace CRM.Business.Tests.Services;

public class AccountsServiceTest
{
    private readonly Mock<IAccountsRepository> _accountsRepositoryMock;
    private readonly Mock<ILeadsService> _leadsServiceMock;
    private readonly MessagesServiceTest _messagesService;
    private readonly IMapper _mapper;
    private readonly CustomFixture _customFixture;

    public AccountsServiceTest()
    {
        _accountsRepositoryMock = new Mock<IAccountsRepository>();
        _leadsServiceMock = new Mock<ILeadsService>();
        _messagesService = new MessagesServiceTest();
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new AccountsMappingProfile());
        });

        _mapper = new Mapper(config);
        _customFixture = new CustomFixture();
    }

    [Fact]
    public async Task AddAccountAsync_GuidLeadAndRegistrationAccountForRegularLeadWithValidCurrencyRequestSent_GuidReceived()
    {
        //arrange
        var fixture = _customFixture.GetFixture();
        var registrationAccountRequest = fixture.Create<RegisterAccountRequest>();
        registrationAccountRequest.Currency = Currency.Usd;
        var leadId = fixture.Create<Guid>();
        var expectedGuid = fixture.Create<Guid>();
        var lead = fixture.Create<LeadDto>();
        lead.Status = LeadStatus.Regular;
        lead.Accounts = [ new AccountDto {Currency = Currency.Rub}];
        var leadResponse = SetupLeadResponse(lead);
        _leadsServiceMock.Setup(x => x.GetLeadByIdAsync(It.IsAny<Guid>())).ReturnsAsync(leadResponse);
        _accountsRepositoryMock.Setup(x => x.AddAccountAsync(It.IsAny<AccountDto>())).ReturnsAsync(expectedGuid);
        var sut = new AccountsService(_accountsRepositoryMock.Object, _leadsServiceMock.Object, _messagesService, _mapper);

        //act
        var actual = await sut.AddAccountAsync(leadId, registrationAccountRequest);

        //assert
        Assert.Equal(expectedGuid, actual);
        _leadsServiceMock.Verify(m => m.GetLeadByIdAsync(It.IsAny<Guid>()), Times.Once);
        _accountsRepositoryMock.Verify(m => m.AddAccountAsync(It.IsAny<AccountDto>()), Times.Once);
    }
    
    
    [Fact]
    public async Task AddAccountAsync_GuidLeadAndRegistrationAccountForVipLeadWithValidCurrencyRequestSent_GuidReceived()
    {
        //arrange
        var fixture = _customFixture.GetFixture();
        var registrationAccountRequest = fixture.Create<RegisterAccountRequest>();
        registrationAccountRequest.Currency = Currency.Jpy;
        var leadId = fixture.Create<Guid>();
        var expectedGuid = fixture.Create<Guid>();
        var lead = fixture.Create<LeadDto>();
        lead.Status = LeadStatus.Vip;
        lead.Accounts = [ new AccountDto {Currency = Currency.Rub}];
        var leadResponse = SetupLeadResponse(lead);
        _leadsServiceMock.Setup(x => x.GetLeadByIdAsync(It.IsAny<Guid>())).ReturnsAsync(leadResponse);
        _accountsRepositoryMock.Setup(x => x.AddAccountAsync(It.IsAny<AccountDto>())).ReturnsAsync(expectedGuid);
        var sut = new AccountsService(_accountsRepositoryMock.Object, _leadsServiceMock.Object, _messagesService, _mapper);

        //act
        var actual = await sut.AddAccountAsync(leadId, registrationAccountRequest);

        //assert
        Assert.Equal(expectedGuid, actual);
        _leadsServiceMock.Verify(m => m.GetLeadByIdAsync(It.IsAny<Guid>()), Times.Once);
        _accountsRepositoryMock.Verify(m => m.AddAccountAsync(It.IsAny<AccountDto>()), Times.Once);
    }
    
    [Fact]
    public async Task AddAccountAsync_GuidLeadAndRegistrationAccountWithSameCurrencyRequestSent_ValidationErrorReceived()
    {
        //arrange
        var fixture = _customFixture.GetFixture();
        var registrationAccountRequest = fixture.Create<RegisterAccountRequest>();
        registrationAccountRequest.Currency = Currency.Rub;
        var leadId = fixture.Create<Guid>();
        var lead = fixture.Create<LeadDto>();
        lead.Accounts = [ new AccountDto {Currency = Currency.Rub}];
        var leadResponse = SetupLeadResponse(lead);
        _leadsServiceMock.Setup(x => x.GetLeadByIdAsync(It.IsAny<Guid>())).ReturnsAsync(leadResponse);
        _accountsRepositoryMock.Setup(x => x.AddAccountAsync(It.IsAny<AccountDto>())).ReturnsAsync(It.IsAny<Guid>);
        var sut = new AccountsService(_accountsRepositoryMock.Object, _leadsServiceMock.Object, null, _mapper);

        //act
        var act = async () => await sut.AddAccountAsync(leadId, registrationAccountRequest);

        //assert
        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage(string.Format(AccountsServiceExceptions.AccountCurrencyContains));
        _leadsServiceMock.Verify(m => m.GetLeadByIdAsync(It.IsAny<Guid>()), Times.Once);
        _accountsRepositoryMock.Verify(m => m.AddAccountAsync(It.IsAny<AccountDto>()), Times.Never);
    }

    [Fact]
    public async Task AddAccountAsync_GuidLeadAndRegistrationAccountForRegularLeadWithUnallowedCurrencyRequestSent_ValidationErrorReceived()
    {
        //arrange
        var (_, allowedCurrencyNames) = AllowedCurrencies.GetAllowedCurrenciesForRegularLead();
        var fixture = _customFixture.GetFixture();
        var registrationAccountRequest = fixture.Create<RegisterAccountRequest>();
        registrationAccountRequest.Currency = Currency.Jpy;
        var leadId = fixture.Create<Guid>();
        var lead = fixture.Create<LeadDto>();
        lead.Status = LeadStatus.Regular;
        lead.Accounts = [ new AccountDto {Currency = Currency.Rub}];
        var leadResponse = SetupLeadResponse(lead);
        _leadsServiceMock.Setup(x => x.GetLeadByIdAsync(It.IsAny<Guid>())).ReturnsAsync(leadResponse);
        _accountsRepositoryMock.Setup(x => x.AddAccountAsync(It.IsAny<AccountDto>())).ReturnsAsync(It.IsAny<Guid>);
        var sut = new AccountsService(_accountsRepositoryMock.Object, _leadsServiceMock.Object, null, _mapper);

        //act
        var act = async () => await sut.AddAccountAsync(leadId, registrationAccountRequest);

        //assert
        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage(string.Format(AccountsServiceExceptions.CurrencyForRegularLead, string.Join(",", allowedCurrencyNames)));
        _leadsServiceMock.Verify(m => m.GetLeadByIdAsync(It.IsAny<Guid>()), Times.Once);
        _accountsRepositoryMock.Verify(m => m.AddAccountAsync(It.IsAny<AccountDto>()), Times.Never);
    }
    
    [Fact]
    public async Task AddAccountAsync_GuidLeadAndRegistrationAccountForLeadWithUnknownCurrencyRequestSent_ValidationErrorReceived()
    {
        //arrange
        var fixture = _customFixture.GetFixture();
        var registrationAccountRequest = fixture.Create<RegisterAccountRequest>();
        registrationAccountRequest.Currency = Currency.Unknown;
        var leadId = fixture.Create<Guid>();
        var expectedGuid = fixture.Create<Guid>();
        var lead = fixture.Create<LeadDto>();
        lead.Accounts = [ new AccountDto {Currency = Currency.Rub}];
        var leadResponse = SetupLeadResponse(lead);
        _leadsServiceMock.Setup(x => x.GetLeadByIdAsync(It.IsAny<Guid>())).ReturnsAsync(leadResponse);
        _accountsRepositoryMock.Setup(x => x.AddAccountAsync(It.IsAny<AccountDto>())).ReturnsAsync(expectedGuid);
        var sut = new AccountsService(_accountsRepositoryMock.Object, _leadsServiceMock.Object, null, _mapper);

        //act
        var act = async () => await sut.AddAccountAsync(leadId, registrationAccountRequest);

        //assert
        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage(string.Format(AccountsServiceExceptions.CurrencyIsUnknown));
        _leadsServiceMock.Verify(m => m.GetLeadByIdAsync(It.IsAny<Guid>()), Times.Once);
        _accountsRepositoryMock.Verify(m => m.AddAccountAsync(It.IsAny<AccountDto>()), Times.Never);
    }

    [Fact]
    public async Task AddAccountAsyncNoLead_EmptyGuidAndRegistrationAccountRequestSent_LeadNotFoundErrorReceived()
    {
        //arrange
        var fixture = _customFixture.GetFixture();
        var leadId = Guid.Empty;
        var registrationAccountRequest = fixture.Create<RegisterAccountRequest>();
        var expectedGuid = Guid.NewGuid();
        _accountsRepositoryMock.Setup(x => x.AddAccountAsync(It.IsAny<AccountDto>())).ReturnsAsync(expectedGuid);
        var sut = new AccountsService(_accountsRepositoryMock.Object, _leadsServiceMock.Object, null, _mapper);

        //act
        var act = async () => await sut.AddAccountAsync(leadId, registrationAccountRequest);

        //assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage(string.Format(LeadsServiceExceptions.NotFoundException, leadId));
        _leadsServiceMock.Verify(m => m.GetLeadByIdAsync(It.IsAny<Guid>()), Times.Once);
        _accountsRepositoryMock.Verify(m => m.AddAccountAsync(It.IsAny<AccountDto>()), Times.Never);
    }
    
    [Fact]
    public async Task GetAccountByIdAsync_GuidSent_AccountResponseReceived()
    {
        //arrange
        var fixture = _customFixture.GetFixture();
        var id = Guid.NewGuid();
        var expectedAccount = fixture.Create<AccountDto>();
        var expected = new AccountForTransactionResponse
        {
            Currency = expectedAccount.Currency,
            LeadId = expectedAccount.Lead.Id
        };

        _accountsRepositoryMock.Setup(x => x.GetAccountByIdAsync(id)).ReturnsAsync(expectedAccount);
        var sut = new AccountsService(_accountsRepositoryMock.Object, null, null, _mapper);

        //act
        var actual = await sut.GetAccountByIdAsync<AccountForTransactionResponse>(id);

        //assert
        actual.Should().BeEquivalentTo(expected);
        _accountsRepositoryMock.Verify(m => m.GetAccountByIdAsync(id), Times.Once);
    }

    [Fact]
    public void GetAccountByIdAsyncNoLead_EmptyGuidSent_AccountNotFoundErrorReceived()
    {
        //arrange
        var id = Guid.Empty;
        _accountsRepositoryMock.Setup(x => x.GetAccountByIdAsync(id)).ReturnsAsync((AccountDto)null);
        var sut = new AccountsService(_accountsRepositoryMock.Object, null, null, _mapper);
        
        //act
        var act = async () => await sut.GetAccountByIdAsync<AccountForTransactionResponse>(id);

        //assert
        act.Should().ThrowAsync<NotFoundException>()
            .WithMessage(string.Format(AccountsServiceExceptions.NotFoundException, id));
        _accountsRepositoryMock.Verify(m => m.GetAccountByIdAsync(id), Times.Once);
    }
    
    [Fact]
    public async Task UpdateAccountStatusAsync_GuidAndUpdateAccountStatusValidRequestSent_NoErrorsReceived()
    {
        //arrange
        var id = Guid.NewGuid();
        var updateAccountStatusRequest = new UpdateAccountStatusRequest
        {
            Status = AccountStatus.Blocked
        };
        var account = new AccountDto
        {
            Currency = Currency.Eur
        };
        _accountsRepositoryMock.Setup(x => x.GetAccountByIdAsync(id)).ReturnsAsync(account);
        var sut = new AccountsService(_accountsRepositoryMock.Object, null, _messagesService, null);

        //act
        await sut.UpdateAccountStatusAsync(id, updateAccountStatusRequest);

        //assert
        _accountsRepositoryMock.Verify(m => m.GetAccountByIdAsync(id), Times.Once);
        _accountsRepositoryMock.Verify(m => m.UpdateAccountAsync(It.IsAny<AccountDto>()), Times.Once);
    }
    
    [Fact]
    public async Task UpdateAccountStatusAsync_GuidAndUpdateAccountRubStatusBlockedRequestSent_ValidationErrorReceived()
    {
        //arrange
        var id = Guid.NewGuid();
        var updateAccountStatusRequest = new UpdateAccountStatusRequest
        {
            Status = AccountStatus.Blocked
        };
        var account = new AccountDto
        {
            Currency = Currency.Rub
        };
        _accountsRepositoryMock.Setup(x => x.GetAccountByIdAsync(id)).ReturnsAsync(account);
        var sut = new AccountsService(_accountsRepositoryMock.Object, null, null, null);

        //act
        var act = async () => await sut.UpdateAccountStatusAsync(id, updateAccountStatusRequest);

        //assert
        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage(AccountsServiceExceptions.AccountRubException);
        _accountsRepositoryMock.Verify(m => m.GetAccountByIdAsync(id), Times.Once);
        _accountsRepositoryMock.Verify(m => m.UpdateAccountAsync(It.IsAny<AccountDto>()), Times.Never);
    }
    
    [Fact]
    public async Task UpdateAccountStatusAsync_GuidAndUpdateAccountEqualStatusRequestSent_ValidationErrorReceived()
    {
        //arrange
        var fixture = _customFixture.GetFixture();
        var id = Guid.NewGuid();
        var updateAccountStatusRequest = fixture.Create<UpdateAccountStatusRequest>();
        var account = new AccountDto
        {
            Status = updateAccountStatusRequest.Status
        };
        _accountsRepositoryMock.Setup(x => x.GetAccountByIdAsync(id)).ReturnsAsync(account);
        var sut = new AccountsService(_accountsRepositoryMock.Object, null, null, null);

        //act
        var act = async () => await sut.UpdateAccountStatusAsync(id, updateAccountStatusRequest);

        //assert
        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage(AccountsServiceExceptions.AccountStatusEqual);
        _accountsRepositoryMock.Verify(m => m.GetAccountByIdAsync(id), Times.Once);
        _accountsRepositoryMock.Verify(m => m.UpdateAccountAsync(It.IsAny<AccountDto>()), Times.Never);
    }
    
    [Fact]
    public async Task UpdateAccountStatusAsync_GuidAndUpdateAccountStatusIsUnknownRequestSent_ValidationErrorReceived()
    {
        //arrange
        var id = Guid.NewGuid();
        var updateAccountStatusRequest = new UpdateAccountStatusRequest
        {
            Status = AccountStatus.Unknown
        };
        var account = new AccountDto
        {
            Status = AccountStatus.Active
        };
        _accountsRepositoryMock.Setup(x => x.GetAccountByIdAsync(id)).ReturnsAsync(account);
        var sut = new AccountsService(_accountsRepositoryMock.Object, null, null, null);

        //act
        var act = async () => await sut.UpdateAccountStatusAsync(id, updateAccountStatusRequest);

        //assert
        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage(AccountsServiceExceptions.AccountStatusIsUnknown);
        _accountsRepositoryMock.Verify(m => m.GetAccountByIdAsync(id), Times.Once);
        _accountsRepositoryMock.Verify(m => m.UpdateAccountAsync(It.IsAny<AccountDto>()), Times.Never);
    }
    
    [Fact]
    public async Task UpdateAccountStatusAsync_EmptyGuidAndUpdateAccountStatusRequestSent_AccountNotFoundErrorReceived()
    {
        //arrange
        var fixture = _customFixture.GetFixture();
        var id = Guid.Empty;
        var updateAccountStatusRequest = fixture.Create<UpdateAccountStatusRequest>();
        _accountsRepositoryMock.Setup(x => x.GetAccountByIdAsync(id)).ReturnsAsync((AccountDto)null);
        var sut = new AccountsService(_accountsRepositoryMock.Object, null, null, null);

        //act
        var act = async () => await sut.UpdateAccountStatusAsync(id, updateAccountStatusRequest);

        //assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage(string.Format(AccountsServiceExceptions.NotFoundException, id));
        _accountsRepositoryMock.Verify(m => m.GetAccountByIdAsync(id), Times.Once);
        _accountsRepositoryMock.Verify(m => m.UpdateAccountAsync(It.IsAny<AccountDto>()), Times.Never);
    }

    private static LeadFullResponse SetupLeadResponse(LeadDto lead)
    {
        var leadResponse = new LeadFullResponse
        {
            Id = lead.Id,
            Name = lead.Name,
            Mail = lead.Mail,
            Phone = lead.Phone,
            Address = lead.Address,
            BirthDate = lead.BirthDate,
            Status = lead.Status,
            Accounts = [new AccountResponse { Currency = Currency.Rub }],
        };

        return leadResponse;
    }
}
