﻿using CRM.API.Controllers;
using CRM.Business.Interfaces;
using CRM.Business.Models.Accounts.Requests;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CRM.API.Tests.Controllers;

public class AccountsControllerTest
{
    private readonly Mock<IAccountsService> _accountsServiceMock = new();

    [Fact]
    public async Task UpdateAccountStatusAsync_GuidAndUpdateAccountStatusRequestSent_NoContentResultReceived()
    {
        //arrange
        var id = new Guid();
        var updateAccountStatusRequest = new UpdateAccountStatusRequest();
        _accountsServiceMock.Setup(x => x.UpdateAccountStatusAsync(id, updateAccountStatusRequest));
        var sut = new AccountsController(_accountsServiceMock.Object, null);

        //act
        var actual = await sut.UpdateAccountStatusAsync(id, updateAccountStatusRequest);

        //assert
        actual.Should().BeOfType<NoContentResult>();
        _accountsServiceMock.Verify(m => m.UpdateAccountStatusAsync(id, updateAccountStatusRequest), Times.Once);
    }
}
