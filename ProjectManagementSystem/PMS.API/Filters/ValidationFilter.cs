using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace PMS.API.Filters;

public class ValidationFilter : IAsyncActionFilter
{
    private readonly IServiceProvider _serviceProvider;

    public ValidationFilter(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        foreach (var argument in context.ActionArguments.Values)
        {
            if (argument is null)
                continue;

            var validatorType = typeof(IValidator<>)
                .MakeGenericType(argument.GetType());

            var validator = _serviceProvider.GetService(validatorType);

            if (validator is null)
                continue;

            var validationContext = new ValidationContext<object>(argument);

            var result = await ((IValidator)validator)
                .ValidateAsync(validationContext);

            if (!result.IsValid)
            {
                context.Result = new BadRequestObjectResult(
                    new
                    {
                        Errors = result.Errors
                            .Select(e => new
                            {
                                e.PropertyName,
                                e.ErrorMessage
                            })
                    });

                return;
            }
        }

        await next();
    }
}
