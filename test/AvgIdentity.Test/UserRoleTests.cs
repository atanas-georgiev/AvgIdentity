namespace AvgIdentity.Test
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using AvgIdentity.Models;
    using AvgIdentity.Test.Mocks;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class UserRoleTests
    {
        [TestMethod]
        public async Task AddRemoveUsersInRolesValidInputTestsAsync()
        {
            using (var userRoleManagerMock = new UserRoleManagerMock())
            {
                var userRoleManager = userRoleManagerMock.userRoleManager;
                bool res;

                // Add roles and users
                await userRoleManager.AddRoleAsync(new[] { "Role1", "Role2", "Role3" });
                await userRoleManager.AddUserAsync("user1@test.com", "Password@1");
                await userRoleManager.AddUserAsync("user2@test.com", "Password@1");

                // Add user in role and check it
                var user = userRoleManager.GetUser("user1@test.com");
                res = await userRoleManager.AddUserInRoleAsync(user, "Role1");
                Assert.IsTrue(res, "User is not added to role");
                res = await userRoleManager.CheckUserInRoleAsync(user, "Role1");
                Assert.IsTrue(res, "User is not added to role");

                // Check if user is not in role
                res = await userRoleManager.CheckUserInRoleAsync(user, "Role2");
                Assert.IsFalse(res, "User is not in correct role");

                // Remove user from role and check it
                res = await userRoleManager.RemoveUserFromRoleAsync(user, "Role1");
                Assert.IsTrue(res, "User is not removed from role");
                res = await userRoleManager.CheckUserInRoleAsync(user, "Role1");
                Assert.IsFalse(res, "User is not removed from role");

                // Add user in multiple roles
                user = userRoleManager.GetUser("user1@test.com");
                res = await userRoleManager.AddUserInRoleAsync(user, new[] { "Role1", "Role2", "Role3" });
                Assert.IsTrue(res, "User is not added to valid multiple roles");
                res = await userRoleManager.CheckUserInRoleAsync(user, "Role1");
                res &= await userRoleManager.CheckUserInRoleAsync(user, "Role2");
                res &= await userRoleManager.CheckUserInRoleAsync(user, "Role3");
                Assert.IsTrue(res, "User is not added to valid multiple roles");

                // Remove user from multiple roles
                res = await userRoleManager.RemoveUserFromRoleAsync(user, new[] { "Role1", "Role2", "Role3" });
                Assert.IsTrue(res, "User is not removed from valid multiple roles");
                res = await userRoleManager.CheckUserInRoleAsync(user, "Role1");
                res |= await userRoleManager.CheckUserInRoleAsync(user, "Role2");
                res |= await userRoleManager.CheckUserInRoleAsync(user, "Role3");
                Assert.IsFalse(res, "User is not removed from valid multiple roles");
            }
        }

        [TestMethod]
        public async Task AddUsersInRolesInvalidInputTestsAsync()
        {
            using (var userRoleManagerMock = new UserRoleManagerMock())
            {
                var userRoleManager = userRoleManagerMock.userRoleManager;
                bool res;

                // Add roles and users
                await userRoleManager.AddRoleAsync(new[] { "Role1", "Role2", "Role3" });
                await userRoleManager.AddUserAsync("user1@test.com", "Password@1");
                await userRoleManager.AddUserAsync("user2@test.com", "Password@1");
                var user = userRoleManager.GetUser("user1@test.com");

                // Add user to null role
                res = await userRoleManager.AddUserInRoleAsync(user, role: null);
                Assert.IsFalse(res, "Add user to null role returns correct result");

                // Add user to null roles
                res = await userRoleManager.AddUserInRoleAsync(user, roles: null);
                Assert.IsFalse(res, "Add user to null roles returns correct result");

                // Add user to null roles
                res = await userRoleManager.AddUserInRoleAsync(user, new[] { "Role1", null });
                Assert.IsFalse(res, "Add user to null roles returns correct result");

                // Add user to empty role
                res = await userRoleManager.AddUserInRoleAsync(user, string.Empty);
                Assert.IsFalse(res, "Add user to empty role returns correct result");

                // Add user to empty roles
                res = await userRoleManager.AddUserInRoleAsync(user, new[] { "Role1", string.Empty });
                Assert.IsFalse(res, "Add user to empty roles returns correct result");

                // Add user to empty roles
                res = await userRoleManager.AddUserInRoleAsync(user, new List<string>());
                Assert.IsFalse(res, "Add user to empty roles returns correct result");

                // Add user to not existing role
                res = await userRoleManager.AddUserInRoleAsync(user, "Role5");
                Assert.IsFalse(res, "Add user to not existing role returns correct result");

                // Add user to not existing roles
                res = await userRoleManager.AddUserInRoleAsync(user, new[] { "Role5", "Role6" });
                Assert.IsFalse(res, "Add user to not existing roles returns correct result");

                // Add role to null user
                res = await userRoleManager.AddUserInRoleAsync(null, "Role1");
                Assert.IsFalse(res, "Add role to null user returns correct result");

                // Add roles to null user
                res = await userRoleManager.AddUserInRoleAsync(null, new[] { "Role1", "Role2" });
                Assert.IsFalse(res, "Add roles to null user returns correct result");

                // Add role to not existing user
                user = new AvgIdentityUser() { Email = "notexisiting@mail.test" };
                res = await userRoleManager.AddUserInRoleAsync(user, "Role1");
                Assert.IsFalse(res, "Add role to not existing user returns correct result");

                // Add user to to role that is already assigned
                user = userRoleManager.GetUser("user1@test.com");
                res = await userRoleManager.AddUserInRoleAsync(user, "Role1");
                Assert.IsTrue(res, "User cannot be added to role");
                res = await userRoleManager.AddUserInRoleAsync(user, "Role1");
                Assert.IsFalse(res, "Add user to to role that is already assigned returns correct result");

                // Add user to to roles that is already assigned
                user = userRoleManager.GetUser("user1@test.com");
                res = await userRoleManager.AddUserInRoleAsync(user, "Role2");
                Assert.IsTrue(res, "User cannot be added to role");
                res = await userRoleManager.AddUserInRoleAsync(user, new[] { "Role1", "Role2" });
                Assert.IsFalse(res, "Add user to to roles that is already assigned returns correct result");
            }
        }

        [TestMethod]
        public async Task CheckUsersInRolesInvalidInputTestsAsync()
        {
            using (var userRoleManagerMock = new UserRoleManagerMock())
            {
                var userRoleManager = userRoleManagerMock.userRoleManager;
                bool res;

                // Add roles and users
                await userRoleManager.AddRoleAsync(new[] { "Role1", "Role2", "Role3" });
                await userRoleManager.AddUserAsync("user1@test.com", "Password@1");
                await userRoleManager.AddUserAsync("user2@test.com", "Password@1");
                var user = userRoleManager.GetUser("user1@test.com");

                // Check null role
                res = await userRoleManager.CheckUserInRoleAsync(user, role: null);
                Assert.IsFalse(res, "Check null role returns correct result");

                // Check empty role
                res = await userRoleManager.CheckUserInRoleAsync(user, role: string.Empty);
                Assert.IsFalse(res, "Check empty role returns correct result");

                // Check null user
                res = await userRoleManager.CheckUserInRoleAsync(null, "Role1");
                Assert.IsFalse(res, "Check null user returns correct result");

                // Check not existing role
                res = await userRoleManager.CheckUserInRoleAsync(user, "Role5");
                Assert.IsFalse(res, "Check not existing role returns correct result");

                // Check not existing user
                user = new AvgIdentityUser() { Email = "notexisiting@mail.test" };
                res = await userRoleManager.CheckUserInRoleAsync(user, "Role1");
                Assert.IsFalse(res, "Check not existing user returns correct result");
            }
        }

        [TestMethod]
        public async Task RemoveUsersFromRolesInvalidInputTestsAsync()
        {
            using (var userRoleManagerMock = new UserRoleManagerMock())
            {
                var userRoleManager = userRoleManagerMock.userRoleManager;
                bool res;

                // Add roles and users
                await userRoleManager.AddRoleAsync(new[] { "Role1", "Role2", "Role3" });
                await userRoleManager.AddUserAsync("user1@test.com", "Password@1");
                await userRoleManager.AddUserAsync("user2@test.com", "Password@1");
                var user = userRoleManager.GetUser("user1@test.com");

                // Remove user from null role
                res = await userRoleManager.RemoveUserFromRoleAsync(user, role: null);
                Assert.IsFalse(res, "Remove user from null role returns correct result");

                // Remove user from null roles
                res = await userRoleManager.RemoveUserFromRoleAsync(user, roles: null);
                Assert.IsFalse(res, "Remove user from null roles returns correct result");

                // Remove user from null roles
                res = await userRoleManager.RemoveUserFromRoleAsync(user, new[] { "Role1", null });
                Assert.IsFalse(res, "Remove user from null roles returns correct result");

                // Remove user from empty role
                res = await userRoleManager.RemoveUserFromRoleAsync(user, string.Empty);
                Assert.IsFalse(res, "Remove user from empty role returns correct result");

                // Remove user from empty roles
                res = await userRoleManager.RemoveUserFromRoleAsync(user, new[] { "Role1", string.Empty });
                Assert.IsFalse(res, "Remove user from empty roles returns correct result");

                // Remove user from empty roles
                res = await userRoleManager.RemoveUserFromRoleAsync(user, new List<string>());
                Assert.IsFalse(res, "Remove user from empty roles returns correct result");

                // Remove user from not existing role
                res = await userRoleManager.RemoveUserFromRoleAsync(user, "Role5");
                Assert.IsFalse(res, "Remove user from not existing role returns correct result");

                // Remove user from not existing roles
                res = await userRoleManager.RemoveUserFromRoleAsync(user, new[] { "Role1", "Role2" });
                Assert.IsFalse(res, "Remove user from not existing roles returns correct result");

                // Remove role from null user
                res = await userRoleManager.RemoveUserFromRoleAsync(null, "Role1");
                Assert.IsFalse(res, "Remove role from null user returns correct result");

                // Remove roles from null user
                res = await userRoleManager.RemoveUserFromRoleAsync(null, new[] { "Role5", "Role6" });
                Assert.IsFalse(res, "Removes role from null user returns correct result");

                // Remove role from not existing user
                user = new AvgIdentityUser() { Email = "notexisiting@mail.test" };
                res = await userRoleManager.RemoveUserFromRoleAsync(user, "Role1");
                Assert.IsFalse(res, "Remove role from not existing user returns correct result");

                // Remove roles from not existing user
                res = await userRoleManager.RemoveUserFromRoleAsync(user, new[] { "Role1", "Role2" });
                Assert.IsFalse(res, "Remove roles from not existing user returns correct result");

                // Remove role from role that is not assigned
                user = userRoleManager.GetUser("user1@test.com");
                res = await userRoleManager.RemoveUserFromRoleAsync(user, "Role1");
                Assert.IsFalse(res, "Remove role from role that is not assigned returns correct result");

                // Remove role from roles that is not assigned
                res = await userRoleManager.RemoveUserFromRoleAsync(user, new[] { "Role1", "Role2" });
                Assert.IsFalse(res, "Remove roles from roles that is not assigned returns correct result");
            }
        }
    }
}