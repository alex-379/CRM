namespace CRM.Business.Models.Leads.Requests;

public class Login2FaLeadRequest
{
    public Guid Token { get; init; }
    public int Code { get; init; }
}