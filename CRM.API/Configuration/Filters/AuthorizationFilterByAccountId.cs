using System.Security.Claims;
using System.Text;
using System.Text.Json;
using CRM.Business.Interfaces;
using CRM.Business.Models.Accounts.Responses;
using CRM.Business.Models.Transactions.Requests;
using CRM.Core.Enums;
using CRM.Core.Exceptions;
using Elasticsearch.Net.Specification.SnapshotApi;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CRM.API.Configuration.Filters;

[AttributeUsage(AttributeTargets.Method)]
public class AuthorizationFilterByAccountId : Attribute, IAsyncAuthorizationFilter
{
    class TestRequest
    {
        public Guid accountId { get; }
    }
    
    private struct RequestData
    {
        private Guid accountId;
    }
    
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var currentUserId = context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var requestBody = await new StreamReader(context.HttpContext.Request.Body).ReadToEndAsync();
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var accountId = JsonSerializer.Deserialize<TransactionRequest>(requestBody, options);
        
        
        using (var reader = new StreamReader(context.HttpContext.Request.Body))
        {
            var postData = await reader.ReadToEndAsync();

            
            // Process the post data as needed
            // ...
        }
        
        /*var requestBody = context.HttpContext.Request.Form;
        using (var reader = new StreamReader(requestBody))
        {
            var requestContent = reader.ReadToEnd();
            //dynamic requestData = JsonConvert.DeserializeObject(requestContent);

            // Извлекаем id из объекта
            //var requestId = requestData.id;

            // Здесь вы можете использовать requestId для проверки авторизации
            // ...
        }*/
        
        
        var accountsService = context.HttpContext.RequestServices.GetRequiredService<IAccountsService>();
        // if (string.IsNullOrEmpty(requestId))
        // {
        //     throw new UnauthorizedException();
        // }
        //
        // var account = await accountsService.GetAccountByIdAsync<AccountForAuthorizationFilterResponse>(new Guid(requestId));
        // if (!context.HttpContext.User.IsInRole(nameof(LeadStatus.Administrator))
        //     && currentUserId != account.LeadId.ToString())
        // {
        //     throw new UnauthorizedException();
        // }
    }
}
