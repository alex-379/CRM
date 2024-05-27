using System.Security.Claims;
using CRM.API.Controllers.ExceptionsConstants;
using CRM.Core.Exceptions;

namespace CRM.API.Controllers;

public static class OperationsWithUsers
{
    public static Guid GetCurrentUserFromClaims(ClaimsPrincipal claimsPrincipal)
    {
        var currentUserId = new Guid(claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier)
                                     ?? throw new NotFoundException(ControllersExceptions.ClaimNotFound));

        return currentUserId;
    }
}