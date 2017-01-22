namespace AvgIdentity.Test
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using AvgIdentity.Models;
    using AvgIdentity.Test.Mocks;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class UserTests
    {
        private const string LongStr =
            "longlonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglong";

        [TestMethod]
        public async Task AddUsersInvalidInputTestsAsync()
        {
            using (var userRoleManagerMock = new IdentityTestContext())
            {
                var userRoleManager = userRoleManagerMock.UserRoleManager;

                // null email
                var user = await userRoleManager.AddUserAsync(email: null);
                Assert.IsNull(user, "AddUserAsync adds user with null email");
                Assert.IsTrue(!userRoleManager.GetAllUsers().Any(), "AddUserAsync adds user with null email");

                // null user
                user = await userRoleManager.AddUserAsync(user: null);
                Assert.IsNull(user, "AddUserAsync adds user with null object");
                Assert.IsTrue(!userRoleManager.GetAllUsers().Any(), "AddUserAsync adds user with null object");

                // invalid email
                user = await userRoleManager.AddUserAsync("invalidemail");
                Assert.IsNull(user, "AddUserAsync adds user with invalid email");
                Assert.IsTrue(!userRoleManager.GetAllUsers().Any(), "AddUserAsync adds user with invalid email");

                // long first name
                user = await userRoleManager.AddUserAsync("email@test.com", null, null, null, LongStr);
                Assert.IsNull(user, "AddUserAsync adds user long first name");
                Assert.IsTrue(!userRoleManager.GetAllUsers().Any(), "AddUserAsync adds user long first name");

                // long last name
                user = await userRoleManager.AddUserAsync("email@test.com", null, null, null, null, LongStr);
                Assert.IsNull(user, "AddUserAsync adds user long last name");
                Assert.IsTrue(!userRoleManager.GetAllUsers().Any(), "AddUserAsync adds user long last name");

                // long password question
                user = await userRoleManager.AddUserAsync("email@test.com", null, LongStr);
                Assert.IsNull(user, "AddUserAsync adds user long password question");
                Assert.IsTrue(!userRoleManager.GetAllUsers().Any(), "AddUserAsync adds user long password question");

                // short password
                user = await userRoleManager.AddUserAsync("user@test.com", "Pas@1");
                Assert.IsNull(user, "AddUserAsync adds user with short password");
                Assert.IsTrue(!userRoleManager.GetAllUsers().Any(), "AddUserAsync adds user with short password");

                // not existing role
                user = await userRoleManager.AddUserAsync(
                           "user@test.com",
                           "Password@1",
                           "Question",
                           "Answer",
                           "FirstName",
                           "LastName",
                           "Role1");
                Assert.IsNull(user, "AddUserAsync adds user with not existing role");
                Assert.IsTrue(!userRoleManager.GetAllUsers().Any(), "AddUserAsync adds user with not existing role");

                // empty role
                user = await userRoleManager.AddUserAsync(
                           "user@test.com",
                           "Password@1",
                           "Question",
                           "Answer",
                           "FirstName",
                           "LastName",
                           string.Empty);
                Assert.IsNull(user, "AddUserAsync adds user with empty role");
                Assert.IsTrue(!userRoleManager.GetAllUsers().Any(), "AddUserAsync adds user with empty role");

                // todo: add user object
            }
        }

        [TestMethod]
        public async Task AddUsersValidInputTestsAsync()
        {
            using (var userRoleManagerMock = new IdentityTestContext())
            {
                var userRoleManager = userRoleManagerMock.UserRoleManager;

                // no password
                var user = await userRoleManager.AddUserAsync("user1@test.com");
                Assert.IsNotNull(user, "AddUserAsync cannot add user with no password");
                Assert.IsTrue(
                    userRoleManager.GetAllUsers().Count() == 1,
                    "AddUserAsync cannot add user with no password");

                // empty password
                user = await userRoleManager.AddUserAsync("user2@test.com", string.Empty);
                Assert.IsNotNull(user, "AddUserAsync cannot add user with no password");
                Assert.IsTrue(
                    userRoleManager.GetAllUsers().Count() == 2,
                    "AddUserAsync cannot add user with no password");

                // null password
                user = await userRoleManager.AddUserAsync("user3@test.com", null);
                Assert.IsNotNull(user, "AddUserAsync cannot add user with no password");
                Assert.IsTrue(
                    userRoleManager.GetAllUsers().Count() == 3,
                    "AddUserAsync cannot add user with no password");

                // duplicate password
                user = await userRoleManager.AddUserAsync("user3@test.com");
                Assert.IsNull(user, "AddUserAsync cannot adds duplicate user");
                Assert.IsTrue(userRoleManager.GetAllUsers().Count() == 3, "AddUserAsync cannot adds duplicate user");

                // valid password
                user = await userRoleManager.AddUserAsync("user4@test.com", "Password@1");
                Assert.IsNotNull(user, "AddUserAsync cannot add user with valid password");
                Assert.IsTrue(
                    userRoleManager.GetAllUsers().Count() == 4,
                    "AddUserAsync cannot add user with valid password");

                // Valid user, valid role
                await userRoleManager.AddRoleAsync("Role1");
                user = await userRoleManager.AddUserAsync(
                           "user5@test.com",
                           "Password@1",
                           "Question",
                           "Answer",
                           "FirstName",
                           "LastName",
                           "Role1");
                Assert.IsNotNull(user, "AddUserAsync do not add valid user with role");
                Assert.IsTrue(
                    userRoleManager.GetAllUsers().Count() == 5,
                    "AddUserAsync do not add valid user with role");

                // Valid user, valid role                
                user = await userRoleManager.AddUserAsync(
                           "user6@test.com",
                           "Password@1",
                           null,
                           null,
                           null,
                           null,
                           "Role1");
                Assert.IsNotNull(user, "AddUserAsync do not add valid user with role");
                Assert.IsTrue(
                    userRoleManager.GetAllUsers().Count() == 6,
                    "AddUserAsync do not add valid user with role");
            }
        }

        [TestMethod]
        public async Task RemoveUsersInvalidInputTestsAsync()
        {
            using (var userRoleManagerMock = new IdentityTestContext())
            {
                var userRoleManager = userRoleManagerMock.UserRoleManager;

                // Add roles and users
                await userRoleManager.AddRoleAsync(new[] { "Role1", "Role2", "Role3", "Role4", "Role5" });
                await userRoleManager.AddUserAsync("user1@test.com", "Password@1", null, null, null, null, "Role1");
                await userRoleManager.AddUserAsync("user2@test.com", "Password@1", null, null, null, null, "Role1");
                await userRoleManager.AddUserAsync("user3@test.com", "Password@1", null, null, null, null, "Role1");
                await userRoleManager.AddUserAsync("user4@test.com", "Password@1", null, null, null, null, "Role1");
                await userRoleManager.AddUserAsync("user5@test.com", "Password@1", null, null, null, null, "Role1");

                Assert.IsTrue(userRoleManager.GetAllUsers().Count() == 5, "AddUserAsync do not add valid user");

                // Remove null user
                var res = await userRoleManager.RemoveUserAsync(user: null);
                Assert.IsFalse(res, "RemoveUserAsync removes null user");
                Assert.IsTrue(userRoleManager.GetAllUsers().Count() == 5, "RemoveUserAsync removes null user");

                // Remove user with null email
                res = await userRoleManager.RemoveUserAsync(email: null);
                Assert.IsFalse(res, "RemoveUserAsync removes null user");
                Assert.IsTrue(userRoleManager.GetAllUsers().Count() == 5, "RemoveUserAsync removes null user");

                // Remove user with empty email
                res = await userRoleManager.RemoveUserAsync(email: string.Empty);
                Assert.IsFalse(res, "RemoveUserAsync removes empty email");
                Assert.IsTrue(userRoleManager.GetAllUsers().Count() == 5, "RemoveUserAsync removes empty email");

                // Remove user with not existing email
                res = await userRoleManager.RemoveUserAsync(email: "notexisting@email.test");
                Assert.IsFalse(res, "RemoveUserAsync removes user with not existing email");
                Assert.IsTrue(
                    userRoleManager.GetAllUsers().Count() == 5,
                    "RemoveUserAsync removes user with not existing email");

                // Remove users with null list
                res = await userRoleManager.RemoveUserAsync(users: null);
                Assert.IsFalse(res, "RemoveUserAsync removes null user");
                Assert.IsTrue(userRoleManager.GetAllUsers().Count() == 5, "RemoveUserAsync removes null user");

                // Removes users with empty list
                res = await userRoleManager.RemoveUserAsync(new List<AvgIdentityUser>());
                Assert.IsFalse(res, "RemoveUserAsync removes empty user list");
                Assert.IsTrue(userRoleManager.GetAllUsers().Count() == 5, "RemoveUserAsync removes empty user list");

                // Remove null user
                res = await userRoleManager.RemoveUserAsync(new List<AvgIdentityUser>() { null });
                Assert.IsFalse(res, "RemoveUserAsync removes empty user list");
                Assert.IsTrue(userRoleManager.GetAllUsers().Count() == 5, "RemoveUserAsync removes empty user list");
            }
        }

        [TestMethod]
        public async Task RemoveUsersValidInputTestsAsync()
        {
            using (var userRoleManagerMock = new IdentityTestContext())
            {
                var userRoleManager = userRoleManagerMock.UserRoleManager;
                bool res;

                // Add roles and users
                await userRoleManager.AddRoleAsync(new[] { "Role1", "Role2", "Role3", "Role4", "Role5" });
                await userRoleManager.AddUserAsync("user1@test.com", "Password@1", null, null, null, null, "Role1");
                await userRoleManager.AddUserAsync("user2@test.com", "Password@1", null, null, null, null, "Role1");
                await userRoleManager.AddUserAsync("user3@test.com", "Password@1", null, null, null, null, "Role1");
                await userRoleManager.AddUserAsync("user4@test.com", "Password@1", null, null, null, null, "Role1");
                await userRoleManager.AddUserAsync("user5@test.com", "Password@1", null, null, null, null, "Role1");

                Assert.IsTrue(userRoleManager.GetAllUsers().Count() == 5, "AddUserAsync do not add valid user");

                var user = userRoleManager.GetUser("user1@test.com");
                Assert.IsNotNull(user, "Cannot get user");

                res = await userRoleManager.RemoveUserAsync(user);
                Assert.IsTrue(res, "RemoveUserAsync do not delete valid user");
                Assert.IsTrue(userRoleManager.GetAllUsers().Count() == 4, "RemoveUserAsync do not delete valid user");

                res = await userRoleManager.RemoveUserAsync("user2@test.com");
                Assert.IsTrue(res, "RemoveUserAsync do not delete valid user");
                Assert.IsTrue(userRoleManager.GetAllUsers().Count() == 3, "RemoveUserAsync do not delete valid user");

                var users = userRoleManager.GetAllUsers().ToList();
                res = await userRoleManager.RemoveUserAsync(users);
                Assert.IsTrue(res, "RemoveUserAsync do not delete valid users");
                Assert.IsTrue(!userRoleManager.GetAllUsers().Any(), "RemoveUserAsync do not delete valid user");
            }
        }

        [TestMethod]
        public async Task UpdateUsersInvalidInputTestsAsync()
        {
            using (var userRoleManagerMock = new IdentityTestContext())
            {
                var userRoleManager = userRoleManagerMock.UserRoleManager;
                bool res;

                // Add roles and users
                await userRoleManager.AddRoleAsync("Role1");
                await userRoleManager.AddUserAsync("user1@test.com", "Password@1", null, null, null, null, "Role1");
                await userRoleManager.AddUserAsync("user2@test.com", "Password@1", null, null, null, null, "Role1");

                // Change null user        
                res = await userRoleManager.UpdateUserAsync(null);
                Assert.IsFalse(res, "UpdateUserAsync updates null user");

                // Change user with duplicate email
                var user = userRoleManager.GetUser("user1@test.com");
                user.Email = "user2@test.com";
                res = await userRoleManager.UpdateUserAsync(user);
                Assert.IsFalse(res, "UpdateUserAsync changes duplicate email");

                // Change user to invalid email
                user = userRoleManager.GetUser("user2@test.com");
                user.Email = "testemail";
                res = await userRoleManager.UpdateUserAsync(user);
                Assert.IsFalse(res, "Change user to invalid email possible");

                // Change user to long name
                user = userRoleManager.GetUser("user2@test.com");
                user.FirstName = LongStr;
                res = await userRoleManager.UpdateUserAsync(user);
                Assert.IsFalse(res, "Change user to long name possible");

                // Change user to long name
                user = userRoleManager.GetUser("user2@test.com");
                user.FirstName = "First";
                user.LastName = LongStr;
                res = await userRoleManager.UpdateUserAsync(user);
                Assert.IsFalse(res, "Change user to long name possible");

                // Change user to long name
                user = userRoleManager.GetUser("user2@test.com");
                user.FirstName = "First";
                user.LastName = "Last";
                user.PasswordQuestion = LongStr;
                res = await userRoleManager.UpdateUserAsync(user);
                Assert.IsFalse(res, "Change user to long name possible");

                // Change not existing user
                user = new AvgIdentityUser() { Email = "notexisting@email.test" };
                res = await userRoleManager.UpdateUserAsync(user);
                Assert.IsFalse(res, "UpdateUserAsync changes not existing user");
            }
        }

        [TestMethod]
        public async Task UpdateUsersValidInputTestsAsync()
        {
            using (var userRoleManagerMock = new IdentityTestContext())
            {
                var userRoleManager = userRoleManagerMock.UserRoleManager;
                bool res;

                // Add roles and users
                await userRoleManager.AddRoleAsync("Role1");
                await userRoleManager.AddUserAsync("user1@test.com", "Password@1", null, null, null, null, "Role1");
                await userRoleManager.AddUserAsync("user2@test.com", "Password@1", null, null, null, null, "Role1");

                // Change not existing user
                var user = userRoleManager.GetUser("user1@test.com");
                user.FirstName = "First";
                res = await userRoleManager.UpdateUserAsync(user);
                Assert.IsTrue(res, "UpdateUserAsync do not update user");
            }
        }
    }
}