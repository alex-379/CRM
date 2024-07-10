using AutoFixture;
using AutoMapper;
using CRM.Business.Configuration;
using CRM.Business.Models.Accounts;
using CRM.Business.Models.Accounts.Responses;
using CRM.Business.Models.Leads;
using CRM.Business.Models.Leads.Requests;
using CRM.Business.Models.Leads.Responses;
using CRM.Business.Services;
using CRM.Business.Services.Constants.Exceptions;
using CRM.Core.Dtos;
using CRM.Core.Enums;
using CRM.Core.Exceptions;
using CRM.Core.Fixture;
using CRM.DataLayer.Interfaces;
using FluentAssertions;
using MemoryCache.Testing.Moq;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Caching.Memory;
using Moq;


namespace CRM.Business.Tests.Services;

public class LeadsServiceTest
{
    private readonly Mock<ILeadsRepository> _leadsRepositoryMock;
    private readonly Mock<IAccountsRepository> _accountsRepositoryMock;
    private readonly Mock<ITransactionsManager> _transactionsManagerMock;
    private readonly IMemoryCache _memoryCacheMock;
    private readonly MessagesServiceTest _messagesService;
    private readonly IMapper _mapper;
    private readonly SecretSettings _secret;
    private readonly CustomFixture _customFixture;

