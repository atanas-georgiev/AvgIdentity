namespace AvgIdentity.Test
{
    using AvgIdentity.Models;
    using AvgIdentity.Test.Mocks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Threading.Tasks;

    [TestClass]
    public class UserRoleTests
    {
        [TestMethod]
        public async Task AddRemoveUsersInRolesTestsAsync()
        {
            using (var userRoleManagerMock = new UserRoleManagerMock())
            {
                var userRoleManager = userRoleManagerMock.userRoleManager;
                AvgIdentityUser user;
                bool res;

                // Add roles and users
                await userRoleManager.AddRoleAsync(new[] { "Role1", "Role2", "Role3"});
                await userRoleManager.AddUserAsync("user1@test.com", "Password@1");
                await userRoleManager.AddUserAsync("user2@test.com", "Password@1");

                // Add user in role and check it
                user = userRoleManager.GetUser("user1@test.com");
                res = await userRoleManager.AddUserInRoleAsync(user, "Role1");
                res |= await userRoleManager.CheckUserInRoleAsync(user, "Role1");
                Assert.IsTrue(res, "User is not added to role");

                // Check if user is not in role
                res = await userRoleManager.CheckUserInRoleAsync(user, "Role2");
                Assert.IsFalse(res, "User is not in correct role");

                // Remove user from role and check it
                res = await userRoleManager.RemoveUserFromRoleAsync(user, "Role1");
                Assert.IsTrue(res, "User is not removed from role");
                res = await userRoleManager.CheckUserInRoleAsync(user, "Role1");
                Assert.IsFalse(res, "User is not removed from role");
            }
        }
    }
}
