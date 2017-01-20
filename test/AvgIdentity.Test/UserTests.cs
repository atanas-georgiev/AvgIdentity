namespace AvgIdentity.Test
{
    using AvgIdentity.Models;
    using AvgIdentity.Test.Mocks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    [TestClass]
    public class UserTests
    {
        [TestMethod]
        public async Task AddUsersShouldBehaveCorrectlyAsync()
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
        public async Task RemoveUsersShouldBehaveCorrectlyAsync()
        {
            using (var userRoleManagerMock = new UserRoleManagerMock())
            {
                var userRoleManager = userRoleManagerMock.userRoleManager;
                AvgIdentityUser user;
                bool res;

                // Add roles and users
                await userRoleManager.AddRoleAsync(new[] { "Role1", "Role2", "Role3", "Role4", "Role5" });
                await userRoleManager.AddUserAsync("user1@test.com", "Password@1", null, null, null, null, "Role1");
                await userRoleManager.AddUserAsync("user2@test.com", "Password@1", null, null, null, null, "Role1");
                await userRoleManager.AddUserAsync("user3@test.com", "Password@1", null, null, null, null, "Role1");
                await userRoleManager.AddUserAsync("user4@test.com", "Password@1", null, null, null, null, "Role1");
                await userRoleManager.AddUserAsync("user5@test.com", "Password@1", null, null, null, null, "Role1");

                Assert.IsTrue(userRoleManager.GetAllUsers().Count() == 5, "AddUserAsync do not add valid user");

                user = userRoleManager.GetUser("user1@test.com");
                Assert.IsNotNull(user, "Cannot get user");

                // Remove null user
                res = await userRoleManager.RemoveUserAsync(user: null);                
                Assert.IsFalse(res, "RemoveUserAsync removes null user");
                Assert.IsTrue(userRoleManager.GetAllUsers().Count() == 5, "RemoveUserAsync removes null user");

                res = await userRoleManager.RemoveUserAsync(email: null);
                Assert.IsFalse(res, "RemoveUserAsync removes null user");
                Assert.IsTrue(userRoleManager.GetAllUsers().Count() == 5, "RemoveUserAsync removes null user");

                res = await userRoleManager.RemoveUserAsync(email: string.Empty);
                Assert.IsFalse(res, "RemoveUserAsync removes empty email");
                Assert.IsTrue(userRoleManager.GetAllUsers().Count() == 5, "RemoveUserAsync removes empty email");

                res = await userRoleManager.RemoveUserAsync(email: "notexisting@email.test");
                Assert.IsFalse(res, "RemoveUserAsync removes user with not existing email");
                Assert.IsTrue(userRoleManager.GetAllUsers().Count() == 5, "RemoveUserAsync removes user with not existing email");

                res = await userRoleManager.RemoveUserAsync(users: null);
                Assert.IsFalse(res, "RemoveUserAsync removes null user");
                Assert.IsTrue(userRoleManager.GetAllUsers().Count() == 5, "RemoveUserAsync removes null user");

                res = await userRoleManager.RemoveUserAsync(new List<AvgIdentityUser>());
                Assert.IsFalse(res, "RemoveUserAsync removes empty user list");
                Assert.IsTrue(userRoleManager.GetAllUsers().Count() == 5, "RemoveUserAsync removes empty user list");

                res = await userRoleManager.RemoveUserAsync(new List<AvgIdentityUser>() { null });
                Assert.IsFalse(res, "RemoveUserAsync removes empty user list");
                Assert.IsTrue(userRoleManager.GetAllUsers().Count() == 5, "RemoveUserAsync removes empty user list");

                res = await userRoleManager.RemoveUserAsync(user);
                Assert.IsTrue(res, "RemoveUserAsync do not delete valid user");
                Assert.IsTrue(userRoleManager.GetAllUsers().Count() == 4, "RemoveUserAsync do not delete valid user");

                var users = userRoleManager.GetAllUsers().ToList();
                res = await userRoleManager.RemoveUserAsync(users);
                Assert.IsTrue(res, "RemoveUserAsync do not delete valid users");
                Assert.IsTrue(userRoleManager.GetAllUsers().Count() == 0, "RemoveUserAsync do not delete valid user");
            }
        }

        [TestMethod]
        public async Task UpdateUsersShouldBehaveCorrectlyAsync()
        {
            using (var userRoleManagerMock = new UserRoleManagerMock())
            {
                var userRoleManager = userRoleManagerMock.userRoleManager;
                AvgIdentityUser user;
                bool res;

                // Add roles and users
                await userRoleManager.AddRoleAsync("Role1");
                await userRoleManager.AddUserAsync("user1@test.com", "Password@1", null, null, null, null, "Role1");
                await userRoleManager.AddUserAsync("user2@test.com", "Password@1", null, null, null, null, "Role1");

                user = userRoleManager.GetUser("user1@test.com");

                // Change null user                
                res = await userRoleManager.UpdateUserAsync(null);
                Assert.IsFalse(res, "UpdateUserAsync updates null user");

                // Change user with duplicate email
                user.Email = "user2@test.com";
                res = await userRoleManager.UpdateUserAsync(user);
                Assert.IsFalse(res, "UpdateUserAsync changes duplicate email");

                // Change not existing user
                user = new AvgIdentityUser() { Email = "notexisting@email.test" };
                res = await userRoleManager.UpdateUserAsync(user);
                Assert.IsFalse(res, "UpdateUserAsync changes not existing user");
            }
        }
    }
}
