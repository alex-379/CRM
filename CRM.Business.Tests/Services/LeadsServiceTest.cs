using AutoMapper;
using CRM.Business.Configuration;
using CRM.Business.Interfaces;
using CRM.Business.Models.Accounts;
using CRM.Business.Models.Leads;
using CRM.Business.Services;
using CRM.Core.Constants.Exceptions.Business;
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
    private readonly Mock<ITransactionsRepository> _transactionsRepositoryMock;
    private readonly IPasswordsService _passwordsService;
    private readonly ITokensService _tokensService;
    private readonly IMapper _mapper;
    private readonly SecretSettings _secret;
    private readonly JwtToken _jwt;

    public LeadsServiceTest()
    {
        _leadsRepositoryMock = new Mock<ILeadsRepository>();
        _accountsRepositoryMock = new Mock<IAccountsRepository>();
        _transactionsRepositoryMock = new Mock<ITransactionsRepository>();
        _secret = new SecretSettings();
        _jwt = new JwtToken();
        _passwordsService = new PasswordsService(_secret);
        _tokensService = new TokensService(_secret, _jwt, _leadsRepositoryMock.Object);
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new LeadsMappingProfile());
            cfg.AddProfile(new AccountsMappingProfile());
        });

        _mapper = new Mapper(config);
    }

    [Fact]
    public void AddLead_RegistrationLeadRequestSent_GuidReceived()
    {
        //arrange
        var registrationLeadRequest = TestsData.GetFakeRegistrationLeadRequest();
        var expectedGuid = Guid.NewGuid();
        _leadsRepositoryMock.Setup(x => x.GetLeadByMail(It.IsAny<string>())).Returns((LeadDto)null);
        _transactionsRepositoryMock.Setup(x => x.BeginTransaction()).Returns(It.IsAny<IDbContextTransaction>());
        _leadsRepositoryMock.Setup(x => x.AddLead(It.IsAny<LeadDto>())).Returns(expectedGuid);
        _transactionsRepositoryMock.Setup(x => x.CommitTransaction(It.IsAny<IDbContextTransaction>()));
        var sut = new LeadsService(_leadsRepositoryMock.Object, _accountsRepositoryMock.Object, _transactionsRepositoryMock.Object,
            _passwordsService, _tokensService, _mapper, _jwt);

        //act
        var actual = sut.AddLead(registrationLeadRequest);

        //assert
        Assert.Equal(expectedGuid, actual);
        _leadsRepositoryMock.Verify(m => m.GetLeadByMail(It.IsAny<string>()), Times.Once);
        _transactionsRepositoryMock.Verify(m => m.BeginTransaction(), Times.Once);
        _leadsRepositoryMock.Verify(m => m.AddLead(It.IsAny<LeadDto>()), Times.Once);
        _accountsRepositoryMock.Verify(m => m.AddAccount(It.IsAny<AccountDto>()), Times.Once);
        _transactionsRepositoryMock.Verify(m => m.CommitTransaction(It.IsAny<IDbContextTransaction>()), Times.Once);
    }

    [Fact]
    public void AddLead_RegistrationLeadRequestSent_ConflictErrorReceived()
    {
        //arrange
        var registrationLeadRequestWithDuplicateMail = TestsData.GetFakeRegistrationLeadRequest();
        _leadsRepositoryMock.Setup(x => x.GetLeadByMail(registrationLeadRequestWithDuplicateMail.Mail)).Returns(new LeadDto());
        _leadsRepositoryMock.Setup(x => x.AddLead(It.IsAny<LeadDto>())).Returns(Guid.NewGuid());
        var sut = new LeadsService(_leadsRepositoryMock.Object, _accountsRepositoryMock.Object, _transactionsRepositoryMock.Object,
            _passwordsService, _tokensService, _mapper, _jwt);

        //act
        Action act = () => sut.AddLead(registrationLeadRequestWithDuplicateMail);

        //assert
        act.Should().Throw<ConflictException>()
            .WithMessage(LeadsServiceExceptions.ConflictException);
        _leadsRepositoryMock.Verify(m => m.AddLead(It.IsAny<LeadDto>()), Times.Never);
    }

    [Fact]
    public void LoginLead_LoginLeadRequestIncorrectMailSent_LeadUnauthenticatedErrorReceived()
    {
        //arrange
        var loginLeadRequestIncorrectMail = TestsData.GetFakeLoginLeadRequest();
        _leadsRepositoryMock.Setup(x => x.GetLeadByMail(loginLeadRequestIncorrectMail.Mail)).Returns((LeadDto)null);
        var sut = new LeadsService(_leadsRepositoryMock.Object, _accountsRepositoryMock.Object, _transactionsRepositoryMock.Object,
            _passwordsService, _tokensService, _mapper, _jwt);

        //act
        Action act = () => sut.LoginLead(loginLeadRequestIncorrectMail);

        //assert
        act.Should().Throw<UnauthenticatedException>();
        _leadsRepositoryMock.Verify(m => m.GetLeadByMail(loginLeadRequestIncorrectMail.Mail), Times.Once);
        _leadsRepositoryMock.Verify(m => m.UpdateLead(It.IsAny<LeadDto>()), Times.Never);
    }

    [Fact]
    public void LoginLead_LoginLeadRequestIncorrectPasswordSent_LeadUnauthenticatedErrorReceived()
    {
        //arrange
        var loginLeadRequestIncorrectPassword = TestsData.GetFakeLoginLeadRequest();
        var lead = TestsData.GetFakeLeadDto();
        _leadsRepositoryMock.Setup(x => x.GetLeadByMail(loginLeadRequestIncorrectPassword.Mail)).Returns(lead);
        var sut = new LeadsService(_leadsRepositoryMock.Object, _accountsRepositoryMock.Object, _transactionsRepositoryMock.Object,
            _passwordsService, _tokensService, _mapper, _jwt);

        //act
        Action act = () => sut.LoginLead(loginLeadRequestIncorrectPassword);

        //assert
        act.Should().Throw<UnauthenticatedException>();
        _leadsRepositoryMock.Verify(m => m.GetLeadByMail(loginLeadRequestIncorrectPassword.Mail), Times.Once);
        _leadsRepositoryMock.Verify(m => m.UpdateLead(It.IsAny<LeadDto>()), Times.Never);
    }

    [Fact]
    public void GetLeads_Called_ListLeadResponseReceived()
    {
        //arrange
        var expected = TestsData.GetFakeListLeadResponse();
        var expectedLeads = TestsData.GetFakeListLeadDto();
        _leadsRepositoryMock.Setup(x => x.GetLeads()).Returns(expectedLeads);
        var sut = new LeadsService(_leadsRepositoryMock.Object, _accountsRepositoryMock.Object, _transactionsRepositoryMock.Object,
            _passwordsService, _tokensService, _mapper, _jwt);

        //act
        var actual = sut.GetLeads();

        //assert
        actual.Should().BeEquivalentTo(expected);
        _leadsRepositoryMock.Verify(m => m.GetLeads(), Times.Once);
    }

    [Fact]
    public void GetLeadById_GuidSent_LeadResponseReceived()
    {
        //arrange
        var expected = TestsData.GetFakeLeadFullResponse();
        var expectedLead = TestsData.GetFakeLeadDto();
        var id = Guid.NewGuid();
        _leadsRepositoryMock.Setup(x => x.GetLeadById(id)).Returns(expectedLead);
        var sut = new LeadsService(_leadsRepositoryMock.Object, _accountsRepositoryMock.Object, _transactionsRepositoryMock.Object,
            _passwordsService, _tokensService, _mapper, _jwt);

        //act
        var actual = sut.GetLeadById(id);

        //assert
        actual.Should().BeEquivalentTo(expected);
        _leadsRepositoryMock.Verify(m => m.GetLeadById(id), Times.Once);
    }

    [Fact]
    public void GetLeadByIdNoLead_EmptyGuidSent_LeadNotFoundErrorReceived()
    {
        //arrange
        var id = Guid.Empty;
        _leadsRepositoryMock.Setup(x => x.GetLeadById(id)).Returns((LeadDto)null);
        var sut = new LeadsService(_leadsRepositoryMock.Object, _accountsRepositoryMock.Object, _transactionsRepositoryMock.Object,
            _passwordsService, _tokensService, _mapper, _jwt);

        //act
        Action act = () => sut.GetLeadById(id);

        //assert
        act.Should().Throw<NotFoundException>()
            .WithMessage(string.Format(LeadsServiceExceptions.NotFoundException, id));
        _leadsRepositoryMock.Verify(m => m.GetLeadById(id), Times.Once);
    }

    [Fact]
    public void UpdateLead_GuidAndUpdateLeadDataRequestSent_NoErrorsReceived()
    {
        //arrange
        var id = Guid.NewGuid();
        var updateLeadDataRequest = TestsData.GetFakeUpdateLeadDataRequest();
        _leadsRepositoryMock.Setup(x => x.GetLeadById(id)).Returns(new LeadDto());
        var sut = new LeadsService(_leadsRepositoryMock.Object, _accountsRepositoryMock.Object, _transactionsRepositoryMock.Object,
            _passwordsService, _tokensService, _mapper, _jwt);

        //act
        sut.UpdateLead(id, updateLeadDataRequest);

        //assert
        _leadsRepositoryMock.Verify(m => m.GetLeadById(id), Times.Once);
        _leadsRepositoryMock.Verify(m => m.UpdateLead(It.IsAny<LeadDto>()), Times.Once);
    }

    [Fact]
    public void UpdateLeadNoLead_EmptyGuidAndUpdateLeadDataRequestSent_LeadNotFoundErrorReceived()
    {
        //arrange
        var id = Guid.NewGuid();
        var updateLeadDataRequest = TestsData.GetFakeUpdateLeadDataRequest();
        _leadsRepositoryMock.Setup(x => x.GetLeadById(id)).Returns((LeadDto)null);
        var sut = new LeadsService(_leadsRepositoryMock.Object, _accountsRepositoryMock.Object, _transactionsRepositoryMock.Object,
            _passwordsService, _tokensService, _mapper, _jwt);

        //act
        Action act = () => sut.UpdateLead(id, updateLeadDataRequest);

        //assert
        act.Should().Throw<NotFoundException>()
            .WithMessage(string.Format(LeadsServiceExceptions.NotFoundException, id));
        _leadsRepositoryMock.Verify(m => m.GetLeadById(id), Times.Once);
        _leadsRepositoryMock.Verify(m => m.UpdateLead(It.IsAny<LeadDto>()), Times.Never);
    }

    [Fact]
    public void UpdateLeadPassword_GuidAndUpdateLeadPasswordRequestSent_NoErrorsReceived()
    {
        //arrange
        var id = Guid.NewGuid();
        var updateLeadPasswordRequest = TestsData.GetFakeUpdateLeadPasswordRequest();
        _leadsRepositoryMock.Setup(x => x.GetLeadById(id)).Returns(new LeadDto());
        var sut = new LeadsService(_leadsRepositoryMock.Object, _accountsRepositoryMock.Object, _transactionsRepositoryMock.Object,
            _passwordsService, _tokensService, _mapper, _jwt);

        //act
        sut.UpdateLeadPassword(id, updateLeadPasswordRequest);

        //assert
        _leadsRepositoryMock.Verify(m => m.GetLeadById(id), Times.Once);
        _leadsRepositoryMock.Verify(m => m.UpdateLead(It.IsAny<LeadDto>()), Times.Once);
    }

    [Fact]
    public void UpdateLeadPasswordNoLead_EmptyGuidAndUpdateLeadPasswordRequestSent_LeadNotFoundErrorReceived()
    {
        //arrange
        var id = Guid.Empty;
        var updateLeadPasswordRequest = TestsData.GetFakeUpdateLeadPasswordRequest();
        _leadsRepositoryMock.Setup(x => x.GetLeadById(id)).Returns((LeadDto)null);
        var sut = new LeadsService(_leadsRepositoryMock.Object, _accountsRepositoryMock.Object, _transactionsRepositoryMock.Object,
            _passwordsService, _tokensService, _mapper, _jwt);

        //act
        Action act = () => sut.UpdateLeadPassword(id, updateLeadPasswordRequest);

        //assert
        act.Should().Throw<NotFoundException>()
            .WithMessage(string.Format(LeadsServiceExceptions.NotFoundException, id));
        _leadsRepositoryMock.Verify(m => m.GetLeadById(id), Times.Once);
        _leadsRepositoryMock.Verify(m => m.UpdateLead(It.IsAny<LeadDto>()), Times.Never);
    }

    [Fact]
    public void UpdateLeadMail_GuidAndUpdateLeadMailRequestSent_NoErrorsReceived()
    {
        //arrange
        var id = Guid.NewGuid();
        var updateLeadMailRequest = TestsData.GetFakeUpdateLeadMailRequest();
        _leadsRepositoryMock.Setup(x => x.GetLeadById(id)).Returns(new LeadDto());
        var sut = new LeadsService(_leadsRepositoryMock.Object, _accountsRepositoryMock.Object, _transactionsRepositoryMock.Object,
            _passwordsService, _tokensService, _mapper, _jwt);

        //act
        sut.UpdateLeadMail(id, updateLeadMailRequest);

        //assert
        _leadsRepositoryMock.Verify(m => m.GetLeadById(id), Times.Once);
        _leadsRepositoryMock.Verify(m => m.UpdateLead(It.IsAny<LeadDto>()), Times.Once);
    }

    [Fact]
    public void UpdateLeadMailNoLead_EmptyGuidAndUpdateLeadMailRequestSent_LeadNotFoundErrorReceived()
    {
        //arrange
        var id = Guid.Empty;
        var updateLeadMailRequest = TestsData.GetFakeUpdateLeadMailRequest();
        _leadsRepositoryMock.Setup(x => x.GetLeadById(id)).Returns((LeadDto)null);
        var sut = new LeadsService(_leadsRepositoryMock.Object, _accountsRepositoryMock.Object, _transactionsRepositoryMock.Object,
            _passwordsService, _tokensService, _mapper, _jwt);

        //act
        Action act = () => sut.UpdateLeadMail(id, updateLeadMailRequest);

        //assert
        act.Should().Throw<NotFoundException>()
            .WithMessage(string.Format(LeadsServiceExceptions.NotFoundException, id));
        _leadsRepositoryMock.Verify(m => m.GetLeadById(id), Times.Once);
        _leadsRepositoryMock.Verify(m => m.UpdateLead(It.IsAny<LeadDto>()), Times.Never);
    }

    [Fact]
    public void UpdateLeadMailDuplicateMail_GuidAndUpdateLeadMailRequestSent_ConflictErrorReceived()
    {
        //arrange
        var id = Guid.NewGuid();
        var updateLeadMailRequest = TestsData.GetFakeUpdateLeadMailRequest();
        _leadsRepositoryMock.Setup(x => x.GetLeadById(id)).Returns(new LeadDto());
        _leadsRepositoryMock.Setup(x => x.GetLeadByMail(updateLeadMailRequest.Mail)).Returns(new LeadDto());
        var sut = new LeadsService(_leadsRepositoryMock.Object, _accountsRepositoryMock.Object, _transactionsRepositoryMock.Object,
            _passwordsService, _tokensService, _mapper, _jwt);

        //act
        Action act = () => sut.UpdateLeadMail(id, updateLeadMailRequest);

        //assert
        act.Should().Throw<ConflictException>()
            .WithMessage(LeadsServiceExceptions.ConflictException);
        _leadsRepositoryMock.Verify(m => m.GetLeadById(id), Times.Once);
        _leadsRepositoryMock.Verify(m => m.GetLeadByMail(updateLeadMailRequest.Mail), Times.Once);
        _leadsRepositoryMock.Verify(m => m.UpdateLead(It.IsAny<LeadDto>()), Times.Never);
    }

    [Fact]
    public void UpdateLeadStatus_GuidAndUpdateLeadStatusRequestSent_NoErrorsReceived()
    {
        //arrange
        var id = Guid.NewGuid();
        var updateLeadStatusRequest = TestsData.GetFakeUpdateLeadStatusRequest();
        _leadsRepositoryMock.Setup(x => x.GetLeadById(id)).Returns(new LeadDto());
        var sut = new LeadsService(_leadsRepositoryMock.Object, _accountsRepositoryMock.Object, _transactionsRepositoryMock.Object,
            _passwordsService, _tokensService, _mapper, _jwt);

        //act
        sut.UpdateLeadStatus(id, updateLeadStatusRequest);

        //assert
        _leadsRepositoryMock.Verify(m => m.GetLeadById(id), Times.Once);
        _leadsRepositoryMock.Verify(m => m.UpdateLead(It.IsAny<LeadDto>()), Times.Once);
    }

    [Fact]
    public void UpdateLeadStatusNoLead_EmptyGuidAndUpdateLeadStatusRequestSent_LeadNotFoundErrorReceived()
    {
        //arrange
        var id = Guid.Empty;
        var updateLeadStatusRequest = TestsData.GetFakeUpdateLeadStatusRequest();
        _leadsRepositoryMock.Setup(x => x.GetLeadById(id)).Returns((LeadDto)null);
        var sut = new LeadsService(_leadsRepositoryMock.Object, _accountsRepositoryMock.Object, _transactionsRepositoryMock.Object,
            _passwordsService, _tokensService, _mapper, _jwt);

        //act
        Action act = () => sut.UpdateLeadStatus(id, updateLeadStatusRequest);

        //assert
        act.Should().Throw<NotFoundException>()
            .WithMessage(string.Format(LeadsServiceExceptions.NotFoundException, id));
        _leadsRepositoryMock.Verify(m => m.GetLeadById(id), Times.Once);
        _leadsRepositoryMock.Verify(m => m.UpdateLead(It.IsAny<LeadDto>()), Times.Never);
    }

    [Fact]
    public void UpdateLeadBirthDate_GuidAndUpdateLeadBirthDateRequestSent_NoErrorsReceived()
    {
        //arrange
        var id = Guid.NewGuid();
        var updateLeadBirthDateRequest = TestsData.GetFakeUpdateLeadBirthDateRequest();
        _leadsRepositoryMock.Setup(x => x.GetLeadById(id)).Returns(new LeadDto());
        var sut = new LeadsService(_leadsRepositoryMock.Object, _accountsRepositoryMock.Object, _transactionsRepositoryMock.Object,
            _passwordsService, _tokensService, _mapper, _jwt);

        //act
        sut.UpdateLeadBirthDate(id, updateLeadBirthDateRequest);

        //assert
        _leadsRepositoryMock.Verify(m => m.GetLeadById(id), Times.Once);
        _leadsRepositoryMock.Verify(m => m.UpdateLead(It.IsAny<LeadDto>()), Times.Once);
    }

    [Fact]
    public void UpdateLeadBirthDateNoLead_EmptyGuidAndUpdateLeadBirthDateRequestSent_LeadNotFoundErrorReceived()
    {
        //arrange
        var id = Guid.Empty;
        var updateLeadBirthDateRequest = TestsData.GetFakeUpdateLeadBirthDateRequest();
        _leadsRepositoryMock.Setup(x => x.GetLeadById(id)).Returns((LeadDto)null);
        var sut = new LeadsService(_leadsRepositoryMock.Object, _accountsRepositoryMock.Object, _transactionsRepositoryMock.Object,
            _passwordsService, _tokensService, _mapper, _jwt);

        //act
        Action act = () => sut.UpdateLeadBirthDate(id, updateLeadBirthDateRequest);

        //assert
        act.Should().Throw<NotFoundException>()
            .WithMessage(string.Format(LeadsServiceExceptions.NotFoundException, id));
        _leadsRepositoryMock.Verify(m => m.GetLeadById(id), Times.Once);
        _leadsRepositoryMock.Verify(m => m.UpdateLead(It.IsAny<LeadDto>()), Times.Never);
    }

    [Fact]
    public void DeleteLeadById_GuidSent_NoErrorsReceived()
    {
        //arrange
        var id = Guid.NewGuid();
        var lead = TestsData.GetFakeLeadDto();
        _leadsRepositoryMock.Setup(x => x.GetLeadById(id)).Returns(lead);
        _transactionsRepositoryMock.Setup(x => x.BeginTransaction()).Returns(It.IsAny<IDbContextTransaction>());
        _transactionsRepositoryMock.Setup(x => x.CommitTransaction(It.IsAny<IDbContextTransaction>()));
        var sut = new LeadsService(_leadsRepositoryMock.Object, _accountsRepositoryMock.Object, _transactionsRepositoryMock.Object,
            _passwordsService, _tokensService, _mapper, _jwt);

        //act
        sut.DeleteLeadById(id);

        //assert
        _leadsRepositoryMock.Verify(m => m.GetLeadById(id), Times.Once);
        _transactionsRepositoryMock.Verify(m => m.BeginTransaction(), Times.Once);
        _leadsRepositoryMock.Verify(m => m.UpdateLead(lead), Times.Once);
        _accountsRepositoryMock.Verify(m => m.UpdateAccount(It.IsAny<AccountDto>()), Times.Exactly(2));
        _transactionsRepositoryMock.Verify(m => m.CommitTransaction(It.IsAny<IDbContextTransaction>()), Times.Once);
    }

    [Fact]
    public void DeleteLeadById_EmptyGuidSent_LeadNotFoundErrorReceived()
    {
        //arrange
        var id = Guid.Empty;
        _leadsRepositoryMock.Setup(x => x.GetLeadById(id)).Returns((LeadDto)null);
        _transactionsRepositoryMock.Setup(x => x.BeginTransaction()).Returns(It.IsAny<IDbContextTransaction>());
        _transactionsRepositoryMock.Setup(x => x.CommitTransaction(It.IsAny<IDbContextTransaction>()));
        var sut = new LeadsService(_leadsRepositoryMock.Object, _accountsRepositoryMock.Object, _transactionsRepositoryMock.Object,
            _passwordsService, _tokensService, _mapper, _jwt);

        //act
        Action act = () => sut.DeleteLeadById(id);

        //assert
        act.Should().Throw<NotFoundException>()
            .WithMessage(string.Format(LeadsServiceExceptions.NotFoundException, id));
        _leadsRepositoryMock.Verify(m => m.GetLeadById(id), Times.Once);
        _transactionsRepositoryMock.Verify(m => m.BeginTransaction(), Times.Never);
        _leadsRepositoryMock.Verify(m => m.UpdateLead(It.IsAny<LeadDto>()), Times.Never);
        _accountsRepositoryMock.Verify(m => m.UpdateAccount(It.IsAny<AccountDto>()), Times.Never);
        _transactionsRepositoryMock.Verify(m => m.CommitTransaction(It.IsAny<IDbContextTransaction>()), Times.Never);
    }
}
