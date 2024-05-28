using CRM.Core.Dtos;
using CRM.DataLayer.Repositories;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;

namespace CRM.DataLayer.Tests.Repositories;

public class AccountsRepositoryTest
{
    private static readonly DbContextOptions<CrmContext> _options = new();
    private readonly Mock<CrmContext> _contextMock = new(_options);

    [Fact]
    public void AddAccount_AccountDtoSent_GuidReceived()
    {
        //arrange
        var accounts = new List<AccountDto>();
        var account = TestData.GetFakeAccountDto();
        var mock = accounts.BuildMock().BuildMockDbSet();
        _contextMock.Setup(x => x.Accounts.Add(account))
            .Returns(mock.Object.Add(account))
            .Callback<AccountDto>(accounts.Add);
        var sut = new AccountsRepository(_contextMock.Object);

        //act
        var actual = sut.AddAccount(account);

        //assert
        Assert.Single(accounts);
        mock.Verify(m => m.Add(account), Times.Once());
        _contextMock.Verify(m => m.SaveChanges(), Times.Once());
    }

    [Fact]
    public void UpdateAccount_AccountDtoSent_NoErrorsReceived()
    {
        //arrange
        var account = TestData.GetFakeAccountDto();
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
