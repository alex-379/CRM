using CRM.Core.Dtos;
using CRM.Core.Enums;

namespace CRM.DataLayer.Tests;

public static class TestsData
{
    public static LeadDto GetFakeLeadDto() =>
        new()
        {
            Name = "testuser03",
            Mail = "test03@test.ru",
        };

    public static List<LeadDto> GetFakeLeadDtoList() =>
        [
        new()
        {
            Id = new Guid("4e7918d2-fdcd-4316-97bb-565f8f4a0566"),
            Name = "TestLead01",
            Mail = "test01@test.test",
            Phone = "+78888888888",
            Address = "TestAddress01",
            BirthDate ="01.01.2000",
        },
        new()
        {
            Id = new Guid("78fa8b9b-91fa-4e94-9a35-33d356d92890"),
            Name = "TestLead02",
            Mail = "test02@test.test",
            Phone = "+79999999999",
            Address = "TestAddress02",
            BirthDate ="01.01.1999",
        }
        ];

    public static AccountDto GetFakeAccountDto() =>
        new()
        {
            Currency = Currency.Rub,
            Status = AccountStatus.Active,
            Lead = new()
            {
                Name = "testuser03",
                Mail = "test03@test.ru",
            }
        };
}  
