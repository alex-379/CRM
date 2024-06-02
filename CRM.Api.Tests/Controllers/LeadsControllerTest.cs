/*
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
    public void RegistrationLead_RegistrationLeadRequestSent_CreatedResultReceived()
    {
        //arrange
        var registrationLeadRequest = new RegisterLeadRequest();
        _leadsServiceMock.Setup(x => x.AddLead(registrationLeadRequest)).Returns(new Guid());
        var sut = new LeadsController(_leadsServiceMock.Object);

        //act
        var actual = sut.RegisterLead(registrationLeadRequest);

        //assert
        actual.Result.Should().BeOfType<CreatedResult>();
        _leadsServiceMock.Verify(m => m.AddLead(registrationLeadRequest), Times.Once);
    }

    [Fact]
    public void Login_LoginLeadRequestSent_OkResultReceived()
    {
        //arrange
        var loginLeadRequest = new LoginLeadRequest();
        _leadsServiceMock.Setup(x => x.LoginLead(loginLeadRequest)).Returns(new AuthenticatedResponse());
        var sut = new LeadsController(_leadsServiceMock.Object);

        //act
        var actual = sut.Login(loginLeadRequest);

        //assert
        actual.Result.Should().BeOfType<OkObjectResult>();
        _leadsServiceMock.Verify(m => m.LoginLead(loginLeadRequest), Times.Once);
    }

    [Fact]
    public void GetLeads_Called_OkResultReceived()
    {
        //arrange
        _leadsServiceMock.Setup(x => x.GetLeads()).Returns([]);
        var sut = new LeadsController(_leadsServiceMock.Object);

        //act
        var actual = sut.GetLeads();

        //assert
        actual.Result.Should().BeOfType<OkObjectResult>();
        _leadsServiceMock.Verify(m => m.GetLeads(), Times.Once);
    }

    [Fact]
    public void GetLeadById_GuidSent_OkResultReceived()
    {
        //arrange
        var id = new Guid();
        _leadsServiceMock.Setup(x => x.GetLeadById(id)).Returns(new LeadFullResponse());
        var sut = new LeadsController(_leadsServiceMock.Object);

        //act
        var actual = sut.GetLeadById(id);

        //assert
        actual.Result.Should().BeOfType<OkObjectResult>();
        _leadsServiceMock.Verify(m => m.GetLeadById(id), Times.Once);
    }

    [Fact]
    public void UpdateLeadData_GuidAndUpdateLeadDataRequestSent_NoContentResultReceived()
    {
        //arrange
        var id = new Guid();
        var updateLeadDataRequest = new UpdateLeadDataRequest();
        _leadsServiceMock.Setup(x => x.UpdateLead(id, updateLeadDataRequest));
        var sut = new LeadsController(_leadsServiceMock.Object);

        //act
        var actual = sut.UpdateLeadData(id, updateLeadDataRequest);

        //assert
        actual.Should().BeOfType<NoContentResult>();
        _leadsServiceMock.Verify(m => m.UpdateLead(id, updateLeadDataRequest), Times.Once);
    }


    [Fact]
    public void DeleteLeadById_GuidSent_NoContentResultReceived()
    {
        //arrange
        var id = new Guid();
        _leadsServiceMock.Setup(x => x.DeleteLeadById(id));
        var sut = new LeadsController(_leadsServiceMock.Object);

        //act
        var actual = sut.DeleteLeadById(id);

        //assert
        actual.Should().BeOfType<NoContentResult>();
        _leadsServiceMock.Verify(m => m.DeleteLeadById(id), Times.Once);
    }

    [Fact]
    public void UpdateLeadPassword_GuidAndUpdateLeadDataRequestSent_NoContentResultReceived()
    {
        //arrange
        var id = new Guid();
        var updateLeadPasswordRequest = new UpdateLeadPasswordRequest();
        _leadsServiceMock.Setup(x => x.UpdateLeadPassword(id, updateLeadPasswordRequest));
        var sut = new LeadsController(_leadsServiceMock.Object);

        //act
        var actual = sut.UpdateLeadPassword(id, updateLeadPasswordRequest);

        //assert
        actual.Should().BeOfType<NoContentResult>();
        _leadsServiceMock.Verify(m => m.UpdateLeadPassword(id, updateLeadPasswordRequest), Times.Once);
    }

    [Fact]
    public void UpdateLeadStatus_GuidAndUpdateLeadStatusRequestSent_NoContentResultReceived()
    {
        //arrange
        var id = new Guid();
        var updateLeadStatusRequest = new UpdateLeadStatusRequest();
        _leadsServiceMock.Setup(x => x.UpdateLeadStatus(id, updateLeadStatusRequest));
        var sut = new LeadsController(_leadsServiceMock.Object);

        //act
        var actual = sut.UpdateLeadStatus(id, updateLeadStatusRequest);

        //assert
        actual.Should().BeOfType<NoContentResult>();
        _leadsServiceMock.Verify(m => m.UpdateLeadStatus(id, updateLeadStatusRequest), Times.Once);
    }

    [Fact]
    public void UpdateLeadBirthDate_GuidAndUpdateLeadBirthDateRequestSent_NoContentResultReceived()
    {
        //arrange
        var id = new Guid();
        var updateLeadBirthDateRequest = new UpdateLeadBirthDateRequest();
        _leadsServiceMock.Setup(x => x.UpdateLeadBirthDate(id, updateLeadBirthDateRequest));
        var sut = new LeadsController(_leadsServiceMock.Object);

        //act
        var actual = sut.UpdateLeadBirthDate(id, updateLeadBirthDateRequest);

        //assert
        actual.Should().BeOfType<NoContentResult>();
        _leadsServiceMock.Verify(m => m.UpdateLeadBirthDate(id, updateLeadBirthDateRequest), Times.Once);
    }
}
*/
