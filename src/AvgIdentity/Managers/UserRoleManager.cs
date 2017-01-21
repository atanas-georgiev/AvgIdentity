namespace AvgIdentity.Managers
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Threading.Tasks;

    using AvgIdentity.Models;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

    public class UserRoleManager<TUser, TContext> : IUserRoleManager<TUser, TContext>
        where TUser : AvgIdentityUser, new() where TContext : IdentityDbContext<TUser>
    {
        public UserRoleManager(UserManager<TUser> userManager, SignInManager<TUser> signInManager, TContext context)
        {
            this.UserManager = userManager;
            this.SignInManager = signInManager;
            this.Context = context;
        }

        protected TContext Context { get; set; }

        protected SignInManager<TUser> SignInManager { get; set; }

        protected UserManager<TUser> UserManager { get; set; }

        public async Task<bool> AddRoleAsync(IEnumerable<string> roles)
        {
            if (!this.CheckNonDbRoles(roles))
            {
                return false;
            }

            await this.Context.Roles.AddRangeAsync(roles.Select(r => new IdentityRole(r)));
            var result = await this.Context.SaveChangesAsync();
            return result != 0;
        }

        public async Task<bool> AddRoleAsync(string role)
        {
            if (!this.CheckNonDbRole(role))
            {
                return false;
            }

            await this.Context.Roles.AddAsync(new IdentityRole(role));
            var result = await this.Context.SaveChangesAsync();
            return result != 0;
        }

        public virtual async Task<TUser> AddUserAsync(TUser user, string password = null, string role = null)
        {
            if (role != null && !this.CheckDbRole(role))
            {
                return null;
            }

            if (!this.CheckNonDbUser(user))
            {
                return null;
            }

            user.UserName = user.Email;

            var hasher = new PasswordHasher<TUser>();

            if (!string.IsNullOrEmpty(user.PasswordAnswerHash))
            {
                user.PasswordAnswerHash = hasher.HashPassword(user, user.PasswordAnswerHash);
            }

            IdentityResult result;

            if (string.IsNullOrEmpty(password))
            {
                result = await this.UserManager.CreateAsync(user);
            }
            else
            {
                result = await this.UserManager.CreateAsync(user, password);
            }

            if (result.Succeeded)
            {
                if (role != null)
                {
                    await this.AddUserInRoleAsync(user, role);
                }

                return user;
            }

            return null;
        }

        public async Task<TUser> AddUserAsync(
            string email,
            string password = null,
            string question = null,
            string answer = null,
            string firstName = null,
            string lastName = null,
            string role = null)
        {
            var user = new TUser
                           {
                               Email = email,
                               PasswordQuestion = question,
                               PasswordAnswerHash = answer,
                               FirstName = firstName,
                               LastName = lastName
                           };
            return await this.AddUserAsync(user, password, role);
        }

        public async Task AddUserExternalLoginInfoAsync(TUser user, ExternalLoginInfo info)
        {
            await this.UserManager.AddLoginAsync(user, info);
        }

        public async Task<bool> AddUserInRoleAsync(TUser user, string role)
        {
            if (!this.CheckDbUser(user))
            {
                return false;
            }

            if (!this.CheckDbRole(role))
            {
                return false;
            }

            var roleDb = this.Context.Roles.FirstOrDefault(r => string.CompareOrdinal(r.Name, role) == 0);
            var userInRole = await this.CheckUserInRoleAsync(user, role);

            if (!userInRole)
            {
                await this.Context.UserRoles.AddAsync(
                    new IdentityUserRole<string>() { RoleId = roleDb.Id, UserId = user.Id });

                var result = await this.Context.SaveChangesAsync();
                return result != 0;
            }

            return false;
        }

        public async Task<bool> AddUserInRoleAsync(TUser user, IEnumerable<string> roles)
        {
            if (!this.CheckDbUser(user))
            {
                return false;
            }

            if (!this.CheckDbRoles(roles))
            {
                return false;
            }

            var rolesDbIds = this.Context.Roles.Where(r => roles.Contains(r.Name)).Select(r => r.Id);

            foreach (var rolesDbId in rolesDbIds)
            {
                if (!this.Context.UserRoles.Any(ur => ur.UserId == user.Id && ur.RoleId == rolesDbId))
                {
                    await this.Context.UserRoles.AddAsync(
                        new IdentityUserRole<string>() { UserId = user.Id, RoleId = rolesDbId });
                }
            }

            var result = await this.Context.SaveChangesAsync();
            return result != 0;
        }

        public async Task<bool> ChangePasswordAsync(TUser user, string oldPassword, string newPassword)
        {
            var result = await this.UserManager.ChangePasswordAsync(user, oldPassword, newPassword);
            return result.Succeeded;
        }

        public async Task<bool> CheckPasswordAsync(TUser user, string password)
        {
            return await this.UserManager.CheckPasswordAsync(user, password);
        }

        public async Task<bool> CheckUserInRoleAsync(TUser user, string role)
        {
            if (!this.CheckDbUser(user))
            {
                return false;
            }

            if (!this.CheckDbRole(role))
            {
                return false;
            }

            var roleDb = this.Context.Roles.FirstOrDefault(r => string.CompareOrdinal(r.Name, role) == 0);

            var userRole = this.Context.UserRoles.FirstOrDefault(ur => ur.UserId == user.Id && ur.RoleId == roleDb.Id);
            return userRole != null;
        }

        public IQueryable<string> GetAllRoles() => this.Context.Roles.Select(r => r.Name);

        public IQueryable<string> GetAllRolesForUser(TUser user)
        {
            if (!this.CheckDbUser(user))
            {
                return null;
            }

            var userRoles = this.Context.UserRoles.Where(ur => ur.UserId == user.Id).Select(r => r.RoleId);
            return this.Context.Roles.Where(r => userRoles.Contains(r.Id)).Select(r => r.Name);
        }

        public IQueryable<TUser> GetAllUsers() => this.UserManager.Users;

        public IQueryable<TUser> GetAllUsersInRole(string role)
        {
            if (!this.CheckDbRole(role))
            {
                return null;
            }

            var roleDb = this.Context.Roles.FirstOrDefault(x => x.Name == role);
            return this.Context.Users.Where(x => x.Roles.Any(r => r.RoleId == roleDb.Id));
        }

        public TUser GetUser(string email) => this.UserManager.Users.FirstOrDefault(u => u.Email == email);

        public async Task<bool> RemoveRoleAsync(string role)
        {
            if (!this.CheckDbRole(role))
            {
                return false;
            }

            var roleDb = this.Context.Roles.FirstOrDefault(x => x.Name == role);

            if (!this.GetAllUsersInRole(role).Any())
            {
                this.Context.Remove(roleDb);
                var result = await this.Context.SaveChangesAsync();
                return result != 0;
            }

            return false;
        }

        public async Task<bool> RemoveRoleAsync(IEnumerable<string> roles)
        {
            if (!this.CheckDbRoles(roles))
            {
                return false;
            }

            if (roles.All(role => !this.GetAllUsersInRole(role).Any()))
            {
                var rolesDb = this.Context.Roles.Where(x => roles.Contains(x.Name));

                this.Context.RemoveRange(rolesDb);
                var result = await this.Context.SaveChangesAsync();
                return result != 0;
            }

            return false;
        }

        public async Task<bool> RemoveUserAsync(TUser user)
        {
            if (!this.CheckDbUser(user))
            {
                return false;
            }

            var result = await this.UserManager.DeleteAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> RemoveUserAsync(string email)
        {
            var user = this.GetUser(email);
            if (!this.CheckDbUser(user))
            {
                return false;
            }

            return await this.RemoveUserAsync(user);
        }

        public async Task<bool> RemoveUserAsync(IEnumerable<TUser> users)
        {
            if (users == null || !users.Any())
            {
                return false;
            }

            if (users.Any(user => !this.CheckDbUser(user)))
            {
                return false;
            }

            foreach (var user in users)
            {
                await this.RemoveUserAsync(user);
            }

            return true;
        }

        public async Task<bool> RemoveUserFromRoleAsync(TUser user, string role)
        {
            if (!this.CheckDbUser(user))
            {
                return false;
            }

            if (!this.CheckDbRole(role))
            {
                return false;
            }

            var res = await this.CheckUserInRoleAsync(user, role);
            if (res == false)
            {
                return false;
            }

            var roleDb = this.Context.Roles.First(r => string.CompareOrdinal(r.Name, role) == 0);
            var userRole = this.Context.UserRoles.FirstOrDefault(ur => ur.RoleId == roleDb.Id && ur.UserId == user.Id);
            this.Context.UserRoles.Remove(userRole);

            var result = await this.Context.SaveChangesAsync();
            return result != 0;
        }

        public async Task<bool> RemoveUserFromRoleAsync(TUser user, IEnumerable<string> roles)
        {
            if (!this.CheckDbUser(user))
            {
                return false;
            }

            if (!this.CheckDbRoles(roles))
            {
                return false;
            }

            var rolesDbIds = this.Context.Roles.Where(r => roles.Contains(r.Name)).Select(r => r.Id);
            var userRolesDb = this.Context.UserRoles.Where(ur => ur.UserId == user.Id);

            foreach (var userRoleDb in userRolesDb)
            {
                if (rolesDbIds.Contains(userRoleDb.RoleId))
                {
                    this.Context.UserRoles.Remove(userRoleDb);
                }
            }

            var result = await this.Context.SaveChangesAsync();
            return result != 0;
        }

        public async Task<bool> ResetPasswordAsync(TUser user, string passwordAnswer, string newPassword)
        {
            var hasher = new PasswordHasher<TUser>();
            var isPasswordCorrect = hasher.VerifyHashedPassword(user, user.PasswordAnswerHash, passwordAnswer);

            if (isPasswordCorrect == PasswordVerificationResult.Success)
            {
                var token = await this.UserManager.GeneratePasswordResetTokenAsync(user);
                var result = await this.UserManager.ResetPasswordAsync(user, token, newPassword);

                return result.Succeeded;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> SignInAsync(TUser user, string password = null)
        {
            if (!await this.SignInManager.CanSignInAsync(user))
            {
                return false;
            }

            if (password == null)
            {
                await this.SignInManager.SignInAsync(user, true);
                return true;
            }
            else
            {
                var result = await this.SignInManager.PasswordSignInAsync(user, password, true, false);

                if (result.Succeeded)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public async Task<bool> SignInAsync(string email, string password = null)
        {
            var user = this.GetUser(email);

            if (user == null)
            {
                return false;
            }

            return await this.SignInAsync(user, password);
        }

        public async Task<bool> UpdateUserAsync(TUser user)
        {
            if (!this.CheckDbUser(user))
            {
                return false;
            }

            user.UserName = user.Email;

            var result = await this.UserManager.UpdateAsync(user);
            return result.Succeeded;
        }

        private static bool IsValidEmail(string email)
        {
            return new EmailAddressAttribute().IsValid(email);
        }

        private bool CheckDbRole(string role)
        {
            if (string.IsNullOrEmpty(role))
            {
                return false;
            }

            var roleDb = this.Context.Roles.FirstOrDefault(r => string.CompareOrdinal(role, r.Name) == 0);
            return roleDb != null;
        }

        private bool CheckDbRoles(IEnumerable<string> roles)
        {
            if (roles == null)
            {
                return false;
            }

            if (!roles.Any())
            {
                return false;
            }

            return !roles.Any(string.IsNullOrEmpty)
                   && roles.All(
                       role => this.Context.Roles.FirstOrDefault(r => string.CompareOrdinal(r.Name, role) == 0) != null);
        }

        private bool CheckDbUser(TUser user)
        {
            if (user == null)
            {
                return false;
            }

            if (!IsValidEmail(user.Email))
            {
                return false;
            }

            if (user.FirstName != null && user.FirstName.Length > AvgIdentityUser.FirstNameMaxLen)
            {
                return false;
            }

            if (user.LastName != null && user.LastName.Length > AvgIdentityUser.LastNameMaxLen)
            {
                return false;
            }

            if (user.PasswordQuestion != null && user.PasswordQuestion.Length > AvgIdentityUser.PasswordQuestionMaxLen)
            {
                return false;
            }

            var userDb = this.Context.Users.FirstOrDefault(u => u.Id == user.Id);
            return userDb != null;
        }

        private bool CheckNonDbRole(string role)
        {
            if (string.IsNullOrEmpty(role))
            {
                return false;
            }

            var roleDb = this.Context.Roles.FirstOrDefault(r => string.CompareOrdinal(role, r.Name) == 0);
            return roleDb == null;
        }

        private bool CheckNonDbRoles(IEnumerable<string> roles)
        {
            if (roles == null)
            {
                return false;
            }

            if (!roles.Any())
            {
                return false;
            }

            return !roles.Any(string.IsNullOrEmpty)
                   && roles.All(role => !this.Context.Roles.Any(r => string.CompareOrdinal(r.Name, role) == 0));
        }

        private bool CheckNonDbUser(TUser user)
        {
            if (user == null)
            {
                return false;
            }

            if (!IsValidEmail(user.Email))
            {
                return false;
            }

            if (user.FirstName != null && user.FirstName.Length > AvgIdentityUser.FirstNameMaxLen)
            {
                return false;
            }

            if (user.LastName != null && user.LastName.Length > AvgIdentityUser.LastNameMaxLen)
            {
                return false;
            }

            if (user.PasswordQuestion != null && user.PasswordQuestion.Length > AvgIdentityUser.PasswordQuestionMaxLen)
            {
                return false;
            }

            var userDb = this.Context.Users.FirstOrDefault(u => u.Id == user.Id);
            return userDb == null;
        }
    }
}