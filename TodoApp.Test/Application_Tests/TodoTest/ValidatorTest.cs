using FluentAssertions;
using TodoApp.Application.TodoCQRS.Commands;
using TodoApp.Application.TodoCQRS.Queries;
using TodoApp.Application.TodoCQRS.Validators;

namespace TodoApp.Test.Application_Tests.TodoTest;

public sealed class ValidatorTest
{
    [Fact]
    public void TestCreateTodoValidator_ReturnsErrorOnEmptyTodo()
    {
        // Arrange
        var validator = new CreateTodoValidator();

        // Act
        var result = validator.Validate(new CreateTodoCommand("Test", "Test"
        ));

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(2);
        result.Errors.Should().Contain(x => x.PropertyName == "Todo.Title");
        result.Errors.Should().Contain(x => x.PropertyName == "Todo.Description");
    }

    [Fact]
    public void TestCreateTodoValidator_ValidOnProperTodo()
    {
        // Arrange
        var validator = new CreateTodoValidator();
        // Act
        var result = validator.Validate(new CreateTodoCommand("Test", "Test"
        ));
        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void TestUpdateTodoValidator_ReturnsErrorOnEmptyTodo()
    {
        // Arrange
        var validator = new UpdateTodoValidator();
        // Act
        var result = validator.Validate(new UpdateTodoCommand(Guid.Empty.ToString(), "", ""
        ));
        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(3);
        result.Errors.Should().Contain(x => x.PropertyName == "Identifier");
        result.Errors.Should().Contain(x => x.PropertyName == "NewTodo.Title");
        result.Errors.Should().Contain(x => x.PropertyName == "NewTodo.Description");
    }

    [Fact]
    public void TestUpdateTodoValidator_ValidOnProperTodo()
    {
        // Arrange
        var validator = new UpdateTodoValidator();
        // Act
        var result = validator.Validate(new UpdateTodoCommand(Guid.NewGuid().ToString(), "Test", "Test"
        ));
        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void TestDeleteTodoValidator_ReturnsErrorOnEmptyGuid()
    {
        // Arrange
        var validator = new DeleteTodoValidator();
        // Act
        var result = validator.Validate(new DeleteTodoCommand(Guid.Empty.ToString()));
        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
        result.Errors.Should().Contain(x => x.PropertyName == "Identifier");
    }

    [Fact]
    public void TestDeleteTodoValidator_ValidOnProperGuid()
    {
        // Arrange
        var validator = new DeleteTodoValidator();
        // Act
        var result = validator.Validate(new DeleteTodoCommand(Guid.NewGuid().ToString()));
        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void TestGetTodoValidator_ReturnsErrorOnEmptyGuid()
    {
        // Arrange
        var validator = new GetTodoValidator();
        // Act
        var result = validator.Validate(new GetTodoByIdQuery(Guid.Empty.ToString()));
        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
        result.Errors.Should().Contain(x => x.PropertyName == "Identifier");
    }

    [Fact]
    public void TestGetTodoValidator_ValidOnProperGuid()
    {
        // Arrange
        var validator = new GetTodoValidator();
        // Act
        var result = validator.Validate(new GetTodoByIdQuery(Guid.NewGuid().ToString()));
        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }
}