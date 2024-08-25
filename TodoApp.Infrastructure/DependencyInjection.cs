using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using TodoApp.Application.Services;
using TodoApp.Domain.UserAggregate;
using TodoApp.Infrastructure.Authentication;
using TodoApp.Infrastructure.Context;
using TodoApp.Infrastructure.Seeders;
using TodoApp.Infrastructure.TodoRepository;

namespace TodoApp.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection service, ConfigurationManager configuration)
    {
        var val1 = configuration.GetValue<string>("JwtSettings:Key")!.Trim('\'');
        var val2 = configuration.GetValue<string>("JwtSettings:Issuer");
        var val3 = configuration.GetValue<string>("JwtSettings:Audience");
        service.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<AppDbContext>();
        service.AddDbContext<AppDbContext>(o => o.UseInMemoryDatabase("todo"));
        service.AddScoped<IAuthService, AuthService>();
        service.AddScoped<ITodoService, TodoService>();
        service.AddTransient<IUserRepository, UserRepository>();
        service.AddAuthentication(o =>
        {
            IdentityModelEventSource.ShowPII = true;
            o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })

        .AddJwtBearer(o =>
        {
            o.UseSecurityTokenValidators = true;
            o.SaveToken = true;
            o.RequireHttpsMetadata = false;
            o.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            {
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<string>("JwtSettings:Key")!)),
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidAudience = configuration.GetValue<string>("JwtSettings:Audience"),
                ValidIssuer = configuration.GetValue<string>("JwtSettings:Issuer"),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
            };
        });
        service.AddAuthorization();

        TodoSeeder.Seed(service.BuildServiceProvider());
        return service;
    }
}