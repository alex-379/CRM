namespace CRM.Business.Services.Constants.Logs;

public static class LeadsServiceLogs
{
    public const string Revoke = "Revoke refresh token";
    public const string SetLowerRegister = "Converted mail to lowercase";
    public const string AddLead = "Calling the repository method \"To create a new lead\"";
    public const string CompleteLead = "A new lead has been created with ID: {id}";
    public const string CheckLeadByMail = "Checking the lead is in the database with e-mail: {mail}";
    public const string CheckLeadById = "Checking the lead is in the database with ID: {id}";
    public const string CheckLeadPassword = "Verification of authentication data";
    public const string GetLeads = "Calling the repository method \"To get all leads\"";
    public const string GetLeadById = "Calling the repository method \"To get the lead with ID {id}\"";
    public const string UpdateLeadById = "Calling the repository method \"To update the lead with ID {id}\"";
    public const string UpdateLeadData = "Updating lead's data with ID: {id}";
    public const string UpdateLeadPassword = "Updating lead's password with ID: {id}";
    public const string UpdateLeadBirthDate = "Updating lead's birth date with ID: {id}";
    public const string UpdateLeadStatus = "Updating lead's status on {status} with ID: {id}";
    public const string SetIsDeletedLeadById = "The parameter \"IsDeleted=true\" is set for lead with ID {id}";
    public const string SendInfoToRabbitMq = "Sending info to RabbitMQ";

}
