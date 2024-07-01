using AutoMapper;
using CRM.Business.Configuration;
using CRM.Business.Models.Accounts;
using CRM.Business.Models.Leads;
using CRM.Business.Services;
using CRM.Business.Services.Constants.Exceptions;
using CRM.Core.Dtos;
using CRM.Core.Exceptions;
using CRM.DataLayer.Interfaces;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;

namespace CRM.Business.Tests.Services;

public class LeadsServiceTest
{
    private readonly Mock<ILeadsRepository> _leadsRepositoryMock;
    private readonly Mock<IAccountsRepository> _accountsRepositoryMock;
    private readonly Mock<ITransactionsManager> _transactionsManagerMock;
    private readonly IMapper _mapper;
    private readonly SecretSettings _secret;

    public LeadsServiceTest()
    {
        _leadsRepositoryMock = new Mock<ILeadsRepository>();
        _accountsRepositoryMock = new Mock<IAccountsRepository>();
        _transactionsManagerMock = new Mock<ITransactionsManager>();
        _secret = new SecretSettings();
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new LeadsMappingProfile());
            cfg.AddProfile(new AccountsMappingProfile());
        });

        _mapper = new Mapper(config);
    }

    [Fact]
    public async Task AddLeadAsync_RegistrationLeadRequestSent_GuidReceived()
    {
        //arrange
        var registrationLeadRequest = TestsData.GetFakeRegistrationLeadRequest();
        var expectedGuid = Guid.NewGuid();
        _leadsRepositoryMock.Setup(x => x.GetLeadByMailAsync(It.IsAny<string>())).ReturnsAsync((LeadDto)null);
        _leadsRepositoryMock.Setup(x => x.AddLeadAsync(It.IsAny<LeadDto>())).ReturnsAsync(expectedGuid);
        var sut = new LeadsService(_leadsRepositoryMock.Object, _accountsRepositoryMock.Object,
            _transactionsManagerMock.Object, null, _mapper, _secret, null, null, null);

        //act
        var actual = await sut.AddLeadAsync(registrationLeadRequest);

        //assert
        Assert.Equal(expectedGuid, actual.leadId);
        _leadsRepositoryMock.Verify(m => m.GetLeadByMailAsync(It.IsAny<string>()), Times.Once);
        _transactionsManagerMock.Verify(m => m.BeginTransactionAsync(), Times.Once);
        _leadsRepositoryMock.Verify(m => m.AddLeadAsync(It.IsAny<LeadDto>()), Times.Once);
        _accountsRepositoryMock.Verify(m => m.AddAccountAsync(It.IsAny<AccountDto>()), Times.Once);
        _transactionsManagerMock.Verify(m => m.CommitTransactionAsync(It.IsAny<IDbContextTransaction>()), Times.Once);
        _transactionsManagerMock.Verify(m => m.RollbackTransactionAsync(It.IsAny<IDbContextTransaction>(), It.IsAny<Exception>()), Times.Never);
    }
    
    [Fact]
    public async Task AddLeadAsync_RegistrationLeadRequestSent_ConflictErrorReceived()
    {
        //arrange
        var registrationLeadRequestWithDuplicateMail = TestsData.GetFakeRegistrationLeadRequest();
        _leadsRepositoryMock.Setup(x => x.GetLeadByMailAsync(registrationLeadRequestWithDuplicateMail.Mail)).ReturnsAsync(new LeadDto());
        _leadsRepositoryMock.Setup(x => x.AddLeadAsync(It.IsAny<LeadDto>())).ReturnsAsync(Guid.NewGuid());
        var sut = new LeadsService(_leadsRepositoryMock.Object, null, null, null, _mapper, null, null, null, null);
        
        //act
        var act = async () => await sut.AddLeadAsync(registrationLeadRequestWithDuplicateMail);
        
        //assert
        await act.Should().ThrowAsync<ConflictException>()
        .WithMessage(LeadsServiceExceptions.ConflictException);
        _leadsRepositoryMock.Verify(m => m.AddLeadAsync(It.IsAny<LeadDto>()), Times.Never);
        
    }

    [Fact]
    public async Task LoginLeadAsync_LoginLeadRequestIncorrectMailSent_LeadUnauthenticatedErrorReceived()
    {
        //arrange
        var loginLeadRequestIncorrectMail = TestsData.GetFakeLoginLeadRequest();
        _leadsRepositoryMock.Setup(x => x.GetLeadByMailAsync(loginLeadRequestIncorrectMail.Mail)).ReturnsAsync((LeadDto)null);
        var sut = new LeadsService(_leadsRepositoryMock.Object, null, null, null, _mapper, null, null, null, null);

        //act
        var act = async () => await sut.LoginLeadAsync(loginLeadRequestIncorrectMail);

        //assert
        await act.Should().ThrowAsync<UnauthenticatedException>();
        _leadsRepositoryMock.Verify(m => m.GetLeadByMailAsync(loginLeadRequestIncorrectMail.Mail), Times.Once);
        _leadsRepositoryMock.Verify(m => m.UpdateLeadAsync(It.IsAny<LeadDto>()), Times.Never);
    }

    [Fact]
    public async Task LoginLeadAsync_LoginLeadRequestIncorrectPasswordSent_LeadUnauthenticatedErrorReceived()
    {
        //arrange
        var loginLeadRequestIncorrectPassword = TestsData.GetFakeLoginLeadRequest();
        var lead = TestsData.GetFakeLeadDto();
        _leadsRepositoryMock.Setup(x => x.GetLeadByMailAsync(loginLeadRequestIncorrectPassword.Mail)).ReturnsAsync(lead);
        var sut = new LeadsService(_leadsRepositoryMock.Object, null, null, null, _mapper, _secret, null, null, null);

        //act
        var act = async () => await sut.LoginLeadAsync(loginLeadRequestIncorrectPassword);

        //assert
        await act.Should().ThrowAsync<UnauthenticatedException>();
        _leadsRepositoryMock.Verify(m => m.GetLeadByMailAsync(loginLeadRequestIncorrectPassword.Mail), Times.Once);
        _leadsRepositoryMock.Verify(m => m.UpdateLeadAsync(It.IsAny<LeadDto>()), Times.Never);
    }

    [Fact]
    public async Task GetLeadsAsync_Called_ListLeadResponseReceived()
    {
        //arrange
        var expected = TestsData.GetFakeListLeadResponse();
        var expectedLeads = TestsData.GetFakeListLeadDto();
        _leadsRepositoryMock.Setup(x => x.GetLeadsAsync()).ReturnsAsync(expectedLeads);
        var sut = new LeadsService(_leadsRepositoryMock.Object, null, null, null, _mapper, null, null, null, null);

        //act
        var actual = await sut.GetLeadsAsync();

        //assert
        actual.Should().BeEquivalentTo(expected);
        _leadsRepositoryMock.Verify(m => m.GetLeadsAsync(), Times.Once);
    }

    [Fact]
    public async Task GetLeadByIdAsync_GuidSent_LeadResponseReceived()
    {
        //arrange
        var expected = TestsData.GetFakeLeadFullResponse();
        var expectedLead = TestsData.GetFakeLeadDto();
        var id = Guid.NewGuid();
        _leadsRepositoryMock.Setup(x => x.GetLeadByIdAsync(id)).ReturnsAsync(expectedLead);
        var sut = new LeadsService(_leadsRepositoryMock.Object, null, null, null, _mapper, null, null, null, null);

        //act
        var actual = await sut.GetLeadByIdAsync(id);

        //assert
        actual.Should().BeEquivalentTo(expected);
        _leadsRepositoryMock.Verify(m => m.GetLeadByIdAsync(id), Times.Once);
    }

    [Fact]
    public void GetLeadByIdAsyncNoLead_EmptyGuidSent_LeadNotFoundErrorReceived()
    {
        //arrange
        var id = Guid.Empty;
        _leadsRepositoryMock.Setup(x => x.GetLeadByIdAsync(id)).ReturnsAsync((LeadDto)null);
        var sut = new LeadsService(_leadsRepositoryMock.Object, null, null, null, null, null, null, null, null);

        //act
        var act = async () => await sut.GetLeadByIdAsync(id);

        //assert
        act.Should().ThrowAsync<NotFoundException>()
            .WithMessage(string.Format(LeadsServiceExceptions.NotFoundException, id));
        _leadsRepositoryMock.Verify(m => m.GetLeadByIdAsync(id), Times.Once);
    }

    [Fact]
    public async Task UpdateLeadAsync_GuidAndUpdateLeadDataRequestSent_NoErrorsReceived()
    {
        //arrange
        var id = Guid.NewGuid();
        var updateLeadDataRequest = TestsData.GetFakeUpdateLeadDataRequest();
        _leadsRepositoryMock.Setup(x => x.GetLeadByIdAsync(id)).ReturnsAsync(new LeadDto());
        var sut = new LeadsService(_leadsRepositoryMock.Object, null, null, null, null, null, null, null, null);

        //act
        await sut.UpdateLeadAsync(id, updateLeadDataRequest);

        //assert
        _leadsRepositoryMock.Verify(m => m.GetLeadByIdAsync(id), Times.Once);
        _leadsRepositoryMock.Verify(m => m.UpdateLeadAsync(It.IsAny<LeadDto>()), Times.Once);
    }

    [Fact]
    public async Task UpdateLeadAsyncNoLead_EmptyGuidAndUpdateLeadDataRequestSent_LeadNotFoundErrorReceived()
    {
        //arrange
        var id = Guid.NewGuid();
        var updateLeadDataRequest = TestsData.GetFakeUpdateLeadDataRequest();
        _leadsRepositoryMock.Setup(x => x.GetLeadByIdAsync(id)).ReturnsAsync((LeadDto)null);
        var sut = new LeadsService(_leadsRepositoryMock.Object, null, null, null, null, null, null, null, null);

        //act
        var act = async () => await sut.UpdateLeadAsync(id, updateLeadDataRequest);

        //assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage(string.Format(LeadsServiceExceptions.NotFoundException, id));
        _leadsRepositoryMock.Verify(m => m.GetLeadByIdAsync(id), Times.Once);
        _leadsRepositoryMock.Verify(m => m.UpdateLeadAsync(It.IsAny<LeadDto>()), Times.Never);
    }

    [Fact]
    public async Task UpdateLeadPasswordAsync_GuidAndUpdateLeadPasswordRequestSent_NoErrorsReceived()
    {
        //arrange
        var id = Guid.NewGuid();
        var updateLeadPasswordRequest = TestsData.GetFakeUpdateLeadPasswordRequest();
        _leadsRepositoryMock.Setup(x => x.GetLeadByIdAsync(id)).ReturnsAsync(new LeadDto());
        var sut = new LeadsService(_leadsRepositoryMock.Object, null, null, null, null, _secret, null, null, null);

        //act
        await sut.UpdateLeadPasswordAsync(id, updateLeadPasswordRequest);

        //assert
        _leadsRepositoryMock.Verify(m => m.GetLeadByIdAsync(id), Times.Once);
        _leadsRepositoryMock.Verify(m => m.UpdateLeadAsync(It.IsAny<LeadDto>()), Times.Once);
    }

    [Fact]
    public async Task UpdateLeadPasswordAsyncNoLead_EmptyGuidAndUpdateLeadPasswordRequestSent_LeadNotFoundErrorReceived()
    {
        //arrange
        var id = Guid.Empty;
        var updateLeadPasswordRequest = TestsData.GetFakeUpdateLeadPasswordRequest();
        _leadsRepositoryMock.Setup(x => x.GetLeadByIdAsync(id)).ReturnsAsync((LeadDto)null);
        var sut = new LeadsService(_leadsRepositoryMock.Object, null, null, null, null, null, null, null, null);

        //act
        var act = async () => await sut.UpdateLeadPasswordAsync(id, updateLeadPasswordRequest);

        //assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage(string.Format(LeadsServiceExceptions.NotFoundException, id));
        _leadsRepositoryMock.Verify(m => m.GetLeadByIdAsync(id), Times.Once);
        _leadsRepositoryMock.Verify(m => m.UpdateLeadAsync(It.IsAny<LeadDto>()), Times.Never);
    }

    [Fact]
    public async Task UpdateLeadStatusAsync_GuidAndUpdateLeadStatusRequestSent_NoErrorsReceived()
    {
        //arrange
        var id = Guid.NewGuid();
        var updateLeadStatusRequest = TestsData.GetFakeUpdateLeadStatusRequest();
        _leadsRepositoryMock.Setup(x => x.GetLeadByIdAsync(id)).ReturnsAsync(new LeadDto());
        var sut = new LeadsService(_leadsRepositoryMock.Object, null, null, null, null, null, null, null, null);

        //act
        await sut.UpdateLeadStatusAsync(id, updateLeadStatusRequest);

        //assert
        _leadsRepositoryMock.Verify(m => m.GetLeadByIdAsync(id), Times.Once);
        _leadsRepositoryMock.Verify(m => m.UpdateLeadAsync(It.IsAny<LeadDto>()), Times.Once);
    }

    [Fact]
    public void UpdateLeadStatusAsyncNoLead_EmptyGuidAndUpdateLeadStatusRequestSent_LeadNotFoundErrorReceived()
    {
        //arrange
        var id = Guid.Empty;
        var updateLeadStatusRequest = TestsData.GetFakeUpdateLeadStatusRequest();
        _leadsRepositoryMock.Setup(x => x.GetLeadByIdAsync(id)).ReturnsAsync((LeadDto)null);
        var sut = new LeadsService(_leadsRepositoryMock.Object, null, null, null, null, null, null, null, null);

        //act
        var act = () => sut.UpdateLeadStatusAsync(id, updateLeadStatusRequest);

        //assert
        act.Should().ThrowAsync<NotFoundException>()
            .WithMessage(string.Format(LeadsServiceExceptions.NotFoundException, id));
        _leadsRepositoryMock.Verify(m => m.GetLeadByIdAsync(id), Times.Once);
        _leadsRepositoryMock.Verify(m => m.UpdateLeadAsync(It.IsAny<LeadDto>()), Times.Never);
    }

    [Fact]
    public async Task UpdateLeadBirthDateAsync_GuidAndUpdateLeadBirthDateRequestSent_NoErrorsReceived()
    {
        //arrange
        var id = Guid.NewGuid();
        var updateLeadBirthDateRequest = TestsData.GetFakeUpdateLeadBirthDateRequest();
        _leadsRepositoryMock.Setup(x => x.GetLeadByIdAsync(id)).ReturnsAsync(new LeadDto());
        var sut = new LeadsService(_leadsRepositoryMock.Object, null, null, null, null, null, null, null, null);

        //act
        await sut.UpdateLeadBirthDateAsync(id, updateLeadBirthDateRequest);

        //assert
        _leadsRepositoryMock.Verify(m => m.GetLeadByIdAsync(id), Times.Once);
        _leadsRepositoryMock.Verify(m => m.UpdateLeadAsync(It.IsAny<LeadDto>()), Times.Once);
    }

    [Fact]
    public async Task UpdateLeadBirthDateAsyncNoLead_EmptyGuidAndUpdateLeadBirthDateRequestSent_LeadNotFoundErrorReceived()
    {
        //arrange
        var id = Guid.Empty;
        var updateLeadBirthDateRequest = TestsData.GetFakeUpdateLeadBirthDateRequest();
        _leadsRepositoryMock.Setup(x => x.GetLeadByIdAsync(id)).ReturnsAsync((LeadDto)null);
        var sut = new LeadsService(_leadsRepositoryMock.Object, null, null, null, null, null, null, null, null);

        //act
        var act = async () => await sut.UpdateLeadBirthDateAsync(id, updateLeadBirthDateRequest);

        //assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage(string.Format(LeadsServiceExceptions.NotFoundException, id));
        _leadsRepositoryMock.Verify(m => m.GetLeadByIdAsync(id), Times.Once);
        _leadsRepositoryMock.Verify(m => m.UpdateLeadAsync(It.IsAny<LeadDto>()), Times.Never);
    }

    [Fact]
    public async Task DeleteLeadByIdAsync_EmptyGuidSent_LeadNotFoundErrorReceived()
    {
        //arrange
        var id = Guid.Empty;
        _leadsRepositoryMock.Setup(x => x.GetLeadByIdAsync(id)).ReturnsAsync((LeadDto)null);
        var sut = new LeadsService(_leadsRepositoryMock.Object, null, null, null, null, null, null, null, null);

        //act
        var act = async () => await sut.DeleteLeadByIdAsync(id);

        //assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage(string.Format(LeadsServiceExceptions.NotFoundException, id));
        _leadsRepositoryMock.Verify(m => m.GetLeadByIdAsync(id), Times.Once);
        _transactionsManagerMock.Verify(m => m.BeginTransactionAsync(), Times.Never);
        _leadsRepositoryMock.Verify(m => m.UpdateLeadAsync(It.IsAny<LeadDto>()), Times.Never);
        _accountsRepositoryMock.Verify(m => m.UpdateAccountAsync(It.IsAny<AccountDto>()), Times.Never);
        _transactionsManagerMock.Verify(m => m.CommitTransactionAsync(It.IsAny<IDbContextTransaction>()), Times.Never);
    } 
}
