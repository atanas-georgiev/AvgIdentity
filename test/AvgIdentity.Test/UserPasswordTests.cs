namespace AvgIdentity.Test
{
    using System.Threading.Tasks;

    using AvgIdentity.Models;
    using AvgIdentity.Test.Mocks;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class UserPasswordTests
    {
        private const string LongStr =
            "longlonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglong";

        [TestMethod]
        public async Task UserPasswordInvalidInputTestsAsync()
        {
            using (var identityTestContext = new IdentityTestContext())
            {
                // Add user
                var user = await identityTestContext.UserRoleManager.AddUserAsync("user1@test.com", "Password@1");
                Assert.IsNotNull(user, "Cannot create user");

                // CheckPasswordAsync Check password for null user
                var res = await identityTestContext.UserRoleManager.CheckPasswordAsync(null, "Password@1");
                Assert.IsFalse(res, "Check password for null user return correct result");

                // CheckPasswordAsync Check password for not existing user
                res =
                    await identityTestContext.UserRoleManager.CheckPasswordAsync(
                        new AvgIdentityUser() { Email = "notexisting@mail.test" },
                        "Password@1");
                Assert.IsFalse(res, "Check password for not existing user return correct result");

                // CheckPasswordAsync Check null password
                res = await identityTestContext.UserRoleManager.CheckPasswordAsync(user, null);
                Assert.IsFalse(res, "Check null password returns correct result");

                // CheckPasswordAsync Check empty password
                res = await identityTestContext.UserRoleManager.CheckPasswordAsync(user, string.Empty);
                Assert.IsFalse(res, "Check empty password returns correct result");

                // HasPasswordAsync null user
                res = await identityTestContext.UserRoleManager.HasPasswordAsync(null);
                Assert.IsFalse(res, "HasPasswordAsync for null user return correct result");

                // HasPasswordAsync not existing user
                res =
                    await identityTestContext.UserRoleManager.HasPasswordAsync(
                        new AvgIdentityUser() { Email = "notexisting@mail.test" });
                Assert.IsFalse(res, "HasPasswordAsync for not existing user return correct result");

                // AddPasswordToUserAsync null user
                res = await identityTestContext.UserRoleManager.AddPasswordToUserAsync(null, "Password@1");
                Assert.IsFalse(res, "AddPasswordToUserAsync for null user return correct result");

                // AddPasswordToUserAsync not existing user
                res =
                    await identityTestContext.UserRoleManager.AddPasswordToUserAsync(
                        new AvgIdentityUser() { Email = "notexisting@mail.test" },
                        "Password@1");
                Assert.IsFalse(res, "AddPasswordToUserAsync for not existing user return correct result");

                // AddPasswordToUserAsync null password
                res = await identityTestContext.UserRoleManager.AddPasswordToUserAsync(user, null);
                Assert.IsFalse(res, "AddPasswordToUserAsync null password returns correct result");

                // AddPasswordToUserAsync empty password
                res = await identityTestContext.UserRoleManager.AddPasswordToUserAsync(user, string.Empty);
                Assert.IsFalse(res, "AddPasswordToUserAsync empty password returns correct result");

                // AddPasswordToUserAsync empty question and answer
                res = await identityTestContext.UserRoleManager.AddPasswordToUserAsync(
                          user,
                          "Password@1",
                          string.Empty,
                          string.Empty);
                Assert.IsFalse(res, "AddPasswordToUserAsync empty question and answer returns correct result");

                // AddPasswordToUserAsync long question
                res = await identityTestContext.UserRoleManager.AddPasswordToUserAsync(
                          user,
                          "Password@1",
                          LongStr,
                          "Answer");
                Assert.IsFalse(res, "AddPasswordToUserAsync long question returns correct result");

                // ResetPasswordAsync null user
                res = await identityTestContext.UserRoleManager.ResetPasswordAsync(null, "Password@1", "Password@2");
                Assert.IsFalse(res, "ResetPasswordAsync for null user return correct result");

                // ResetPasswordAsync not existing user
                res =
                    await identityTestContext.UserRoleManager.ResetPasswordAsync(
                        new AvgIdentityUser() { Email = "notexisting@mail.test" },
                        "Password@1",
                        "Password@2");
                Assert.IsFalse(res, "ResetPasswordAsync for not existing user return correct result");

                // ResetPasswordAsync null password
                res = await identityTestContext.UserRoleManager.ResetPasswordAsync(user, null, "Password@2");
                Assert.IsFalse(res, "ResetPasswordAsync null old password returns correct result");

                // ResetPasswordAsync empty password
                res = await identityTestContext.UserRoleManager.ResetPasswordAsync(user, string.Empty, "Password@2");
                Assert.IsFalse(res, "ResetPasswordAsync empty old password returns correct result");

                // ResetPasswordAsync null password
                res = await identityTestContext.UserRoleManager.ResetPasswordAsync(user, "Password@1", null);
                Assert.IsFalse(res, "ResetPasswordAsync null new password returns correct result");

                // ResetPasswordAsync empty password
                res = await identityTestContext.UserRoleManager.ResetPasswordAsync(user, "Password@1", string.Empty);
                Assert.IsFalse(res, "ResetPasswordAsync empty new password returns correct result");

                // ChangePasswordAsync null user
                res = await identityTestContext.UserRoleManager.ChangePasswordAsync(null, "Password@1", "Password@2");
                Assert.IsFalse(res, "ChangePasswordAsync for null user return correct result");

                // ChangePasswordAsync not existing user
                res =
                    await identityTestContext.UserRoleManager.ChangePasswordAsync(
                        new AvgIdentityUser() { Email = "notexisting@mail.test" },
                        "Password@1",
                        "Password@2");
                Assert.IsFalse(res, "ChangePasswordAsync for not existing user return correct result");

                // ChangePasswordAsync null password
                res = await identityTestContext.UserRoleManager.ChangePasswordAsync(user, null, "Password@2");
                Assert.IsFalse(res, "ChangePasswordAsync null old password returns correct result");

                // ChangePasswordAsync empty password
                res = await identityTestContext.UserRoleManager.ChangePasswordAsync(user, string.Empty, "Password@2");
                Assert.IsFalse(res, "ChangePasswordAsync empty old password returns correct result");

                // ChangePasswordAsync null password
                res = await identityTestContext.UserRoleManager.ChangePasswordAsync(user, "Password@1", null);
                Assert.IsFalse(res, "ChangePasswordAsync null new password returns correct result");

                // ChangePasswordAsync empty password
                res = await identityTestContext.UserRoleManager.ChangePasswordAsync(user, "Password@1", string.Empty);
                Assert.IsFalse(res, "ChangePasswordAsync empty new password returns correct result");
            }
        }

        [TestMethod]
        public async Task UserPasswordValidInputTestsAsync()
        {
            using (var identityTestContext = new IdentityTestContext())
            {
                // Add user
                var user1 = await identityTestContext.UserRoleManager.AddUserAsync("user@test.com");
                var user2 = await identityTestContext.UserRoleManager.AddUserAsync("user2@test.com", "Password@1");
                Assert.IsNotNull(user1, "Cannot create user");
                Assert.IsNotNull(user2, "Cannot create user");

                // Wrong password
                var res = await identityTestContext.UserRoleManager.CheckPasswordAsync(user2, "Wrong password");
                Assert.IsFalse(res, "Wrong password returns correct result");

                // Correct password
                res = await identityTestContext.UserRoleManager.CheckPasswordAsync(user2, "Password@1");
                Assert.IsTrue(res, "Correct password returns not correct result");

                // Has password
                res = await identityTestContext.UserRoleManager.HasPasswordAsync(user1);
                Assert.IsFalse(res, "HasPasswordAsync not correct result");
                res = await identityTestContext.UserRoleManager.HasPasswordAsync(user2);
                Assert.IsTrue(res, "HasPasswordAsync not correct result");
            }
        }
    }
}
