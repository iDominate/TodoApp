using System.Reflection;
using ErrorOr;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TodoApp.Application.Auth.Commands;
using TodoApp.Application.Auth.Validators;
using TodoApp.Application.Common;
using TodoApp.Application.TodoCQRS.Commands;
using TodoApp.Application.TodoCQRS.Queries;
using TodoApp.Application.TodoCQRS.Validators;
using TodoApp.Domain.TodoAggregate;

namespace TodoApp.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {

        #region Register Validators
        services.AddScoped<IValidator<CreateTodoCommand>, CreateTodoValidator>();
        services.AddScoped<IValidator<UpdateTodoCommand>, UpdateTodoValidator>();
        services.AddScoped<IValidator<DeleteTodoCommand>, DeleteTodoValidator>();
        services.AddScoped<IValidator<GetTodoByIdQuery>, GetTodoValidator>();
        services.AddScoped<IValidator<LoginCommand>, LoginCommandValidator>();
        services.AddScoped<IValidator<RegisterUserCommand>, RegisterCommadValidator>();
        services.AddScoped<IValidator<GenerateTokenAsyncCommand>, GenerateTokenAsyncCommandValidator>();
        #endregion

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationErrorBehavior<,>));
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        return services;
    }
}