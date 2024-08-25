using Bogus;
using Microsoft.Extensions.DependencyInjection;
using TodoApp.Domain.TodoAggregate;
using TodoApp.Domain.TodoAggregate.ValueObjects;
using TodoApp.Infrastructure.Context;

namespace TodoApp.Infrastructure.Seeders;

public sealed class TodoSeeder
{
    private static AppDbContext GetAppDbContext(IServiceProvider services)
    {
        var scope = services.CreateScope();
        return scope.ServiceProvider.GetRequiredService<AppDbContext>();
    }

    public static IEnumerable<Todo> GenerateTodos()
    {
        var todos = new Faker<Todo>()
        .RuleFor(t => t.Id, f => TodoId.CreateUnique(null))
        .RuleFor(t => t.Title, f => f.Lorem.Text())
        .RuleFor(t => t.Description, f => f.Lorem.Text())
        .RuleFor(t => t.Identifier, f => Guid.NewGuid().ToString())
        .Generate(10);

        return todos;
    }

    public static void Seed(IServiceProvider service)
    {
        var db = GetAppDbContext(service);
        var todos = GenerateTodos();
        db.Todos.AddRange(todos);
        db.SaveChanges();
    }
}