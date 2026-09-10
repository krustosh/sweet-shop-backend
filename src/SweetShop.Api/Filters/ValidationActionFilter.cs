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
    /// Executes the action filter asynchronously, validating action arguments and returning a BadRequest response if validation fails.
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

            var validator = context.HttpContext.RequestServices
                .GetService(validatorType);

            if (validator is null)
            {
                continue;
            }

            var validationContextType = typeof(ValidationContext<>)
                .MakeGenericType(argument.GetType());

            var validationContext = Activator.CreateInstance(
                validationContextType,
                argument);

            if (validationContext is null)
            {
                continue;
            }

            var validateAsyncMethod = validatorType.GetMethod(
                nameof(IValidator<object>.ValidateAsync),
                new[]
                {
                    validationContextType,
                    typeof(CancellationToken)
                });

            if (validateAsyncMethod is null)
            {
                continue;
            }

            var validationTask = (Task)validateAsyncMethod.Invoke(
                validator,
                new object[]
                {
                    validationContext,
                    context.HttpContext.RequestAborted
                })!;

            await validationTask.ConfigureAwait(false);

            var validationResult = validationTask
                .GetType()
                .GetProperty("Result")?
                .GetValue(validationTask)
                as FluentValidation.Results.ValidationResult;

            if (validationResult is null || validationResult.IsValid)
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
