using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Results;

namespace CRM.API.Configuration
{
    public class ValidationResultFactory : IFluentValidationAutoValidationResultFactory
    {
        public IActionResult CreateActionResult(ActionExecutingContext context, ValidationProblemDetails validationProblemDetails)
        {
            return new UnprocessableEntityObjectResult(new
            {
                Title = "Ошибки валидации",
                Status = StatusCodes.Status422UnprocessableEntity,
                ValidationErrors = validationProblemDetails?.Errors
            }
            );
        }
    }
}
