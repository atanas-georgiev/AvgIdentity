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
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Microsoft.EntityFrameworkCore;

    using Moq;

    public class DatabaseSetupTests : IDisposable
    {


        public DatabaseSetupTests()
        {
            
        }

        public void Dispose()
        {
        }
    }

    [TestClass]
    public class UserRoleManagerTests
    {
        private UserManager<AvgIdentityUser> userManager { get; set; }

        private TestInMemoryIdentityDbContext testDbContext;

        private UserRoleManager<AvgIdentityUser, TestInMemoryIdentityDbContext> userRoleManager;

        private SignInManager<AvgIdentityUser> signInManager;

        [TestInitialize]
        public void TestStart()
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

        [TestCleanup]
        public void TestEnd()
        {
            this.testDbContext.Dispose();
        }

        [TestMethod]
        public void AddRolesShouldBehaveCorrectly()
        {
            bool res;

            res = this.userRoleManager.AddRoleAsync(role: null).Result;
            Assert.IsFalse(res, "AddRoleAsync adds null role");
            Assert.IsTrue(!this.userRoleManager.GetAllRoles().Any(), "AddRoleAsync adds null role");

            res = this.userRoleManager.AddRoleAsync(string.Empty).Result;
            Assert.IsFalse(res, "AddRoleAsync adds empty role");
            Assert.IsTrue(!this.userRoleManager.GetAllRoles().Any(), "AddRoleAsync adds empty role");

            res = this.userRoleManager.AddRoleAsync(roles: null).Result;
            Assert.IsFalse(res, "AddRoleAsync adds null roles");
            Assert.IsTrue(!this.userRoleManager.GetAllRoles().Any(), "AddRoleAsync adds null roles");

            res = this.userRoleManager.AddRoleAsync(new[] { "Role", null }).Result;
            Assert.IsFalse(res, "AddRoleAsync adds null role");
            Assert.IsTrue(!this.userRoleManager.GetAllRoles().Any(), "AddRoleAsync adds null role");

            res = this.userRoleManager.AddRoleAsync(new[] { "Role", string.Empty }).Result;
            Assert.IsFalse(res, "AddRoleAsync adds empty role");
            Assert.IsTrue(!this.userRoleManager.GetAllRoles().Any(), "AddRoleAsync adds empty role");

            res = this.userRoleManager.AddRoleAsync("Role1").Result;
            Assert.IsTrue(res, "AddRoleAsync do not add role");
            Assert.IsTrue(this.userRoleManager.GetAllRoles().Count() == 1, "AddRoleAsync do not add role");

            res = this.userRoleManager.AddRoleAsync("Role1").Result;
            Assert.IsFalse(res, "AddRoleAsync add already existing role");
            Assert.IsTrue(this.userRoleManager.GetAllRoles().Count() == 1, "AddRoleAsync add already existing role");

            res = this.userRoleManager.AddRoleAsync(new[] { "Role2", "Role3" }).Result;
            Assert.IsTrue(res, "AddRoleAsync do not add multiple roles");
            Assert.IsTrue(this.userRoleManager.GetAllRoles().Count() == 3, "AddRoleAsync do not add multiple roles");

            res = this.userRoleManager.AddRoleAsync(new[] { "Role3", "Role4" }).Result;
            Assert.IsFalse(res, "AddRoleAsync do add multiple roles if role already exists");
            Assert.IsTrue(this.userRoleManager.GetAllRoles().Count() == 3, "AddRoleAsync do add multiple roles if role already exists");
        }

        [TestMethod]
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

        [TestMethod]
        public void DeleteRolesShouldBehaveCorrectly()
        {
            bool res;

            res = this.userRoleManager.AddRoleAsync("Role1").Result;
            Assert.IsTrue(res, "AddRoleAsync do not add role");
            Assert.IsTrue(this.userRoleManager.GetAllRoles().Count() == 1, "AddRoleAsync do not add role");

            res = this.userRoleManager.RemoveRoleAsync("Role2").Result;
            Assert.IsFalse(res, "RemoveRoleAsync deletes not existing role");
            Assert.IsTrue(this.userRoleManager.GetAllRoles().Count() == 1, "RemoveRoleAsync deletes not existing role");

            res = this.userRoleManager.RemoveRoleAsync("Role1").Result;
            Assert.IsTrue(res, "RemoveRoleAsync do not delete existing role");
            Assert.IsTrue(!this.userRoleManager.GetAllRoles().Any(), "RemoveRoleAsync do not delete existing role");

        }
    }
}