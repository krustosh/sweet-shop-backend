using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SweetShop.Api.Common.Models;

namespace SweetShop.Api.Filters;

/// <summary>
/// Represents an action filter that performs validation on action arguments using FluentValidation.
/// </summary>
public sealed class ValidationActionFilter : IAsyncActionFilter
{
    /// <summary>
    /// Executes the action filter asynchronously, validating action arguments and returning
    /// a bad request response if validation fails.
    /// If validation passes, the action execution continues to the next filter or action.
    /// </summary>
    /// <param name="context">The action executing context.</param>
    /// <param name="next">The next delegate in the pipeline.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        foreach (var argument in context.ActionArguments.Values)
        {
            if (argument is null)
            {
                continue;
            }

            var validatorType = typeof(IValidator<>)
                .MakeGenericType(argument.GetType());

            if (context.HttpContext.RequestServices.GetService(validatorType)
                is not IValidator validator)
            {
                continue;
            }

            var validationContext = new ValidationContext<object>(
                argument);

            var validationResult = await validator.ValidateAsync(
                validationContext,
                context.HttpContext.RequestAborted);

            if (validationResult.IsValid)
            {
                continue;
            }

            var details = validationResult.Errors
                .Select(error => new ApiErrorDetail(
                    error.PropertyName,
                    error.ErrorMessage))
                .ToArray();

            context.Result = new BadRequestObjectResult(
                new ApiErrorResponse(
                    new ApiError(
                        "VALIDATION_ERROR",
                        "One or more validation errors occurred.",
                        details)));

            return;
        }

        await next();
    }
}