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
    public void AddAccount_GuidLeadAndRegistrationAccountRequestSent_GuidReceived()
    {
        //arrange
        var leadId = Guid.NewGuid();
        var registrationAccountRequest = TestsData.GetFakeRegistrationAccountRequest();
        _leadsRepositoryMock.Setup(x => x.GetLeadById(leadId)).Returns(new LeadDto());
        var expectedGuid = Guid.NewGuid();
        _accountsRepositoryMock.Setup(x => x.AddAccount(It.IsAny<AccountDto>())).Returns(expectedGuid);
        var sut = new AccountsService(_accountsRepositoryMock.Object, _leadsRepositoryMock.Object, _mapper);

        //act
        var actual = sut.AddAccount(leadId, registrationAccountRequest);

        //assert
        Assert.Equal(expectedGuid, actual);
        _leadsRepositoryMock.Verify(m => m.GetLeadById(leadId), Times.Once);
        _accountsRepositoryMock.Verify(m => m.AddAccount(It.IsAny<AccountDto>()), Times.Once);
    }


    [Fact]
    public void AddAccountNoLead_EmptyGuidAndRegistrationAccountRequestSent_LeadNotFoundErrorReceived()
    {
        //arrange
        var leadId = Guid.Empty;
        var registrationAccountRequest = TestsData.GetFakeRegistrationAccountRequest();
        _leadsRepositoryMock.Setup(x => x.GetLeadById(leadId)).Returns((LeadDto)null);
        var expectedGuid = Guid.NewGuid();
        _accountsRepositoryMock.Setup(x => x.AddAccount(It.IsAny<AccountDto>())).Returns(expectedGuid);
        var sut = new AccountsService(_accountsRepositoryMock.Object, _leadsRepositoryMock.Object, _mapper);

        //act
        Action act = () => sut.AddAccount(leadId, registrationAccountRequest);

        //assert
        act.Should().Throw<NotFoundException>()
            .WithMessage(string.Format(LeadsServiceExceptions.NotFoundException, leadId));
        _leadsRepositoryMock.Verify(m => m.GetLeadById(leadId), Times.Once);
        _accountsRepositoryMock.Verify(m => m.AddAccount(It.IsAny<AccountDto>()), Times.Never);
    }

    [Fact]
    public void UpdateAccountStatus_GuidAndUpdateAccountStatusRequestSent_NoErrorsReceived()
    {
        //arrange
        var id = Guid.NewGuid();
        var updateAccountStatusRequest = TestsData.GetFakeUpdateAccountStatusRequest();
        _accountsRepositoryMock.Setup(x => x.GetAccountById(id)).Returns(new AccountDto());
        var sut = new AccountsService(_accountsRepositoryMock.Object, null, null);

        //act
        sut.UpdateAccountStatus(id, updateAccountStatusRequest);

        //assert
        _accountsRepositoryMock.Verify(m => m.GetAccountById(id), Times.Once);
        _accountsRepositoryMock.Verify(m => m.UpdateAccount(It.IsAny<AccountDto>()), Times.Once);
    }

    [Fact]
    public void UpdateAccountStatus_EmptyGuidAndUpdateAccountStatusRequestSent_AccountNotFoundErrorReceived()
    {
        //arrange
        var id = Guid.Empty;
        var updateAccountStatusRequest = TestsData.GetFakeUpdateAccountStatusRequest();
        _accountsRepositoryMock.Setup(x => x.GetAccountById(id)).Returns((AccountDto)null);
        var sut = new AccountsService(_accountsRepositoryMock.Object, null, null);

        //act
        var act = () => sut.UpdateAccountStatus(id, updateAccountStatusRequest);

        //assert
        act.Should().Throw<NotFoundException>()
            .WithMessage(string.Format(AccountsServiceExceptions.NotFoundException, id));
        _accountsRepositoryMock.Verify(m => m.GetAccountById(id), Times.Once);
        _accountsRepositoryMock.Verify(m => m.UpdateAccount(It.IsAny<AccountDto>()), Times.Never);
    }
}
