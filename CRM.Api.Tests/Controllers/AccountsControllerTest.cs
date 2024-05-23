﻿using CRM.API.Controllers;
using CRM.Business.Interfaces;
using CRM.Business.Models.Accounts.Requests;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CRM.API.Tests.Controllers;

public class AccountsControllerTest
{
    private readonly Mock<IAccountsService> _accountsServiceMock;

    public AccountsControllerTest()
    {
        _accountsServiceMock = new Mock<IAccountsService>();
    }

    [Fact]
    public void UpdateAccountStatus_GuidAndUpdateAccountStatusRequestSent_NoContentResultReceived()
    {
        //arrange
        var id = new Guid();
        var updateAccountStatusRequest = new UpdateAccountStatusRequest();
        _accountsServiceMock.Setup(x => x.UpdateAccountStatus(id, updateAccountStatusRequest));
        var sut = new AccountsController(_accountsServiceMock.Object);

        //act
        var actual = sut.UpdateAccountStatus(id, updateAccountStatusRequest);

        //assert
        actual.Should().BeOfType<NoContentResult>();
        _accountsServiceMock.Verify(m => m.UpdateAccountStatus(id, updateAccountStatusRequest), Times.Once);
    }
}