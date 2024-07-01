using CRM.API.Controllers;
using CRM.Business.Interfaces;
using CRM.Business.Models.Leads.Requests;
using CRM.Business.Models.Leads.Responses;
using CRM.Business.Models.Tokens.Responses;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CRM.API.Tests.Controllers;

public class LeadsControllerTest
{
    private readonly Mock<ILeadsService> _leadsServiceMock = new();

    [Fact]
    public async Task RegistrationLeadAsync_RegistrationLeadRequestSent_CreatedResultReceived()
    {
        //arrange
        var registrationLeadRequest = new RegisterLeadRequest();
        _leadsServiceMock.Setup(x => x.AddLeadAsync(registrationLeadRequest)).ReturnsAsync((new Guid(), new Guid()));
        var sut = new LeadsController(_leadsServiceMock.Object);

        //act
        var actual = await sut.RegisterLeadAsync(registrationLeadRequest);

        //assert
        actual.Result.Should().BeOfType<CreatedResult>();
        _leadsServiceMock.Verify(m => m.AddLeadAsync(registrationLeadRequest), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_LoginLeadRequestSent_OkResultReceived()
    {
        //arrange
        var loginLeadRequest = new LoginLeadRequest();
        _leadsServiceMock.Setup(x => x.LoginLeadAsync(loginLeadRequest)).ReturnsAsync(new Guid());
        var sut = new LeadsController(_leadsServiceMock.Object);

        //act
        var actual = await sut.LoginAsync(loginLeadRequest);

        //assert
        actual.Result.Should().BeOfType<OkObjectResult>();
        _leadsServiceMock.Verify(m => m.LoginLeadAsync(loginLeadRequest), Times.Once);
    }

    [Fact]
    public async Task GetLeadsAsync_Called_OkResultReceived()
    {
        //arrange
        _leadsServiceMock.Setup(x => x.GetLeadsAsync()).ReturnsAsync([]);
        var sut = new LeadsController(_leadsServiceMock.Object);

        //act
        var actual = await sut.GetLeadsAsync();

        //assert
        actual.Result.Should().BeOfType<OkObjectResult>();
        _leadsServiceMock.Verify(m => m.GetLeadsAsync(), Times.Once);
    }

    [Fact]
    public async Task GetLeadByIdAsync_GuidSent_OkResultReceived()
    {
        //arrange
        var id = new Guid();
        _leadsServiceMock.Setup(x => x.GetLeadByIdAsync(id)).ReturnsAsync(new LeadFullResponse());
        var sut = new LeadsController(_leadsServiceMock.Object);

        //act
        var actual = await sut.GetLeadByIdAsync(id);

        //assert
        actual.Result.Should().BeOfType<OkObjectResult>();
        _leadsServiceMock.Verify(m => m.GetLeadByIdAsync(id), Times.Once);
    }

    [Fact]
    public async Task UpdateLeadDataAsync_GuidAndUpdateLeadDataRequestSent_NoContentResultReceived()
    {
        //arrange
        var id = new Guid();
        var updateLeadDataRequest = new UpdateLeadDataRequest();
        _leadsServiceMock.Setup(x => x.UpdateLeadAsync(id, updateLeadDataRequest));
        var sut = new LeadsController(_leadsServiceMock.Object);

        //act
        var actual = await sut.UpdateLeadDataAsync(id, updateLeadDataRequest);

        //assert
        actual.Should().BeOfType<NoContentResult>();
        _leadsServiceMock.Verify(m => m.UpdateLeadAsync(id, updateLeadDataRequest), Times.Once);
    }


    [Fact]
    public async Task DeleteLeadByIdAsync_GuidSent_NoContentResultReceived()
    {
        //arrange
        var id = new Guid();
        _leadsServiceMock.Setup(x => x.DeleteLeadByIdAsync(id));
        var sut = new LeadsController(_leadsServiceMock.Object);

        //act
        var actual = await sut.DeleteLeadByIdAsync(id);

        //assert
        actual.Should().BeOfType<NoContentResult>();
        _leadsServiceMock.Verify(m => m.DeleteLeadByIdAsync(id), Times.Once);
    }

    [Fact]
    public async Task UpdateLeadPasswordAsync_GuidAndUpdateLeadDataRequestSent_NoContentResultReceived()
    {
        //arrange
        var id = new Guid();
        var updateLeadPasswordRequest = new UpdateLeadPasswordRequest();
        _leadsServiceMock.Setup(x => x.UpdateLeadPasswordAsync(id, updateLeadPasswordRequest));
        var sut = new LeadsController(_leadsServiceMock.Object);

        //act
        var actual = await sut.UpdateLeadPasswordAsync(id, updateLeadPasswordRequest);

        //assert
        actual.Should().BeOfType<NoContentResult>();
        _leadsServiceMock.Verify(m => m.UpdateLeadPasswordAsync(id, updateLeadPasswordRequest), Times.Once);
    }

    [Fact]
    public async Task UpdateLeadStatusAsync_GuidAndUpdateLeadStatusRequestSent_NoContentResultReceived()
    {
        //arrange
        var id = new Guid();
        var updateLeadStatusRequest = new UpdateLeadStatusRequest();
        _leadsServiceMock.Setup(x => x.UpdateLeadStatusAsync(id, updateLeadStatusRequest));
        var sut = new LeadsController(_leadsServiceMock.Object);

        //act
        var actual = await sut.UpdateLeadStatusAsync(id, updateLeadStatusRequest);

        //assert
        actual.Should().BeOfType<NoContentResult>();
        _leadsServiceMock.Verify(m => m.UpdateLeadStatusAsync(id, updateLeadStatusRequest), Times.Once);
    }

    [Fact]
    public async Task UpdateLeadBirthDateAsync_GuidAndUpdateLeadBirthDateRequestSent_NoContentResultReceived()
    {
        //arrange
        var id = new Guid();
        var updateLeadBirthDateRequest = new UpdateLeadBirthDateRequest();
        _leadsServiceMock.Setup(x => x.UpdateLeadBirthDateAsync(id, updateLeadBirthDateRequest));
        var sut = new LeadsController(_leadsServiceMock.Object);

        //act
        var actual = await sut.UpdateLeadBirthDateAsync(id, updateLeadBirthDateRequest);

        //assert
        actual.Should().BeOfType<NoContentResult>();
        _leadsServiceMock.Verify(m => m.UpdateLeadBirthDateAsync(id, updateLeadBirthDateRequest), Times.Once);
    }
}
