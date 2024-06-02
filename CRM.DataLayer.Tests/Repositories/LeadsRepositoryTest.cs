using CRM.Core.Dtos;
using CRM.DataLayer.Repositories;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;
using Moq.EntityFrameworkCore;

namespace CRM.DataLayer.Tests.Repositories;

public class LeadsRepositoryTest
{
    private static readonly DbContextOptions<CrmContext> _options = new();
    private readonly Mock<CrmContext> _contextMock = new(_options);

    [Fact]
    public async Task AddLeadAsync_LeadDtoSent_GuidReceived()
    {
        //arrange
        var leads = new List<LeadDto>();
        var lead = TestData.GetFakeLeadDto();
        var mock = leads.BuildMock().BuildMockDbSet();
        _contextMock.Setup(x => x.Leads)
            .Returns(mock.Object);
        _contextMock.Setup(x => x.SaveChangesAsync(default))
            .Callback<CancellationToken>(_ => leads.Add(lead));
        var sut = new LeadsRepository(_contextMock.Object);

        //act
        var actual = await sut.AddLeadAsync(lead);

        //assert
        Assert.Matches(TestData.RegexGuid, actual.ToString());
        Assert.Single(leads);
        mock.Verify(m => m.AddAsync(lead,default), Times.Once());
        _contextMock.Verify(m => m.SaveChangesAsync(default), Times.Once());
    }

    [Fact]
    public async Task GetLeadsAsync_Called_LeadDtoListReceived()
    {
        //arrange
        const int expected = TestData.LeadsCount;
        _contextMock.Setup(x => x.Leads)
            .ReturnsDbSet(TestData.GetFakeLeadDtoList());
        var sut = new LeadsRepository(_contextMock.Object);

        //act
        var actual = await sut.GetLeadsAsync();

        //assert
        Assert.NotNull(actual);
        Assert.Equal(expected, actual.Count);
    }

    [Fact]
    public async Task GetLeadByIdAsync_GuidSent_LeadDtoReceived()
    {
        //arrange
        var expected = new Guid(TestData.Guid);
        var mock = TestData.GetFakeLeadDtoList().BuildMock().BuildMockDbSet();
        _contextMock.Setup(x => x.Leads)
            .Returns(mock.Object);
        var sut = new LeadsRepository(_contextMock.Object);

        //act
        var actual = await sut.GetLeadByIdAsync(expected);

        //assert
        Assert.NotNull(actual);
        Assert.Equal(expected, actual.Id);
    }

    [Fact]
    public async Task GetLeadByMailAsync_MailSent_LeadDtoReceived()
    {
        //arrange
        const string mail = TestData.Mail;
        var expected = new Guid(TestData.Guid);
        var mock = TestData.GetFakeLeadDtoList().BuildMock().BuildMockDbSet();
        _contextMock.Setup(x => x.Leads)
            .Returns(mock.Object);
        var sut = new LeadsRepository(_contextMock.Object);

        //act
        var actual = await sut.GetLeadByMailAsync(mail);

        //assert
        Assert.NotNull(actual);
        Assert.Equal(expected, actual.Id);
    }

    [Fact]
    public async Task UpdateLeadAsync_LeadDtoSent_NoErrorsReceived()
    {
        //arrange
        var lead = TestData.GetFakeLeadDto();
        var mock = Enumerable.Empty<LeadDto>().BuildMock().BuildMockDbSet();
        _contextMock.Setup(x => x.Leads)
            .Returns(mock.Object);
        var sut = new LeadsRepository(_contextMock.Object);

        //act
        await sut.UpdateLeadAsync(lead);

        //assert
        mock.Verify(m => m.Update(lead), Times.Once());
        _contextMock.Verify(m => m.SaveChangesAsync(default), Times.Once());
    }
}
