using AvgIdentity.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using AvgIdentity.Managers;
using System;
using System.Linq;

namespace AvgIdentity.Test.Mocks
{
    internal class UserRoleManagerMock : IDisposable
    {
        private UserManager<AvgIdentityUser> userManager { get; set; }

        private TestInMemoryIdentityDbContext testDbContext;

        public UserRoleManager<AvgIdentityUser, TestInMemoryIdentityDbContext> userRoleManager { get; private set; }

        private SignInManager<AvgIdentityUser> signInManager;

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

            //this.signInManager = new SignInManager<AvgIdentityUser>();
            //this.testDbContext = new TestInMemoryIdentityDbContext();
            this.userRoleManager = new UserRoleManager<AvgIdentityUser, TestInMemoryIdentityDbContext>(this.userManager, null, this.testDbContext);
        }

        public void Dispose()
        {
            var resUsers = this.userRoleManager.RemoveUserAsync(this.userRoleManager.GetAllUsers().ToList()).Result;
            var resRoles = this.userRoleManager.RemoveRoleAsync(this.userRoleManager.GetAllRoles().ToList()).Result;

            this.userManager.Dispose();
            this.testDbContext.Dispose();
        }
    }
}
