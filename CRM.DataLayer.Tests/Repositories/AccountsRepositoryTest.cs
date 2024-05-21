using CRM.Core.Dtos;
using CRM.DataLayer.Repositories;
using MockQueryable.Moq;
using Moq;

namespace CRM.DataLayer.Tests.Repositories;

public class AccountsRepositoryTest
{
    private readonly Mock<CrmContext> _contextMock;

    public AccountsRepositoryTest()
    {
        _contextMock = new Mock<CrmContext>();
    }

    [Fact]
    public void AddAccount_AccountDtoSent_GuidReceieved()
    {
        //arrange
        var expected = 1;
        var accounts = new List<AccountDto>();
        var account = TestsData.GetFakeAccountDto();
        var mock = accounts.BuildMock().BuildMockDbSet();
        _contextMock.Setup(x => x.Accounts.Add(account))
            .Returns(mock.Object.Add(account))
            .Callback<AccountDto>(accounts.Add);
        var sut = new AccountsRepository(_contextMock.Object);

        //act
        var actual = sut.AddAccount(account);

        //assert
        Assert.Equal(expected, accounts.Count);
        mock.Verify(m => m.Add(account), Times.Once());
        _contextMock.Verify(m => m.SaveChanges(), Times.Once());
    }

    [Fact]
    public void UpdateAccount_AccountDtoSent_NoErrorsReceieved()
    {
        //arrange
        var account = TestsData.GetFakeAccountDto();
        var mock = Enumerable.Empty<AccountDto>().BuildMock().BuildMockDbSet();
        _contextMock.Setup(x => x.Accounts.Update(account))
            .Returns(mock.Object.Update(account));
        var sut = new AccountsRepository(_contextMock.Object);

        //act
        sut.UpdateAccount(account);

        //assert
        mock.Verify(m => m.Update(account), Times.Once());
        _contextMock.Verify(m => m.SaveChanges(), Times.Once());
    }
}
