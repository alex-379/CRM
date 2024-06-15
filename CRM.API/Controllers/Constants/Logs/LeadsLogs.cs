namespace CRM.API.Controllers.Constants.Logs;

public static class LeadsLogs
{
    public const string RegisterLead = "Creating lead with mail: {request.Mail}";
    public const string Login = "Lead authentication";
    public const string GetLeads = "Getting all leads at the database";
    public const string GetLeadById = "Getting the lead by ID: {id}";
    public const string UpdateLeadData = "Updating lead data with ID: {id}";
    public const string DeleteLeadById = "Deleting lead with ID: {id}";
    public const string UpdateLeadPassword = "Updating lead password with ID: {id}";
    public const string UpdateLeadStatus = "Updating lead status with ID: {id}";
    public const string UpdateLeadBirthDate = "Updating lead birth date with ID: {id}";
    public const string GetAuthorizedLead = "Getting the current authorized lead";
}
