using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Moq;

namespace CRM.DataLayer.Tests.Repositories;

public static class DatabaseConnection
{
    public static void SetDatabase<T>(Mock<T> contextMock)
        where T:DbContext
    {
        var mockDatabase = new Mock<DatabaseFacade>(contextMock.Object);
        mockDatabase.Setup(db => db.CanConnect()).Returns(true);
        contextMock.Setup(c => c.Database).Returns(mockDatabase.Object);
    }
}