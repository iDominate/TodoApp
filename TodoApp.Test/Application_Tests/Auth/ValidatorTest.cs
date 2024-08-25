using FluentAssertions;
using TodoApp.Application.Auth.Commands;
using TodoApp.Application.Auth.Validators;

namespace TodoApp.Test.Application_Tests.Auth;


public sealed class ValidatorTest
{
    [Fact]
    public void TestGenerateTokenAsyncCommandValidator_NotValidOnError()
    {
        // Arrange
        var validator = new GenerateTokenAsyncCommandValidator();
        var command = new GenerateTokenAsyncCommand("");

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
        result.Errors.Should().Contain(e => e.PropertyName == "Token");
    }
    [Fact]
    public void TestGenerateTokenAsyncCommandValidator_Valid()
    {
        // Arrange
        var validator = new GenerateTokenAsyncCommandValidator();
        var command = new GenerateTokenAsyncCommand("123");

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void TestLoginCommandValidator_NotValidOnError()
    {
        // Arrange
        var validator = new LoginCommandValidator();
        var command = new LoginCommand("", "");

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
        result.Errors.Should().Contain(e => e.PropertyName == "Username");
        result.Errors.Should().Contain(e => e.PropertyName == "Password");
    }

    [Fact]
    public void TestLoginCommandValidator_Valid()
    {
        // Arrange
        var validator = new LoginCommandValidator();
        var command = new LoginCommand("123", "123");

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void TestRegisterUserCommandValidator_NotValidOnError()
    {
        // Arrange
        var validator = new RegisterCommadValidator();
        var command = new RegisterUserCommand("", "", "", "", "", "");

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
        result.Errors.Should().Contain(e => e.PropertyName == "FirstName");
        result.Errors.Should().Contain(e => e.PropertyName == "LastName");
        result.Errors.Should().Contain(e => e.PropertyName == "UserName");
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
        result.Errors.Should().Contain(e => e.PropertyName == "Password");
        result.Errors.Should().Contain(e => e.PropertyName == "ConfirmPassword");
    }

    [Fact]
    public void TestRegisterUserCommandValidator_Valid()
    {
        // Arrange
        var validator = new RegisterCommadValidator();
        var command = new RegisterUserCommand("123", "123", "123", "example@gmail.com", "123", "123");

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void TestRegisterUserCommandValidator_NoValidOnWrongEmail()
    {
        // Arrange
        var validator = new RegisterCommadValidator();
        var command = new RegisterUserCommand("123", "123", "123", "example", "123", "123");

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
    }

    [Fact]
    public void TestRegisterUserCommandValidator_NotvalidOnMisMatchedPasswords()
    {
        // Arrange
        var validator = new RegisterCommadValidator();
        var command = new RegisterUserCommand("123", "123", "123", "example@gmail.com", "1234", "123");
        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
        result.Errors.Should().Contain(e => e.PropertyName == "ConfirmPassword");
    }

}