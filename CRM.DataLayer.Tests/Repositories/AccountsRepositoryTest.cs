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
    public async Task AddAccountAsync_AccountDtoSent_GuidReceived()
    {
        //arrange
        var accounts = new List<AccountDto>();
        var account = TestData.GetFakeAccountDto();
        var mock = accounts.BuildMock().BuildMockDbSet();
        _contextMock.Setup(x => x.Accounts)
            .Returns(mock.Object);
        _contextMock.Setup(x => x.SaveChangesAsync(default))
            .Callback<CancellationToken>(_ => accounts.Add(account));
        var sut = new AccountsRepository(_contextMock.Object);

        //act
        var actual = await sut.AddAccountAsync(account);

        //assert
        Assert.Matches(TestData.RegexGuid, actual.ToString());
        Assert.Single(accounts);
        mock.Verify(m => m.AddAsync(account,default), Times.Once());
        _contextMock.Verify(m => m.SaveChangesAsync(default), Times.Once());
    }
    
    [Fact]
    public async Task GetAccountByIdAsync_GuidSent_AccountDtoReceived()
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
    public async Task UpdateAccountAsync_AccountDtoSent_NoErrorsReceived()
    {
        //arrange
        var account = TestData.GetFakeAccountDto();
        var mock = Enumerable.Empty<AccountDto>().BuildMock().BuildMockDbSet();
        _contextMock.Setup(x => x.Accounts)
            .Returns(mock.Object);
        var sut = new AccountsRepository(_contextMock.Object);

        //act
        await sut.UpdateAccountAsync(account);

        //assert
        mock.Verify(m => m.Update(account), Times.Once());
        _contextMock.Verify(m => m.SaveChangesAsync(default), Times.Once());
    }
}
