namespace AvgIdentity.Test.Mocks
{
    using System;
    using System.Linq;

    using AvgIdentity.Managers;
    using AvgIdentity.Models;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Http.Features.Authentication;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Infrastructure;
    using Microsoft.Extensions.DependencyInjection;

    internal class UserRoleManagerMock : IDisposable
    {
        private SignInManager<AvgIdentityUser> signInManager;

        private TestInMemoryIdentityDbContext testDbContext;

        public UserRoleManagerMock()
        {
            ServiceCollection services = new ServiceCollection();

            services.AddEntityFramework()
                .AddEntityFrameworkInMemoryDatabase()
                .AddDbContext<TestInMemoryIdentityDbContext>(options => options.UseInMemoryDatabase());

            services.AddIdentity<AvgIdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<TestInMemoryIdentityDbContext>();

            var context = new DefaultHttpContext();
            context.Features.Set<IHttpAuthenticationFeature>(new HttpAuthenticationFeature());
            services.AddScoped<IHttpContextAccessor>(h => new HttpContextAccessor { HttpContext = context });

            var serviceProvider = services.BuildServiceProvider();
            this.testDbContext = serviceProvider.GetRequiredService<TestInMemoryIdentityDbContext>();
            this.userManager = serviceProvider.GetRequiredService<UserManager<AvgIdentityUser>>();

            // this.signInManager = new SignInManager<AvgIdentityUser>();
            // this.testDbContext = new TestInMemoryIdentityDbContext();
            this.userRoleManager = new UserRoleManager<AvgIdentityUser, TestInMemoryIdentityDbContext>(
                this.userManager,
                null,
                this.testDbContext);
        }

        public UserRoleManager<AvgIdentityUser, TestInMemoryIdentityDbContext> userRoleManager { get; private set; }

        private UserManager<AvgIdentityUser> userManager { get; set; }

        public void Dispose()
        {
            var resUsers = this.userRoleManager.RemoveUserAsync(this.userRoleManager.GetAllUsers().ToList()).Result;
            var resRoles = this.userRoleManager.RemoveRoleAsync(this.userRoleManager.GetAllRoles().ToList()).Result;

            this.userManager.Dispose();
            this.testDbContext.Dispose();
        }
    }
}