﻿namespace CRM.API.Configuration.Exceptions;

public class ConfigurationMissingException(string message="fault configuration") : Exception(message)
{
}
