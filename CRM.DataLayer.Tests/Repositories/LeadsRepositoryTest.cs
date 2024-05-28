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
    public void AddLead_LeadDtoSent_GuidReceived()
    {
        //arrange
        const int expected = 3;
        var leads = TestData.GetFakeLeadDtoList();
        var lead = TestData.GetFakeLeadDto();
        var mock = leads.BuildMock().BuildMockDbSet();
        _contextMock.Setup(x => x.Leads.Add(lead))
            .Returns(mock.Object.Add(lead))
            .Callback<LeadDto>(leads.Add);
        var sut = new LeadsRepository(_contextMock.Object);

        //act
        var actual = sut.AddLead(lead);

        //assert
        Assert.Equal(expected, leads.Count);
        mock.Verify(m => m.Add(lead), Times.Once());
        _contextMock.Verify(m => m.SaveChanges(), Times.Once());
    }

    [Fact]
    public void GetLeads_Called_LeadDtoListReceived()
    {
        //arrange
        const int expected = 2;
        _contextMock.Setup(x => x.Leads)
            .ReturnsDbSet(TestData.GetFakeLeadDtoList());
        var sut = new LeadsRepository(_contextMock.Object);

        //act
        var actual = sut.GetLeads();

        //assert
        Assert.NotNull(actual);
        Assert.Equal(expected, actual.Count());
    }

    [Fact]
    public void GetLeadById_GuidSent_LeadDtoReceived()
    {
        //arrange
        var expected = new Guid("4e7918d2-fdcd-4316-97bb-565f8f4a0566");
        var mock = TestData.GetFakeLeadDtoList().BuildMock().BuildMockDbSet();
        _contextMock.Setup(x => x.Leads)
            .Returns(mock.Object);
        var sut = new LeadsRepository(_contextMock.Object);

        //act
        var actual = sut.GetLeadById(expected);

        //assert
        Assert.NotNull(actual);
        Assert.Equal(expected, actual.Id);
    }

    [Fact]
    public void GetLeadByMail_MailSent_LeadDtoReceived()
    {
        //arrange
        const string mail = "test02@test.test";
        var expected = new Guid("78fa8b9b-91fa-4e94-9a35-33d356d92890");
        var mock = TestData.GetFakeLeadDtoList().BuildMock().BuildMockDbSet();
        _contextMock.Setup(x => x.Leads)
            .Returns(mock.Object);
        var sut = new LeadsRepository(_contextMock.Object);

        //act
        var actual = sut.GetLeadByMail(mail);

        //assert
        Assert.NotNull(actual);
        Assert.Equal(expected, actual.Id);
    }

    [Fact]
    public void UpdateLead_LeadDtoSent_NoErrorsReceived()
    {
        //arrange
        var lead = TestData.GetFakeLeadDtoList()[0];
        var mock = Enumerable.Empty<LeadDto>().BuildMock().BuildMockDbSet();
        _contextMock.Setup(x => x.Leads.Update(lead))
            .Returns(mock.Object.Update(lead));
        var sut = new LeadsRepository(_contextMock.Object);

        //act
        sut.UpdateLead(lead);

        //assert
        mock.Verify(m => m.Update(lead), Times.Once());
        _contextMock.Verify(m => m.SaveChanges(), Times.Once());
    }
}
