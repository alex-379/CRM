﻿using CRM.Business.Models.Accounts.Requests;
using CRM.Business.Models.Leads.Requests;
using CRM.Business.Models.Leads.Responses;
using CRM.Core.Dtos;
using CRM.Core.Enums;

namespace CRM.Business.Tests;

public static class TestsData
{
    public static RegistrationLeadRequest GetFakeRegistrationLeadRequest() =>
        new()
        {
            Name = "TestLead01",
            Mail = "test01@test.test",
            Phone = "+78888888888",
            Address = "TestAddress01",
            BirthDate = new DateOnly(2000, 1, 1),
            Password = "Password",
        };

    public static LoginLeadRequest GetFakeLoginLeadRequest() =>
        new()
        {
            Mail = "test01@test.test",
            Password = "Password",
        };

    public static LeadDto GetFakeLeadDto() =>
        new()
        {
            Id = new Guid("865179f5-1adb-4788-9fed-b9a57ce9abf8"),
            Name = "TestLead01",
            Mail = "test01@test.test",
            Phone = "+78888888888",
            Address = "TestAddress01",
            BirthDate = new DateOnly(2000, 1, 1),
            Password = "6D208A964DBEB743626BAAA8C1048EF31C882764F74F89E368728721CA6A922042C9E4D6D50D69D1A707B45FF680674EFC299BD93AF48DEAC0B5F862EB080728",
            Salt = "6D208A964DBEB743626BAAA8C1048EF31C882764F74F89E368728721CA6A922042C9E4D6D50D69D1A707B45FF680674EFC299BD93AF48DEAC0B5F862EB080729",
            Status = LeadStatus.Regular,
            Accounts =
            [
                new()
                {
                    Currency = Currency.Eur,
                },
                new()
                {
                    Currency = Currency.Usd,
                }
            ]
        };

    public static LeadResponse GetFakeLeadResponse() =>
        new()
        {
            Name = "TestLead01",
            Mail = "test01@test.test",
            Phone = "+78888888888",
            Address = "TestAddress01",
            BirthDate = new DateOnly(2000, 1, 1),
            Status = LeadStatus.Regular,
            Accounts =
        [
            new()
            {
                Currency = Currency.Eur,
            },
            new()
            {
                Currency = Currency.Usd,
            }
        ]
        };

    public static UpdateLeadDataRequest GetFakeUpdateLeadDataRequest() =>
        new()
        {
            Name = "TestLead10",
            Phone = "+76666666666",
            Address = "TestAddress10",
        };

    public static UpdateLeadPasswordRequest GetFakeUpdateLeadPasswordRequest() =>
        new()
        {
            Password = "PassPass0012",
        };

    public static UpdateLeadMailRequest GetFakeUpdateLeadMailRequest() =>
        new()
        {
            Mail = "test508@test.ru",
        };

    public static UpdateLeadStatusRequest GetFakeUpdateLeadStatusRequest() =>
        new()
        {
            Status = LeadStatus.Vip,
        };

    public static UpdateLeadBirthDateRequest GetFakeUpdateLeadBirthDateRequest() =>
        new()
        {
            BirthDate = new DateOnly(1990, 11, 20),
        };

    public static RegistrationAccountRequest GetFakeRegistrationAccountRequest() =>
        new()
        {
            Currency = Currency.Usd,
        };

    public static UpdateAccountStatusRequest GetFakeUpdateAccountStatusRequest() =>
        new()
        {
            Status = AccountStatus.Block,
        };
}