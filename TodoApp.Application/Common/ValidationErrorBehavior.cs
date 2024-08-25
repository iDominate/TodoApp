using ErrorOr;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace TodoApp.Application.Common;

public sealed class ValidationErrorBehavior<TRequest, TResponse>(IValidator<TRequest>? validator = null) : IPipelineBehavior<TRequest, TResponse>
where TRequest : IRequest<TResponse>
where TResponse : IErrorOr
{

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var result = await next();
        if (validator is null)
        {
            return result;
        }
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (validationResult.IsValid)
        {
            return result;
        }

        var errors = validationResult.Errors.ConvertAll(e => Error.Validation(e.PropertyName, e.ErrorMessage));
        return (dynamic)errors;
    }
}