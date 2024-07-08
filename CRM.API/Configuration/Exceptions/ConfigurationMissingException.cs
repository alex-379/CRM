using CRM.Core.Exceptions;

namespace CRM.API.Configuration.Exceptions;

public class ConfigurationMissingException(string message=DefaultMessages.ConfigurationError) : Exception(message)
{
}
