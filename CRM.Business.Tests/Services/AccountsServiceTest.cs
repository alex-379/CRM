using AutoFixture;
using AutoMapper;
using CRM.Business.Models.Accounts;
using CRM.Business.Models.Accounts.Requests;
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
    private readonly Mock<ILeadsRepository> _leadsRepositoryMock;
    private readonly MessagesServiceTest _messagesService;
    private readonly IMapper _mapper;
    private readonly CustomFixture _customFixture;

    public AccountsServiceTest()
    {
        _accountsRepositoryMock = new Mock<IAccountsRepository>();
        _leadsRepositoryMock = new Mock<ILeadsRepository>();
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
        lead.Accounts = [ new AccountDto(){Currency = Currency.Rub}];
        _leadsRepositoryMock.Setup(x => x.GetLeadByIdAsync(It.IsAny<Guid>())).ReturnsAsync(lead);
        _accountsRepositoryMock.Setup(x => x.AddAccountAsync(It.IsAny<AccountDto>())).ReturnsAsync(expectedGuid);
        var sut = new AccountsService(_accountsRepositoryMock.Object, _leadsRepositoryMock.Object, _messagesService, _mapper);

        //act
        var actual = await sut.AddAccountAsync(leadId, registrationAccountRequest);

        //assert
        Assert.Equal(expectedGuid, actual);
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
        lead.Accounts = [ new AccountDto(){Currency = Currency.Rub}];
        _leadsRepositoryMock.Setup(x => x.GetLeadByIdAsync(It.IsAny<Guid>())).ReturnsAsync(lead);
        _accountsRepositoryMock.Setup(x => x.AddAccountAsync(It.IsAny<AccountDto>())).ReturnsAsync(expectedGuid);
        var sut = new AccountsService(_accountsRepositoryMock.Object, _leadsRepositoryMock.Object, _messagesService, _mapper);

        //act
        var actual = await sut.AddAccountAsync(leadId, registrationAccountRequest);

        //assert
        Assert.Equal(expectedGuid, actual);
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
        lead.Accounts = [ new AccountDto(){Currency = Currency.Rub}];
        _leadsRepositoryMock.Setup(x => x.GetLeadByIdAsync(It.IsAny<Guid>())).ReturnsAsync(lead);
        var sut = new AccountsService(_accountsRepositoryMock.Object, _leadsRepositoryMock.Object, _messagesService, _mapper);

        //act
        var act = async () => await sut.AddAccountAsync(leadId, registrationAccountRequest);

        //assert
        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage(string.Format(AccountsServiceExceptions.AccountCurrencyContains));
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
        var expectedGuid = fixture.Create<Guid>();
        var lead = fixture.Create<LeadDto>();
        lead.Status = LeadStatus.Regular;
        lead.Accounts = [ new AccountDto(){Currency = Currency.Rub}];
        _leadsRepositoryMock.Setup(x => x.GetLeadByIdAsync(It.IsAny<Guid>())).ReturnsAsync(lead);
        _accountsRepositoryMock.Setup(x => x.AddAccountAsync(It.IsAny<AccountDto>())).ReturnsAsync(expectedGuid);
        var sut = new AccountsService(_accountsRepositoryMock.Object, _leadsRepositoryMock.Object, _messagesService, _mapper);

        //act
        var act = async () => await sut.AddAccountAsync(leadId, registrationAccountRequest);

        //assert
        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage(string.Format(AccountsServiceExceptions.CurrencyForRegularLead, string.Join(",", allowedCurrencyNames)));
        _accountsRepositoryMock.Verify(m => m.AddAccountAsync(It.IsAny<AccountDto>()), Times.Never);
    }

    [Fact]
    public async Task AddAccountAsyncNoLead_EmptyGuidAndRegistrationAccountRequestSent_LeadNotFoundErrorReceived()
    {
        //arrange
        var leadId = Guid.Empty;
        var registrationAccountRequest = TestsData.GetFakeRegistrationAccountRequest();
        var expectedGuid = Guid.NewGuid();
        _accountsRepositoryMock.Setup(x => x.AddAccountAsync(It.IsAny<AccountDto>())).ReturnsAsync(expectedGuid);
        var sut = new AccountsService(_accountsRepositoryMock.Object, null, null, _mapper);

        //act
        var act = async () => await sut.AddAccountAsync(leadId, registrationAccountRequest);

        //assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage(string.Format(LeadsServiceExceptions.NotFoundException, leadId));
        _accountsRepositoryMock.Verify(m => m.AddAccountAsync(It.IsAny<AccountDto>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAccountStatusAsync_GuidAndUpdateAccountStatusRequestSent_NoErrorsReceived()
    {
        //arrange
        var id = Guid.NewGuid();
        var updateAccountStatusRequest = TestsData.GetFakeUpdateAccountStatusRequest();
        _accountsRepositoryMock.Setup(x => x.GetAccountByIdAsync(id)).ReturnsAsync(new AccountDto());
        var sut = new AccountsService(_accountsRepositoryMock.Object, null, null, null);

        //act
        await sut.UpdateAccountStatusAsync(id, updateAccountStatusRequest);

        //assert
        _accountsRepositoryMock.Verify(m => m.GetAccountByIdAsync(id), Times.Once);
        _accountsRepositoryMock.Verify(m => m.UpdateAccountAsync(It.IsAny<AccountDto>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAccountStatusAsync_EmptyGuidAndUpdateAccountStatusRequestSent_AccountNotFoundErrorReceived()
    {
        //arrange
        var id = Guid.Empty;
        var updateAccountStatusRequest = TestsData.GetFakeUpdateAccountStatusRequest();
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
}
