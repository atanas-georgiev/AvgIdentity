namespace AvgIdentity.Test
{
    using System.Collections.Generic;
    using System.Linq;

    using AvgIdentity.Managers;
    using AvgIdentity.Models;
    using AvgIdentity.Test.Mocks;

    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class UserRoleManagerTests
    {
        private TestInMemoryIdentityDbContext testDbContext;

        private UserRoleManager<AvgIdentityUser, TestInMemoryIdentityDbContext> userRoleManager;

        [TestInitialize]
        public void TestStart()
        {
            this.testDbContext = new TestInMemoryIdentityDbContext();
            this.userRoleManager = new UserRoleManager<AvgIdentityUser, TestInMemoryIdentityDbContext>(null, null, this.testDbContext);
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