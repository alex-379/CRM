namespace CRM.Core.Constants.Exceptions.Business;

public static class LeadsServiceExceptions
{
    public const string NotFoundException = "Lead with Id: {0} not found";
    public const string NotFoundExceptionMail = "Lead with mail: {0} not found";
    public const string ConflictException = "E-mail already exists";
}
