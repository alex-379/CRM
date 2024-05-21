using CRM.Core.Dtos;
using CRM.DataLayer.Repositories;
using MockQueryable.Moq;
using Moq;

namespace CRM.DataLayer.Tests.Repositories;

public class LeadsRepositoryTest
{
    private readonly Mock<CrmContext> _contextMock;

    public LeadsRepositoryTest()
    {
        _contextMock = new Mock<CrmContext>();
    }

    [Fact]
    public void AddLead_LeadDtoSent_GuidReceieved()
    {
        //arrange
        var expected = 3;
        var leads = TestsData.GetFakeLeadDtoList();
        var lead = TestsData.GetFakeLeadDto();
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
    public void GetLeadById_GuidSent_LeadDtoReceieved()
    {
        //arrange
        var expected = new Guid("4e7918d2-fdcd-4316-97bb-565f8f4a0566");
        var mock = TestsData.GetFakeLeadDtoList().BuildMock().BuildMockDbSet();
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
    public void GetLeadByMail_MailSent_LeadDtoReceieved()
    {
        //arrange
        var mail = "test02@test.test";
        var expected = new Guid("78fa8b9b-91fa-4e94-9a35-33d356d92890");
        var mock = TestsData.GetFakeLeadDtoList().BuildMock().BuildMockDbSet();
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
    public void UpdateLead_LeadDtoSent_NoErrorsReceieved()
    {
        //arrange
        var lead = TestsData.GetFakeLeadDtoList()[0];
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
