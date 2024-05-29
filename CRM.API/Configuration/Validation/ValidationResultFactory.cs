using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Results;

namespace CRM.API.Configuration.Validation
{
    public class ValidationResultFactory : IFluentValidationAutoValidationResultFactory
    {
        public IActionResult CreateActionResult(ActionExecutingContext context, ValidationProblemDetails validationProblemDetails)
        {
            return new UnprocessableEntityObjectResult(new
            {
                Title = ValidationSettings.ValidationErrorsTitle,
                Status = StatusCodes.Status422UnprocessableEntity,
                ValidationErrors = validationProblemDetails?.Errors
            }
            );
        }
    }
}
