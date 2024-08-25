using TodoApp.Domain.UserAggregate;

namespace TodoApp.Domain.UserAggregate;

public static class Extensions
{
    public static ApplicationUser ToApplicationUser(this RegisterRequest dto)
    {
        return new ApplicationUser(dto.FirstName, dto.Lastname)
        {
            UserName = dto.UserName,
            Email = dto.Email
        };
    }
}