namespace AvgIdentity.Test
{
    using AvgIdentity.Models;
    using AvgIdentity.Test.Mocks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Linq;
    using System.Threading.Tasks;

    [TestClass]
    public class RoleTests
    {
        [TestMethod]
        public async Task AddRolesShouldBehaveCorrectlyAsync()
        {
            using (var userRoleManagerMock = new UserRoleManagerMock())
            {
                var userRoleManager = userRoleManagerMock.userRoleManager;
                bool res;

                res = await userRoleManager.AddRoleAsync(role: null);
                Assert.IsFalse(res, "AddRoleAsync adds null role");
                Assert.IsTrue(!userRoleManager.GetAllRoles().Any(), "AddRoleAsync adds null role");

                res = await userRoleManager.AddRoleAsync(string.Empty);
                Assert.IsFalse(res, "AddRoleAsync adds empty role");
                Assert.IsTrue(!userRoleManager.GetAllRoles().Any(), "AddRoleAsync adds empty role");

                res = await userRoleManager.AddRoleAsync(roles: null);
                Assert.IsFalse(res, "AddRoleAsync adds null roles");
                Assert.IsTrue(!userRoleManager.GetAllRoles().Any(), "AddRoleAsync adds null roles");

                res = await userRoleManager.AddRoleAsync(new[] { "Role", null });
                Assert.IsFalse(res, "AddRoleAsync adds null role");
                Assert.IsTrue(!userRoleManager.GetAllRoles().Any(), "AddRoleAsync adds null role");

                res = await userRoleManager.AddRoleAsync(new[] { "Role", string.Empty });
                Assert.IsFalse(res, "AddRoleAsync adds empty role");
                Assert.IsTrue(!userRoleManager.GetAllRoles().Any(), "AddRoleAsync adds empty role");

                res = await userRoleManager.AddRoleAsync("Role1");
                Assert.IsTrue(res, "AddRoleAsync do not add role");
                Assert.IsTrue(userRoleManager.GetAllRoles().Count() == 1, "AddRoleAsync do not add role");

                res = await userRoleManager.AddRoleAsync("Role1");
                Assert.IsFalse(res, "AddRoleAsync add already existing role");
                Assert.IsTrue(userRoleManager.GetAllRoles().Count() == 1, "AddRoleAsync add already existing role");

                res = await userRoleManager.AddRoleAsync(new[] { "Role2", "Role3" });
                Assert.IsTrue(res, "AddRoleAsync do not add multiple roles");
                Assert.IsTrue(userRoleManager.GetAllRoles().Count() == 3, "AddRoleAsync do not add multiple roles");

                res = await userRoleManager.AddRoleAsync(new[] { "Role3", "Role4" });
                Assert.IsFalse(res, "AddRoleAsync do add multiple roles if role already exists");
                Assert.IsTrue(userRoleManager.GetAllRoles().Count() == 3, "AddRoleAsync do add multiple roles if role already exists");
            }
        }

        [TestMethod]
        public async Task RemoveRolesShouldBehaveCorrectlyAsync()
        {
            using (var userRoleManagerMock = new UserRoleManagerMock())
            {
                var userRoleManager = userRoleManagerMock.userRoleManager;
                AvgIdentityUser user;
                bool res;

                // Add test role
                res = await userRoleManager.AddRoleAsync("Role1");
                Assert.IsTrue(res, "AddRoleAsync do not add role");
                Assert.IsTrue(userRoleManager.GetAllRoles().Count() == 1, "AddRoleAsync do not add role");

                // Remove null role
                res = await userRoleManager.RemoveRoleAsync(role: null);
                Assert.IsFalse(res, "RemoveRoleAsync deletes null role");
                Assert.IsTrue(userRoleManager.GetAllRoles().Count() == 1, "RemoveRoleAsync deletes null role");

                // Remove empty role
                res = await userRoleManager.RemoveRoleAsync(role: string.Empty);
                Assert.IsFalse(res, "RemoveRoleAsync deletes empty role");
                Assert.IsTrue(userRoleManager.GetAllRoles().Count() == 1, "RemoveRoleAsync deletes empty role");

                // Remove null roles
                res = await userRoleManager.RemoveRoleAsync(roles: null);
                Assert.IsFalse(res, "RemoveRoleAsync deletes null roles");
                Assert.IsTrue(userRoleManager.GetAllRoles().Count() == 1, "RemoveRoleAsync deletes null roles");

                // Remove roles containing null role
                res = await userRoleManager.RemoveRoleAsync(new[] { "Role1", null });
                Assert.IsFalse(res, "RemoveRoleAsync invalid remove null role");
                Assert.IsTrue(userRoleManager.GetAllRoles().Count() == 1, "RemoveRoleAsync invalid remove null role");

                // Remove roles containing empty role
                res = await userRoleManager.RemoveRoleAsync(new[] { "Role1", string.Empty });
                Assert.IsFalse(res, "RemoveRoleAsync invalid remove null role");
                Assert.IsTrue(userRoleManager.GetAllRoles().Count() == 1, "RemoveRoleAsync invalid remove null role");

                // Remove existing role
                res = await userRoleManager.RemoveRoleAsync("Role1");
                Assert.IsTrue(res, "RemoveRoleAsync do not delete existing role");
                Assert.IsTrue(userRoleManager.GetAllRoles().Count() == 0, "RemoveRoleAsync do not delete existing role");

                // Add test roles
                res = await userRoleManager.AddRoleAsync(new[] { "Role1", "Role2", "Role3" });
                Assert.IsTrue(res, "AddRoleAsync do not add roles");
                Assert.IsTrue(userRoleManager.GetAllRoles().Count() == 3, "AddRoleAsync do not add roles");

                // Remove existing roles
                res = await userRoleManager.RemoveRoleAsync(new[] { "Role1", "Role2" });
                Assert.IsTrue(res, "RemoveRoleAsync do not delete existing roles");
                Assert.IsTrue(userRoleManager.GetAllRoles().Count() == 1, "RemoveRoleAsync do not delete existing roles");

                // Add valid user
                user = await userRoleManager.AddUserAsync("user3@test.com", "Password@1", "Question", "Answer", "FirstName", "LastName", "Role3");
                Assert.IsNotNull(user, "AddUserAsync do not add valid user");
                Assert.IsTrue(userRoleManager.GetAllUsers().Count() == 1, "AddUserAsync do not add valid user");

                // Delete roles with exsting user in this role
                res = await userRoleManager.RemoveRoleAsync("Role3");
                Assert.IsFalse(res, "RemoveRoleAsync do not delete existing role");
                Assert.IsTrue(userRoleManager.GetAllRoles().Count() == 1, "RemoveRoleAsync do not delete existing role");
            }
        }
    }
}
