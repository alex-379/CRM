namespace CRM.Business.Models.Tokens.Responses;

public class AuthenticatedResponse
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
}