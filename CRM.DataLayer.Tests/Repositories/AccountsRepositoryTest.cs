using AutoFixture;
using CRM.Core.Dtos;
using CRM.Core.Fixture;
using CRM.DataLayer.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;

namespace CRM.DataLayer.Tests.Repositories;

public class AccountsRepositoryTest
{
    private readonly Mock<CrmContext> _contextMock = new(new DbContextOptions<CrmContext>());
    private readonly CustomFixture _customFixture = new();

    [Fact]
    public async Task AddAccountAsync_AccountDtoSent_GuidReceived()
    {
        //arrange
        DatabaseConnection.SetDatabase(_contextMock);
        var fixture = _customFixture.GetFixture();
        var accounts = new List<AccountDto>();
        var account = fixture.Create<AccountDto>();
        var mock = accounts.BuildMock().BuildMockDbSet();
        _contextMock.Setup(x => x.Accounts)
            .Returns(mock.Object);
        _contextMock.Setup(x => x.SaveChangesAsync(default))
            .Callback<CancellationToken>(_ => accounts.Add(account));
        var sut = new AccountsRepository(_contextMock.Object);

        //act
        var actual = await sut.AddAccountAsync(account);

        //assert
        Assert.Matches(CustomFixture.RegexGuid, actual.ToString());
        Assert.Single(accounts);
        mock.Verify(m => m.AddAsync(account,default), Times.Once());
        _contextMock.Verify(m => m.SaveChangesAsync(default), Times.Once());
    }
    
    [Fact]
    public async Task AddAccountAsync_NullSent_NullReferenceExceptionErrorReceived()
    {
        //arrange
        DatabaseConnection.SetDatabase(_contextMock);
        var mock = Enumerable.Empty<AccountDto>().BuildMock().BuildMockDbSet();
        _contextMock.Setup(x => x.Accounts)
            .Returns(mock.Object);
        var sut = new AccountsRepository(_contextMock.Object);

        //act
        var act = async () => await sut.AddAccountAsync(null);

        //assert
        await act.Should().ThrowAsync<NullReferenceException>();
    }
    
    [Fact]
    public async Task GetAccountByIdAsync_GuidSent_AccountDtoReceived()
    {
        //arrange
        DatabaseConnection.SetDatabase(_contextMock);
        var fixture = _customFixture.GetFixture();
        var accounts = fixture.Create<List<AccountDto>>();
        var expected = accounts.FirstOrDefault()!.Id;
        var mock = accounts.BuildMock().BuildMockDbSet();
        _contextMock.Setup(x => x.Accounts)
            .Returns(mock.Object);
        var sut = new AccountsRepository(_contextMock.Object);

        //act
        var actual = await sut.GetAccountByIdAsync(expected);

        //assert
        Assert.NotNull(actual);
        Assert.Equal(expected, actual.Id);
    }
    
    [Fact]
    public async Task UpdateAccountAsync_AccountDtoSent_NoErrorsReceived()
    {
        //arrange
        DatabaseConnection.SetDatabase(_contextMock);
        var fixture = _customFixture.GetFixture();
        var account = fixture.Create<AccountDto>();
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
    
    [Fact]
    public async Task UpdateAccountAsync_NullSent_NullReferenceExceptionErrorReceived()
    {
        //arrange
        DatabaseConnection.SetDatabase(_contextMock);
        var mock = Enumerable.Empty<AccountDto>().BuildMock().BuildMockDbSet();
        _contextMock.Setup(x => x.Accounts)
            .Returns(mock.Object);
        var sut = new AccountsRepository(_contextMock.Object);

        //act
        var act = async () => await sut.UpdateAccountAsync(null);

        //assert
        await act.Should().ThrowAsync<NullReferenceException>();
    }
}
