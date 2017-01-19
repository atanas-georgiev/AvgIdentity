namespace AvgIdentity.Test
{
    using System;
    using System.Linq;
    using AvgIdentity.Models;
    using AvgIdentity.Test.Mocks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Threading.Tasks;

    [TestClass]
    public class UserRoleManagerTests
    {
        [TestMethod]
        public void AddRolesShouldBehaveCorrectly()
        {
            using (var userRoleManagerMock = new UserRoleManagerMock())
            {
                var userRoleManager = userRoleManagerMock.userRoleManager;
                bool res;

                res = userRoleManager.AddRoleAsync(role: null).Result;
                Assert.IsFalse(res, "AddRoleAsync adds null role");
                Assert.IsTrue(!userRoleManager.GetAllRoles().Any(), "AddRoleAsync adds null role");

                res = userRoleManager.AddRoleAsync(string.Empty).Result;
                Assert.IsFalse(res, "AddRoleAsync adds empty role");
                Assert.IsTrue(!userRoleManager.GetAllRoles().Any(), "AddRoleAsync adds empty role");

                res = userRoleManager.AddRoleAsync(roles: null).Result;
                Assert.IsFalse(res, "AddRoleAsync adds null roles");
                Assert.IsTrue(!userRoleManager.GetAllRoles().Any(), "AddRoleAsync adds null roles");

                res = userRoleManager.AddRoleAsync(new[] { "Role", null }).Result;
                Assert.IsFalse(res, "AddRoleAsync adds null role");
                Assert.IsTrue(!userRoleManager.GetAllRoles().Any(), "AddRoleAsync adds null role");

                res = userRoleManager.AddRoleAsync(new[] { "Role", string.Empty }).Result;
                Assert.IsFalse(res, "AddRoleAsync adds empty role");
                Assert.IsTrue(!userRoleManager.GetAllRoles().Any(), "AddRoleAsync adds empty role");

                res = userRoleManager.AddRoleAsync("Role1").Result;
                Assert.IsTrue(res, "AddRoleAsync do not add role");
                Assert.IsTrue(userRoleManager.GetAllRoles().Count() == 1, "AddRoleAsync do not add role");

                res = userRoleManager.AddRoleAsync("Role1").Result;
                Assert.IsFalse(res, "AddRoleAsync add already existing role");
                Assert.IsTrue(userRoleManager.GetAllRoles().Count() == 1, "AddRoleAsync add already existing role");

                res = userRoleManager.AddRoleAsync(new[] { "Role2", "Role3" }).Result;
                Assert.IsTrue(res, "AddRoleAsync do not add multiple roles");
                Assert.IsTrue(userRoleManager.GetAllRoles().Count() == 3, "AddRoleAsync do not add multiple roles");

                res = userRoleManager.AddRoleAsync(new[] { "Role3", "Role4" }).Result;
                Assert.IsFalse(res, "AddRoleAsync do add multiple roles if role already exists");
                Assert.IsTrue(userRoleManager.GetAllRoles().Count() == 3, "AddRoleAsync do add multiple roles if role already exists");
            }
        }

        [TestMethod]
        public async Task AddUsersShouldBehaveCorrectly()
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

            using (var userRoleManagerMock = new UserRoleManagerMock())
            {
                var userRoleManager = userRoleManagerMock.userRoleManager;                
                AvgIdentityUser user;

                // null email
                user = await userRoleManager.AddUserAsync(email: null, password: "Password@1");
                Assert.IsNull(user, "AddUserAsync adds user with null email");
                Assert.IsTrue(userRoleManager.GetAllUsers().Count() == 0, "AddUserAsync adds user with null email");

                // null user
                user = await userRoleManager.AddUserAsync(user: null, password: "Password@1");
                Assert.IsNull(user, "AddUserAsync adds user with null object");
                Assert.IsTrue(userRoleManager.GetAllUsers().Count() == 0, "AddUserAsync adds user with null object");

                // null password
                user = await userRoleManager.AddUserAsync("user@test.com", null);
                Assert.IsNull(user, "AddUserAsync adds user with null password");
                Assert.IsTrue(userRoleManager.GetAllUsers().Count() == 0, "AddUserAsync adds user with null password");

                // empty password
                user = await userRoleManager.AddUserAsync("user@test.com", string.Empty);
                Assert.IsNull(user, "AddUserAsync adds user with empty password");
                Assert.IsTrue(userRoleManager.GetAllUsers().Count() == 0, "AddUserAsync adds user with empty password");

                // short password
                user = await userRoleManager.AddUserAsync("user@test.com", "Pas@1");
                Assert.IsNull(user, "AddUserAsync adds user with short password");
                Assert.IsTrue(userRoleManager.GetAllUsers().Count() == 0, "AddUserAsync adds user with short password");

                // not existing role
                user = await userRoleManager.AddUserAsync("user@test.com", "Password@1", "Question", "Answer", "FirstName", "LastName", "Role1");
                Assert.IsNull(user, "AddUserAsync adds user with not existing role");
                Assert.IsTrue(userRoleManager.GetAllUsers().Count() == 0, "AddUserAsync adds user with not existing role");

                // empty role
                user = await userRoleManager.AddUserAsync("user@test.com", "Password@1", "Question", "Answer", "FirstName", "LastName", string.Empty);
                Assert.IsNull(user, "AddUserAsync adds user with empty role");
                Assert.IsTrue(userRoleManager.GetAllUsers().Count() == 0, "AddUserAsync adds user with empty role");

                await userRoleManager.AddRoleAsync("Role1");

                // Valid user, no role
                user = await userRoleManager.AddUserAsync("user1@test.com", "Password@1");
                Assert.IsNotNull(user, "AddUserAsync do not add valid user");
                Assert.IsTrue(userRoleManager.GetAllUsers().Count() == 1, "AddUserAsync do not add valid user");

                // Duplicate user email, no role                
                user = await userRoleManager.AddUserAsync("user1@test.com", "Password@1");
                Assert.IsNull(user, "AddUserAsync adds duplicate user email");
                Assert.IsTrue(userRoleManager.GetAllUsers().Count() == 1, "AddUserAsync adds duplicate user email");

                // Valid user, valid role                
                user = await userRoleManager.AddUserAsync("user2@test.com", "Password@1", "Question", "Answer", "FirstName", "LastName", "Role1");
                Assert.IsNotNull(user, "AddUserAsync do not add valid user");
                Assert.IsTrue(userRoleManager.GetAllUsers().Count() == 2, "AddUserAsync do not add valid user");

                // Valid user, valid role                
                user = await userRoleManager.AddUserAsync("user3@test.com", "Password@1", null, null, null, null, "Role1");
                Assert.IsNotNull(user, "AddUserAsync do not add valid user");
                Assert.IsTrue(userRoleManager.GetAllUsers().Count() == 3, "AddUserAsync do not add valid user");

                // todo: add user object
            }
        }

        [TestMethod]
        public async Task RemoveRolesShouldBehaveCorrectly()
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



                // Add user to existing role

                // Remove existing role
                //res = await userRoleManager.RemoveRoleAsync("Role1");
                //Assert.IsTrue(res, "RemoveRoleAsync do not delete existing role");
                //Assert.IsTrue(userRoleManager.GetAllRoles().Count() == 0, "RemoveRoleAsync do not delete existing role");

                //res = await userRoleManager.RemoveRoleAsync("Role2");
                //Assert.IsFalse(res, "RemoveRoleAsync deletes not existing role");
                //Assert.IsTrue(userRoleManager.GetAllRoles().Count() == 1, "RemoveRoleAsync deletes not existing role");



                //res = await userRoleManager.RemoveRoleAsync("Role1");
                //Assert.IsTrue(res, "RemoveRoleAsync do not delete existing role");
                //Assert.IsTrue(!userRoleManager.GetAllRoles().Any(), "RemoveRoleAsync do not delete existing role");
            }
        }
    }
}