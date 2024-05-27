﻿using CRM.Core.Constants;
using CRM.Core.Enums;
using CRM.Core.Exceptions;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using CRM.API.Configuration.Exceptions.Constants;

namespace CRM.API.Configuration.Filters;

[AttributeUsage(AttributeTargets.Method)]
public class AuthorizationFilterByLeadId : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var request = context.HttpContext.Request.Path.Value
                      ?? throw new BadRequestException(FiltersExceptions.BadRequestPath);
        var currentUserId = context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var requestId = context.HttpContext.Request.Path.Value.ToString()[$"{ControllersRoutes.LeadsController}/".Length..].Trim();
        if (!context.HttpContext.User.IsInRole(nameof(LeadStatus.Administrator))
            && currentUserId != requestId)
        {
            throw new UnauthorizedException();
        }
    }
}
