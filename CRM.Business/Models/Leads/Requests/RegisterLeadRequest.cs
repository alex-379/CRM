namespace CRM.Business.Models.Leads.Requests;

public class RegisterLeadRequest
{
    public string Name { get; set; }
    public string Mail { get; set; }
    public string Phone { get; set; }
    public string Address { get; set; }
    public DateOnly BirthDate { get; set; }
    public string Password { get; set; }
}
