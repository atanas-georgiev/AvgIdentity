namespace AvgIdentity.Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using AvgIdentity.Managers;
    using AvgIdentity.Models;
    using AvgIdentity.Test.Mocks;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Http.Features.Authentication;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Infrastructure;
    using Microsoft.Extensions.DependencyInjection;

    using Microsoft.EntityFrameworkCore;

    using Moq;
    using Xunit;

    public class UserRoleManagerTests : IDisposable
    {
        private UserManager<AvgIdentityUser> userManager { get; set; }

        private TestInMemoryIdentityDbContext testDbContext;

        private UserRoleManager<AvgIdentityUser, TestInMemoryIdentityDbContext> userRoleManager;

        private SignInManager<AvgIdentityUser> signInManager;

        public UserRoleManagerTests()
        {
            var services = new ServiceCollection();
            services.AddEntityFramework()
                .AddEntityFrameworkInMemoryDatabase()
                .AddDbContext<TestInMemoryIdentityDbContext>(options => options.UseInMemoryDatabase());
            services.AddIdentity<AvgIdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<TestInMemoryIdentityDbContext>();
            // Taken from https://github.com/aspnet/MusicStore/blob/dev/test/MusicStore.Test/ManageControllerTest.cs (and modified)
            // IHttpContextAccessor is required for SignInManager, and UserManager
            var context = new DefaultHttpContext();
            context.Features.Set<IHttpAuthenticationFeature>(new HttpAuthenticationFeature());
            services.AddSingleton<IHttpContextAccessor>(h => new HttpContextAccessor { HttpContext = context });
            var serviceProvider = services.BuildServiceProvider();
            this.testDbContext = serviceProvider.GetRequiredService<TestInMemoryIdentityDbContext>();
            this.userManager = serviceProvider.GetRequiredService<UserManager<AvgIdentityUser>>();

            //this.signInManager = new SignInManager<AvgIdentityUser>();
            //this.testDbContext = new TestInMemoryIdentityDbContext();
            this.userRoleManager = new UserRoleManager<AvgIdentityUser, TestInMemoryIdentityDbContext>(this.userManager, null, this.testDbContext);
        }

        public void Dispose()
        {

        }

        //[TestCleanup]
        //public void TestEnd()
        //{
        //    this.testDbContext.Dispose();
        //}

        [Fact]
        public void AddRolesShouldBehaveCorrectly()
        {
            bool res;

            res = this.userRoleManager.AddRoleAsync(role: null).Result;
            Assert.False(res, "AddRoleAsync adds null role");
            Assert.True(!this.userRoleManager.GetAllRoles().Any(), "AddRoleAsync adds null role");

            res = this.userRoleManager.AddRoleAsync(string.Empty).Result;
            Assert.False(res, "AddRoleAsync adds empty role");
            Assert.True(!this.userRoleManager.GetAllRoles().Any(), "AddRoleAsync adds empty role");

            res = this.userRoleManager.AddRoleAsync(roles: null).Result;
            Assert.False(res, "AddRoleAsync adds null roles");
            Assert.True(!this.userRoleManager.GetAllRoles().Any(), "AddRoleAsync adds null roles");

            res = this.userRoleManager.AddRoleAsync(new[] { "Role", null }).Result;
            Assert.False(res, "AddRoleAsync adds null role");
            Assert.True(!this.userRoleManager.GetAllRoles().Any(), "AddRoleAsync adds null role");

            res = this.userRoleManager.AddRoleAsync(new[] { "Role", string.Empty }).Result;
            Assert.False(res, "AddRoleAsync adds empty role");
            Assert.True(!this.userRoleManager.GetAllRoles().Any(), "AddRoleAsync adds empty role");

            res = this.userRoleManager.AddRoleAsync("Role1").Result;
            Assert.True(res, "AddRoleAsync do not add role");
            Assert.True(this.userRoleManager.GetAllRoles().Count() == 1, "AddRoleAsync do not add role");

            res = this.userRoleManager.AddRoleAsync("Role1").Result;
            Assert.False(res, "AddRoleAsync add already existing role");
            Assert.True(this.userRoleManager.GetAllRoles().Count() == 1, "AddRoleAsync add already existing role");

            res = this.userRoleManager.AddRoleAsync(new[] { "Role2", "Role3" }).Result;
            Assert.True(res, "AddRoleAsync do not add multiple roles");
            Assert.True(this.userRoleManager.GetAllRoles().Count() == 3, "AddRoleAsync do not add multiple roles");

            res = this.userRoleManager.AddRoleAsync(new[] { "Role3", "Role4" }).Result;
            Assert.False(res, "AddRoleAsync do add multiple roles if role already exists");
            Assert.True(this.userRoleManager.GetAllRoles().Count() == 3, "AddRoleAsync do add multiple roles if role already exists");
        }

        [Fact]
        public void AddUserssShouldBehaveCorrectly()
        {
    //        Task<TUser> AddUserAsync(
    //string email,
    //string password,
    //string question = null,
    //string answer = null,
    //string firstName = null,
    //string lastName = null,
    //string role = null);

            // null email, empty email
            // duplicate email
            // null password, empty password, short password
            // invalid email
            // long email, long password
            // long question, long firstname, long lastname
            // invalid role, empty role

            
            bool res;

            var user = this.userRoleManager.AddUserAsync("avg@gbg.bg", "Mypassword@1").Result;

            var users = this.userRoleManager.GetAllUsers().Count();

        }

        [Fact]
        public void DeleteRolesShouldBehaveCorrectly()
        {
            bool res;

            res = this.userRoleManager.AddRoleAsync("Role1").Result;
            Assert.True(res, "AddRoleAsync do not add role");
            Assert.True(this.userRoleManager.GetAllRoles().Count() == 1, "AddRoleAsync do not add role");

            res = this.userRoleManager.RemoveRoleAsync("Role2").Result;
            Assert.False(res, "RemoveRoleAsync deletes not existing role");
            Assert.True(this.userRoleManager.GetAllRoles().Count() == 1, "RemoveRoleAsync deletes not existing role");

            res = this.userRoleManager.RemoveRoleAsync("Role1").Result;
            Assert.True(res, "RemoveRoleAsync do not delete existing role");
            Assert.True(!this.userRoleManager.GetAllRoles().Any(), "RemoveRoleAsync do not delete existing role");

        }
    }
}