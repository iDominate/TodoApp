using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TodoApp.Domain.TodoAggregate;
using TodoApp.Domain.TodoAggregate.ValueObjects;

namespace TodoApp.Infrastructure.Configs;

public sealed class TodoConfig : IEntityTypeConfiguration<Todo>
{
    public void Configure(EntityTypeBuilder<Todo> builder)
    {
        builder.Property(t => t.Id)
        .ValueGeneratedNever()
        .HasConversion(t => t.Value, t => TodoId.CreateUnique(t));
        builder.Property(t => t.Title).HasMaxLength(50).IsRequired();
        builder.Property(t => t.Description).HasMaxLength(250).IsRequired();
        builder.Property(t => t.Identifier).ValueGeneratedNever().IsRequired();
    }
}