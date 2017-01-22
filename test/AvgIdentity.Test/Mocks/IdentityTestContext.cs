namespace AvgIdentity.Test.Mocks
{
    using System;
    using System.Linq;
    using System.Security.Claims;

    using AvgIdentity.Managers;
    using AvgIdentity.Models;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Http.Features.Authentication;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Infrastructure;
    using Microsoft.Extensions.DependencyInjection;

    internal class IdentityTestContext : IDisposable
    {
        public IdentityTestContext()
        {
            var services = new ServiceCollection();

            services.AddEntityFramework()
                .AddEntityFrameworkInMemoryDatabase()
                .AddDbContext<IdentityTestInMemoryDbContext>(options => options.UseInMemoryDatabase());

            services.AddIdentity<AvgIdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<IdentityTestInMemoryDbContext>();

            var context = new DefaultHttpContext();
            this.AuthenticationHandler = new IdentityTestAuthenticationHandler();
            this.User = new ClaimsPrincipal();
            context.Features.Set<IHttpAuthenticationFeature>(
                new HttpAuthenticationFeature() { Handler = this.AuthenticationHandler, User = this.User });
            services.AddScoped<IHttpContextAccessor>(h => new HttpContextAccessor { HttpContext = context });

            var serviceProvider = services.BuildServiceProvider();
            var testDbContext = serviceProvider.GetRequiredService<IdentityTestInMemoryDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<AvgIdentityUser>>();
            var signInManager = serviceProvider.GetRequiredService<SignInManager<AvgIdentityUser>>();
            serviceProvider.GetRequiredService<SignInManager<AvgIdentityUser>>();

            this.UserRoleManager = new UserRoleManager<AvgIdentityUser, IdentityTestInMemoryDbContext>(
                userManager,
                signInManager,
                testDbContext);
        }

        public IAuthenticationHandler AuthenticationHandler { get; private set; }

        public ClaimsPrincipal User { get; set; }

        public UserRoleManager<AvgIdentityUser, IdentityTestInMemoryDbContext> UserRoleManager { get; private set; }

        public void Dispose()
        {
            var resUsers = this.UserRoleManager.RemoveUserAsync(this.UserRoleManager.GetAllUsers().ToList()).Result;
            var resRoles = this.UserRoleManager.RemoveRoleAsync(this.UserRoleManager.GetAllRoles().ToList()).Result;
        }
    }
}