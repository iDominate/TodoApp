namespace TodoApp.Test.Infrastructure_Tests.Auth;

using System.Linq.Expressions;
using System.Security.Claims;
using ErrorOr;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using TodoApp.Application.Services;
using TodoApp.Domain.RefreshTokenAggregate;
using TodoApp.Domain.UserAggregate;
using TodoApp.Infrastructure.Authentication;

public class AuthServiceTest
{
    private readonly RoleManager<IdentityRole> roleManager;
    private readonly Mock<IUserRepository> _userRepo;
    private readonly AuthService serviceTest;
    private ApplicationUser user;
    public AuthServiceTest()
    {
        user = new ApplicationUser("Test", "Test")
        {
            UserName = "test",
            Email = "test@example.com"
        };

        var userManager = new Mock<UserManager<ApplicationUser>>(
        new Mock<IQueryableUserStore<ApplicationUser>>().Object,
        null!,
        null!,
        null!,
        null!,
        null!,
        null!,
        null!,
        null!
        );
        roleManager = new RoleManager<IdentityRole>(
        new Mock<IRoleStore<IdentityRole>>().Object,
        [],
        new Mock<ILookupNormalizer>().Object,
        new Mock<IdentityErrorDescriber>().Object,
        new Mock<ILogger<RoleManager<IdentityRole>>>().Object
        );
        var options = Options.Create<JwtSettings>(
            new JwtSettings()
            {
                Audience = "test",
                DurationInMinutes = 5,
                Issuer = "test",
                Key = "90053901813859088021207993080505"
            });
        _userRepo = new Mock<IUserRepository>();
        serviceTest = new AuthService(_userRepo.Object, roleManager, options);

    }
    public virtual async Task<ApplicationUser?> EqualStrings(UserManager<ApplicationUser> userManager)
    {

        return await userManager.Users.FirstOrDefaultAsync(u => u.UserName == It.IsAny<string>());
    }
    [Fact]
    public async void Test_OnSuccessfulRegistration_ReturnsAuthModel()
    {
        //Arrange
        _userRepo.Setup(u => u.GetUserAsync(It.IsAny<Expression<Func<ApplicationUser, bool>>>()))
        .Returns(Task.FromResult<ApplicationUser?>(null));
        var dto = new RegisterRequest()
        {
            FirstName = "Test",
            Lastname = "Test",
            Email = "test@example.com",
            UserName = "test",
            Password = "123",
            ConfirmPassword = "123"
        };
        var result = await serviceTest.RegisterAsync(dto);
        result.Value.Should().BeOfType<AuthModel>();
        result.Value.UserName.Should().Be("test");
        result.Value.Token.Should().NotBeNullOrEmpty();
        result.IsError.Should().BeFalse();
    }

    [Fact]
    public async void Test_OnFailedRegister_ReturnsAuthModelWithError()
    {
        // Arrange
        _userRepo.Setup(u => u.GetUserAsync(It.IsAny<Expression<Func<ApplicationUser, bool>>>()))
        .Returns(Task.FromResult<ApplicationUser?>(user));
        var dto = new RegisterRequest()
        {
            UserName = "test"
        };

        // Act
        var result = await serviceTest.RegisterAsync(dto);

        // Assert
        result.Value.Should().BeOfType<AuthModel>();
        result.IsError.Should().BeTrue();
    }

    [Fact]
    public async void Test_OnFailedLogin_ReturnAuthModelWithError()
    {
        // Arrange
        _userRepo.Setup(u => u.GetUserAsync(It.IsAny<Expression<Func<ApplicationUser, bool>>>()))
        .Returns(Task.FromResult<ApplicationUser>(null!)!);

        // Act
        var dto = new LoginReuqest("test", "123");
        var result = await serviceTest.LoginAsync(dto);

        // Assert
        result.Value.Should().BeOfType<AuthModel>();
        result.IsError.Should().BeTrue();
    }

    [Fact]
    public async void Test_OnSuccessfulLogin_ReturnsAuthModel()
    {
        // Arrange
        SetupToken();

        // Act
        var dto = new LoginReuqest("test", "123");
        var result = await serviceTest.LoginAsync(dto);

        // Assert
        result.Value.Should().BeOfType<AuthModel>();
        result.IsError.Should().BeFalse();
        result.Value.IsAuthenticated.Should().BeTrue();
        result.Value.Token.Should().NotBeNullOrEmpty();
    }
    [Fact]
    public async void TestGenerateTokenAsyncWithNoUser_ReturnsInvalidToken()
    {
        // Arrange
        _userRepo.Setup(u => u.GetUserAsync(It.IsAny<Expression<Func<ApplicationUser, bool>>>()))
        .Returns(Task.FromResult<ApplicationUser?>(null));

        // Act
        var result = await serviceTest.GenerateTokenAsync("123");

        // Assert
        result.Should().BeOfType<Error>();
        result.IsError.Should().BeTrue();
    }

    [Fact]
    public async void TestGenerateTokenAsyncWithUserInDataBase_ReturnsAuthModel()
    {
        // Assert
        SetupToken();
        var refreshToken = new RefreshToken("123", DateTime.UtcNow.AddMinutes(15));

        _userRepo.Setup(u => u.GetRefreshToken(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
        .Returns(refreshToken);

        // Act
        var result = await serviceTest.GenerateTokenAsync("123");

        // Assert
        result.Value.Should().BeOfType<AuthModel>();
        result.Value.Token.Should().NotBeEmpty();
        result.IsError.Should().BeFalse();
        result.Value.IsAuthenticated.Should().BeTrue();
    }
    private void SetupToken()
    {
        _userRepo.Setup(u => u.GetUserAsync(It.IsAny<Expression<Func<ApplicationUser, bool>>>()))
        .Returns(Task.FromResult<ApplicationUser?>(user));
        _userRepo.Setup(repo => repo.CheckPasswordsAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
        .Returns(Task.FromResult<bool>(true));
        _userRepo.Setup(repo => repo.GetClaimsAsync(It.IsAny<ApplicationUser>()))
        .Returns(Task.FromResult<IEnumerable<Claim>>(new List<Claim>{
            new Claim(ClaimTypes.Name, "test"),
            new Claim(ClaimTypes.Email, "test@example.com"),
            new Claim(ClaimTypes.NameIdentifier, "test"),
            new Claim(ClaimTypes.Role, "user"),
            new Claim(ClaimTypes.Role, "admin")
        }));
        _userRepo.Setup(repo => repo.GetRolesAsync(It.IsAny<ApplicationUser>()))
        .Returns(Task.FromResult<IEnumerable<string>>(new List<string>{
            "User"
        }));
    }

}


