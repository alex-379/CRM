using AutoMapper;
using CRM.Business.Models.Accounts;
using CRM.Business.Services;
using CRM.Business.Services.Constants.Exceptions;
using CRM.Core.Dtos;
using CRM.Core.Exceptions;
using CRM.DataLayer.Interfaces;
using FluentAssertions;
using Moq;

namespace CRM.Business.Tests.Services;

public class AccountsServiceTest
{
    private readonly Mock<IAccountsRepository> _accountsRepositoryMock;
    private readonly Mock<ILeadsRepository> _leadsRepositoryMock;
    private readonly IMapper _mapper;

    public AccountsServiceTest()
    {
        _accountsRepositoryMock = new Mock<IAccountsRepository>();
        _leadsRepositoryMock = new Mock<ILeadsRepository>();
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new AccountsMappingProfile());
        });

        _mapper = new Mapper(config);
    }

    [Fact]
    public async Task AddAccountAsync_GuidLeadAndRegistrationAccountRequestSent_GuidReceived()
    {
        //arrange
        var leadId = Guid.NewGuid();
        var registrationAccountRequest = TestsData.GetFakeRegistrationAccountRequest();
        _leadsRepositoryMock.Setup(x => x.GetLeadByIdAsync(leadId)).ReturnsAsync(new LeadDto());
        var expectedGuid = Guid.NewGuid();
        _accountsRepositoryMock.Setup(x => x.AddAccountAsync(It.IsAny<AccountDto>())).ReturnsAsync(expectedGuid);
        var sut = new AccountsService(_accountsRepositoryMock.Object, _leadsRepositoryMock.Object, _mapper, null);

        //act
        var actual = await sut.AddAccountAsync(leadId, registrationAccountRequest);

        //assert
        Assert.Equal(expectedGuid, actual);
        _leadsRepositoryMock.Verify(m => m.GetLeadByIdAsync(leadId), Times.Once);
        _accountsRepositoryMock.Verify(m => m.AddAccountAsync(It.IsAny<AccountDto>()), Times.Once);
    }


    [Fact]
    public async Task AddAccountAsyncNoLead_EmptyGuidAndRegistrationAccountRequestSent_LeadNotFoundErrorReceived()
    {
        //arrange
        var leadId = Guid.Empty;
        var registrationAccountRequest = TestsData.GetFakeRegistrationAccountRequest();
        _leadsRepositoryMock.Setup(x => x.GetLeadByIdAsync(leadId)).ReturnsAsync((LeadDto)null);
        var expectedGuid = Guid.NewGuid();
        _accountsRepositoryMock.Setup(x => x.AddAccountAsync(It.IsAny<AccountDto>())).ReturnsAsync(expectedGuid);
        var sut = new AccountsService(_accountsRepositoryMock.Object, _leadsRepositoryMock.Object, _mapper, null);

        //act
        var act = async () => await sut.AddAccountAsync(leadId, registrationAccountRequest);

        //assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage(string.Format(LeadsServiceExceptions.NotFoundException, leadId));
        _leadsRepositoryMock.Verify(m => m.GetLeadByIdAsync(leadId), Times.Once);
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
