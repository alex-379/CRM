namespace CRM.Business.Models.Leads.Requests;

public class RegisterLeadRequest
{
    public string Name { get; init; }
    public string Mail { get; init; }
    public string Phone { get; init; }
    public string Address { get; init; }
    public DateOnly BirthDate { get; init; }
    public string Password { get; init; }
}