    public LeadsServiceTest()
    {
        _leadsRepositoryMock = new Mock<ILeadsRepository>();
        _accountsRepositoryMock = new Mock<IAccountsRepository>();
        _transactionsManagerMock = new Mock<ITransactionsManager>();
        _secret = new SecretSettings();
        _messagesService = new MessagesServiceTest();
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new LeadsMappingProfile());
            cfg.AddProfile(new AccountsMappingProfile());
        });

        _mapper = new Mapper(config);
        _customFixture = new CustomFixture();
        _memoryCacheMock = Create.MockedMemoryCache();
    }

    [Fact]
    public async Task AddLeadAsync_RegistrationLeadRequestSent_GuidReceived()
    {
        //arrange
        var fixture = _customFixture.GetFixture();
        var registrationLeadRequest = fixture.Create<RegisterLeadRequest>();
        var expectedGuid = Guid.NewGuid();
        _leadsRepositoryMock.Setup(x => x.GetLeadByMailAsync(It.IsAny<string>())).ReturnsAsync((LeadDto)null);
        _leadsRepositoryMock.Setup(x => x.AddLeadAsync(It.IsAny<LeadDto>())).ReturnsAsync(expectedGuid);
        var sut = new LeadsService(_leadsRepositoryMock.Object, _accountsRepositoryMock.Object,
            _transactionsManagerMock.Object, null, _mapper, _secret, null, _messagesService, null);

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
        var fixture = _customFixture.GetFixture();
        var registrationLeadRequestWithDuplicateMail = fixture.Create<RegisterLeadRequest>();
        var lead = fixture.Create<LeadDto>();
        lead.IsDeleted = false;
        _leadsRepositoryMock.Setup(x => x.GetLeadByMailAsync(registrationLeadRequestWithDuplicateMail.Mail.ToLower())).ReturnsAsync(lead);
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
    public async Task AddLeadAsync_RegistrationLeadRequestSent_ConflictErrorDeletedLeadReceived()
    {
        //arrange
        var fixture = _customFixture.GetFixture();
        var registrationLeadRequestWithDuplicateMail = fixture.Create<RegisterLeadRequest>();
        var lead = fixture.Create<LeadDto>();
        lead.IsDeleted = true;
        _leadsRepositoryMock.Setup(x => x.GetLeadByMailAsync(registrationLeadRequestWithDuplicateMail.Mail.ToLower())).ReturnsAsync(lead);
        _leadsRepositoryMock.Setup(x => x.AddLeadAsync(It.IsAny<LeadDto>())).ReturnsAsync(Guid.NewGuid());
        var sut = new LeadsService(_leadsRepositoryMock.Object, null, null, null, _mapper, null, null, null, null);
        
        //act
        var act = async () => await sut.AddLeadAsync(registrationLeadRequestWithDuplicateMail);
        
        //assert
        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage(LeadsServiceExceptions.ConflictExceptionIsDeleted);
        _leadsRepositoryMock.Verify(m => m.AddLeadAsync(It.IsAny<LeadDto>()), Times.Never);
    }


    [Fact]
    public async Task LoginLeadAsync_LoginLeadRequestIncorrectMailSent_LeadUnauthenticatedErrorReceived()
    {
        //arrange
        var fixture = _customFixture.GetFixture();
        var loginLeadRequestIncorrectMail = fixture.Create<LoginLeadRequest>();
        _leadsRepositoryMock.Setup(x => x.GetLeadByMailAsync(loginLeadRequestIncorrectMail.Mail.ToLower())).ReturnsAsync((LeadDto)null);
        var sut = new LeadsService(_leadsRepositoryMock.Object, null, null, null, _mapper, null, null, null, null);

        //act
        var act = async () => await sut.LoginLeadAsync(loginLeadRequestIncorrectMail);

        //assert
        await act.Should().ThrowAsync<UnauthenticatedException>();
        _leadsRepositoryMock.Verify(m => m.GetLeadByMailAsync(loginLeadRequestIncorrectMail.Mail.ToLower()), Times.Once);
        _leadsRepositoryMock.Verify(m => m.UpdateLeadAsync(It.IsAny<LeadDto>()), Times.Never);
    }

    [Fact]
    public async Task LoginLeadAsync_LoginLeadRequestIncorrectPasswordSent_LeadUnauthenticatedErrorReceived()
    {
        //arrange
        var fixture = _customFixture.GetFixture();
        var loginLeadRequestIncorrectPassword = fixture.Create<LoginLeadRequest>();
        var lead = fixture.Create<LeadDto>();
        _leadsRepositoryMock.Setup(x => x.GetLeadByMailAsync(loginLeadRequestIncorrectPassword.Mail.ToLower())).ReturnsAsync(lead);
        var sut = new LeadsService(_leadsRepositoryMock.Object, null, null, null, _mapper, _secret, null, null, null);

        //act
        var act = async () => await sut.LoginLeadAsync(loginLeadRequestIncorrectPassword);

        //assert
        await act.Should().ThrowAsync<UnauthenticatedException>();
        _leadsRepositoryMock.Verify(m => m.GetLeadByMailAsync(loginLeadRequestIncorrectPassword.Mail.ToLower()), Times.Once);
        _leadsRepositoryMock.Verify(m => m.UpdateLeadAsync(It.IsAny<LeadDto>()), Times.Never);
    }

    [Fact]
    public async Task Login2FaLeadAsync_Login2FaLeadRequestSentEmptyCache_LeadUnauthenticatedErrorReceived()
    {
        //arrange
        var fixture = _customFixture.GetFixture();
        var loginLeadRequestIncorrectPassword = fixture.Create<Login2FaLeadRequest>(); 
        _memoryCacheMock.GetOrCreate(new Guid(), entry => loginLeadRequestIncorrectPassword.Code);
        var sut = new LeadsService(_leadsRepositoryMock.Object, null, null, null, _mapper, _secret, null, null, _memoryCacheMock);

        //act
        var act = async () => await sut.Login2FaLeadAsync(loginLeadRequestIncorrectPassword);

        //assert
        await act.Should().ThrowAsync<UnauthenticatedException>();
        _leadsRepositoryMock.Verify(m => m.GetLeadByMailAsync(loginLeadRequestIncorrectPassword.Mail.ToLower()), Times.Never);
        _leadsRepositoryMock.Verify(m => m.UpdateLeadAsync(It.IsAny<LeadDto>()), Times.Never);
    }
    
    [Fact]
    public async Task Login2FaLeadAsync_Login2FaLeadRequestIncorrectCodeSent_LeadUnauthenticatedErrorReceived()
    {
        //arrange
        var fixture = _customFixture.GetFixture();
        var loginLeadRequestIncorrectPassword = fixture.Create<Login2FaLeadRequest>(); 
        _memoryCacheMock.GetOrCreate(loginLeadRequestIncorrectPassword.Token, entry => It.IsAny<int>());
        var sut = new LeadsService(_leadsRepositoryMock.Object, null, null, null, _mapper, _secret, null, null, _memoryCacheMock);

        //act
        var act = async () => await sut.Login2FaLeadAsync(loginLeadRequestIncorrectPassword);

        //assert
        await act.Should().ThrowAsync<UnauthenticatedException>();
        _leadsRepositoryMock.Verify(m => m.GetLeadByMailAsync(loginLeadRequestIncorrectPassword.Mail.ToLower()), Times.Never);
        _leadsRepositoryMock.Verify(m => m.UpdateLeadAsync(It.IsAny<LeadDto>()), Times.Never);
    }
    
    [Fact]
    public async Task GetLeadsAsync_Called_ListLeadResponseReceived()
    {
        //arrange
        var fixture = _customFixture.GetFixture();
        var expectedLeads = new List<LeadDto>()
        {
            fixture.Create<LeadDto>()
        };
        var expected = new List<LeadResponse>()
        {
            new()
            {
                Id = expectedLeads[0].Id,
                Name = expectedLeads[0].Name,
                Mail = expectedLeads[0].Mail,
                Phone = expectedLeads[0].Phone
            }
        };
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
        var fixture = _customFixture.GetFixture();
        var id = Guid.NewGuid();
        var expectedLead = fixture.Create<LeadDto>();
        expectedLead.Accounts = [new AccountDto(){Currency = Currency.Ars}];
        var expected = new LeadFullResponse
        {
            Id = expectedLead.Id,
            Name = expectedLead.Name,
            Mail = expectedLead.Mail,
            Phone = expectedLead.Phone,
            Address = expectedLead.Address,
            BirthDate = expectedLead.BirthDate,
            Status = expectedLead.Status,
            Accounts = [new AccountResponse {Currency = Currency.Ars}],
        };
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
        var fixture = _customFixture.GetFixture();
        var id = Guid.NewGuid();
        var updateLeadDataRequest = fixture.Create<UpdateLeadDataRequest>();
        _leadsRepositoryMock.Setup(x => x.GetLeadByIdAsync(id)).ReturnsAsync(new LeadDto());
        var sut = new LeadsService(_leadsRepositoryMock.Object, null, null, null, null, null, null, _messagesService, null);

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
        var fixture = _customFixture.GetFixture();
        var id = Guid.Empty;
        var updateLeadDataRequest = fixture.Create<UpdateLeadDataRequest>();
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
        var fixture = _customFixture.GetFixture();
        var id = Guid.NewGuid();
        var updateLeadPasswordRequest = fixture.Create<UpdateLeadPasswordRequest>();
        _leadsRepositoryMock.Setup(x => x.GetLeadByIdAsync(id)).ReturnsAsync(new LeadDto());
        var sut = new LeadsService(_leadsRepositoryMock.Object, null, null, null, null, _secret, null, _messagesService, null);

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
        var fixture = _customFixture.GetFixture();
        var id = Guid.NewGuid();
        var updateLeadPasswordRequest = fixture.Create<UpdateLeadPasswordRequest>();
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
        var fixture = _customFixture.GetFixture();
        var id = Guid.NewGuid();
        var updateLeadStatusRequest = fixture.Create<UpdateLeadStatusRequest>();
        _leadsRepositoryMock.Setup(x => x.GetLeadByIdAsync(id)).ReturnsAsync(new LeadDto());
        var sut = new LeadsService(_leadsRepositoryMock.Object, null, null, null, null, null, null, _messagesService, null);

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
        var fixture = _customFixture.GetFixture();
        var id = Guid.Empty;
        var updateLeadStatusRequest = fixture.Create<UpdateLeadStatusRequest>();
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
        var fixture = _customFixture.GetFixture();
        var id = Guid.NewGuid();
        var updateLeadBirthDateRequest = fixture.Create<UpdateLeadBirthDateRequest>();
        _leadsRepositoryMock.Setup(x => x.GetLeadByIdAsync(id)).ReturnsAsync(new LeadDto());
        var sut = new LeadsService(_leadsRepositoryMock.Object, null, null, null, null, null, null, _messagesService, null);

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
        var fixture = _customFixture.GetFixture();
        var id = Guid.Empty;
        var updateLeadBirthDateRequest = fixture.Create<UpdateLeadBirthDateRequest>();
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
