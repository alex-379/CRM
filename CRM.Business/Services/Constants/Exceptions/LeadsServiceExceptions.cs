namespace CRM.Business.Services.Constants.Exceptions;

public static class LeadsServiceExceptions
{
    public const string NotFoundException = "Lead with Id: {0} not found";
    public const string ConflictException = "E-mail already exists";
    public const string ConflictExceptionIsDeleted = "E-mail already exists and was deleted. Contact the Administrator.";
}
