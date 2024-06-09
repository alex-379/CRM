using System.Security.Claims;
using CRM.API.Controllers.Constants;
using CRM.Core.Exceptions;

namespace CRM.API.Controllers;

public static class InformationFromClaims
{
    public static Guid GetCurrentLeadFromClaims(ClaimsPrincipal claimsPrincipal)
    {
        var currentLeadId = new Guid(claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier)
                                     ?? throw new NotFoundException(Exceptions.ClaimNotFound));

        return currentLeadId;
    }
}