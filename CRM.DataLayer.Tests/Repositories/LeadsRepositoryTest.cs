using AutoFixture;
using CRM.Core.Dtos;
using CRM.Core.Fixture;
using CRM.DataLayer.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;
using Moq.EntityFrameworkCore;

namespace CRM.DataLayer.Tests.Repositories;

public class LeadsRepositoryTest
{
    private readonly Mock<CrmContext> _contextMock = new(new DbContextOptions<CrmContext>());
    private readonly CustomFixture _customFixture = new();

    [Fact]
    public async Task AddLeadAsync_LeadDtoSent_GuidReceived()
    {
        //arrange
        DatabaseConnection.SetDatabase(_contextMock);
        var fixture = _customFixture.GetFixture();
        var leads = new List<LeadDto>();
        var lead = fixture.Create<LeadDto>();
        var mock = leads.BuildMock().BuildMockDbSet();
        _contextMock.Setup(x => x.Leads)
            .Returns(mock.Object);
        _contextMock.Setup(x => x.SaveChangesAsync(default))
            .Callback<CancellationToken>(_ => leads.Add(lead));
        var sut = new LeadsRepository(_contextMock.Object);

        //act
        var actual = await sut.AddLeadAsync(lead);

        //assert
        Assert.Matches(CustomFixture.RegexGuid, actual.ToString());
        Assert.Single(leads);
        mock.Verify(m => m.AddAsync(lead,default), Times.Once());
        _contextMock.Verify(m => m.SaveChangesAsync(default), Times.Once());
    }
    
    [Fact]
    public async Task AddLeadAsync_NullSent_NullReferenceExceptionErrorReceived()
    {
        //arrange
        DatabaseConnection.SetDatabase(_contextMock);
        var mock = Enumerable.Empty<LeadDto>().BuildMock().BuildMockDbSet();
        _contextMock.Setup(x => x.Leads)
            .Returns(mock.Object);
        var sut = new LeadsRepository(_contextMock.Object);

        //act
        var act = async () => await sut.AddLeadAsync(null);

        //assert
        await act.Should().ThrowAsync<NullReferenceException>();
    }

    [Fact]
    public async Task GetLeadsAsync_Called_LeadDtoListReceived()
    {
        //arrange
        DatabaseConnection.SetDatabase(_contextMock);
        var fixture = _customFixture.GetFixture();
        var leads = fixture.Create<List<LeadDto>>().Where(d => d.IsDeleted == false).ToList();;
        var expected = leads.Count;
        _contextMock.Setup(x => x.Leads)
            .ReturnsDbSet(leads);
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
        DatabaseConnection.SetDatabase(_contextMock);
        var fixture = _customFixture.GetFixture();
        var leads = fixture.Create<List<LeadDto>>().Where(d => d.IsDeleted == false).ToList();
        var expected = leads.FirstOrDefault()!.Id;
        var mock = leads.BuildMock().BuildMockDbSet();
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
        DatabaseConnection.SetDatabase(_contextMock);
        var fixture = _customFixture.GetFixture();
        var leads = fixture.Create<List<LeadDto>>().Where(d => d.IsDeleted == false).ToList();
        var mail = leads.FirstOrDefault()?.Mail;
        var expected = leads.FirstOrDefault()?.Id;
        var mock = leads.BuildMock().BuildMockDbSet();
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
        DatabaseConnection.SetDatabase(_contextMock);
        var fixture = _customFixture.GetFixture();
        var lead = fixture.Create<LeadDto>();
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
    
    [Fact]
    public async Task UpdateLeadAsync_NullSent_NullReferenceExceptionErrorReceived()
    {
        //arrange
        DatabaseConnection.SetDatabase(_contextMock);
        var mock = Enumerable.Empty<LeadDto>().BuildMock().BuildMockDbSet();
        _contextMock.Setup(x => x.Leads)
            .Returns(mock.Object);
        var sut = new LeadsRepository(_contextMock.Object);

        //act
        var act = async () => await sut.UpdateLeadAsync(null);

        //assert
        await act.Should().ThrowAsync<NullReferenceException>();
    }
}
