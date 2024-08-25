using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Serilog;

namespace TodoApp.Test.Integration;

public sealed class TodoAppFixture<TProgram> : WebApplicationFactory<TProgram> where TProgram : Program
{
    public string DefaultUserId { get; set; } = "1";
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {

        builder.ConfigureTestServices(c =>
        {
            // c.Configure<TestAuthHandlerOptions>(options => options.DefaultUserId = DefaultUserId);
            // c.AddAuthentication(TestAuthenticationHandler.AuthenticationScheme)
            //     .AddScheme<TestAuthHandlerOptions, TestAuthenticationHandler>(TestAuthenticationHandler.AuthenticationScheme, options => { });
        });

    }

}