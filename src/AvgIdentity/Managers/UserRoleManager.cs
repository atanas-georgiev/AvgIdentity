﻿namespace AvgIdentity.Managers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using AvgIdentity.Models;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

    public class UserRoleManager<TUser, TContext> : IUserRoleManager<TUser, TContext>
        where TUser : AvgIdentityUser, new() where TContext : IdentityDbContext<TUser>
    {
        protected const string InitialPassword = "changeme";

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
            if (roles == null)
            {
                return false;
            }

            var rolesList = roles as IList<string> ?? roles.ToList();
            if (rolesList.Any(string.IsNullOrEmpty))
            {
                return false;
            }

            if (this.Context.Roles.Any(r => rolesList.Contains(r.Name)))
            {
                return false;
            }

            await this.Context.Roles.AddRangeAsync(rolesList.Select(r => new IdentityRole(r)));
            var result = await this.Context.SaveChangesAsync();
            return result != 0;
        }

        public async Task<bool> AddRoleAsync(string role)
        {
            if (string.IsNullOrEmpty(role))
            {
                return false;
            }

            if (this.Context.Roles.Any(r => string.CompareOrdinal(r.Name, role) == 0))
            {
                return false;
            }

            await this.Context.Roles.AddAsync(new IdentityRole(role));
            var result = await this.Context.SaveChangesAsync();
            return result != 0;
        }

        public virtual async Task<TUser> AddUserAsync(TUser user, string password, string role = null)
        {
            if (string.IsNullOrEmpty(password))
            {
                return null;
            }

            if(string.CompareOrdinal(role, string.Empty) == 0)
            {
                return null;
            }

            if (role != null)
            {
                var roleDb = this.GetAllRoles().FirstOrDefault(x => x == role);

                if (roleDb == null)
                {
                    return null;
                }
            }

            if (user != null)
            {
                user.UserName = user.Email;

                var hasher = new PasswordHasher<TUser>();

                if (!string.IsNullOrEmpty(user.PasswordAnswerHash))
                {
                    user.PasswordAnswerHash = hasher.HashPassword(user, user.PasswordAnswerHash);
                }

                var result = await this.UserManager.CreateAsync(user, password ?? InitialPassword);

                if (result.Succeeded)
                {
                    if (role != null)
                    {
                        await this.AddUserInRoleAsync(user, role);
                    }

                    return user;
                }
            }

            return null;
        }

        public async Task<TUser> AddUserAsync(
            string email,
            string password,
            string question = null,
            string answer = null,
            string firstName = null,
            string lastName = null,
            string role = null)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                return null;
            }

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
            if (user == null)
            {
                return false;
            }

            var userDb = this.GetUser(user.Email);
            if (userDb == null)
            {
                return false;
            }

            if (string.IsNullOrEmpty(role))
            {
                return false;
            }

            var roleDb = this.Context.Roles.First(r => string.CompareOrdinal(r.Name, role) == 0);
            if (roleDb == null)
            {
                return false;
            }

            var userInRole = await this.UserManager.IsInRoleAsync(user, role);
            if (!userInRole)
            {
                await this.Context.UserRoles.AddAsync(
                    new IdentityUserRole<string>()
                    {
                        RoleId = roleDb.Id,
                        UserId = userDb.Id
                    });

                var result = await this.Context.SaveChangesAsync();
                return result != 0;
            }

            return false;
        }

        public Task<bool> AddUserInRoleAsync(TUser user, IEnumerable<string> roles)
        {
            return null;
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

        public async Task<bool> RemoveUserAsync(TUser user)
        {
            if (user == null)
            {
                return false;
            }

            var result = await this.UserManager.DeleteAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> RemoveUserAsync(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return false;
            }

            var user = this.GetUser(email);

            if (user == null)
            {
                return false;
            }

            return await this.RemoveUserAsync(user);
        }

        public IQueryable<string> GetAllRoles() => this.Context.Roles.Select(r => r.Name);

        public IQueryable<TUser> GetAllUsers() => this.UserManager.Users;

        public IQueryable<TUser> GetAllUsersinRole(string role)
        {
            var roleDb = this.Context.Roles.FirstOrDefault(x => x.Name == role);

            if (roleDb != null)
            {
                return this.Context.Users.Where(x => x.Roles.Any(r => r.RoleId == roleDb.Id));
            }

            return new List<TUser>().AsQueryable();
        }

        public TUser GetUser(string email) => this.UserManager.Users.FirstOrDefault(u => u.Email == email);

        public async Task<bool> RemoveRoleAsync(string role)
        {
            if (string.IsNullOrEmpty(role))
            {
                return false;
            }

            var roleDb = this.Context.Roles.FirstOrDefault(x => x.Name == role);
            if (roleDb == null)
            {
                return false;
            }

            if (!this.GetAllUsersinRole(role).Any())
            {
                this.Context.Remove(roleDb);
                var result = await this.Context.SaveChangesAsync();
                return result != 0;
            }

            return false;
        }

        public async Task<bool> RemoveRoleAsync(IEnumerable<string> roles)
        {
            if (roles == null)
            {
                return false;
            }

            if (roles.Count() == 0)
            {
                return false;
            }

            foreach (var role in roles)
            {
                if (string.IsNullOrEmpty(role))
                {
                    return false;
                }
            }

            if (roles.All(role => !this.GetAllUsersinRole(role).Any()))
            {
                var rolesDb = this.Context.Roles.Where(x => roles.Contains(x.Name));

                this.Context.RemoveRange(rolesDb);
                var result = await this.Context.SaveChangesAsync();
                return result != 0;
            }

            return false;
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
            if (user == null)
            {
                return false;
            }

            user.UserName = user.Email;

            var result = await this.UserManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> RemoveUserAsync(IEnumerable<TUser> users)
        {
            if (users == null)
            {
                return false;
            }

            if (users.Count() == 0)
            {
                return false;
            }

            foreach (var user in users)
            {
                var result = await this.RemoveUserAsync(user);

                if (!result)
                {
                    return false;
                }
            }

            return true;
        }

        public async Task<bool> RemoveUserFromRoleAsync(TUser user, string role)
        {
            // todo: check this
            //var res = await this.CheckUserInRoleAsync(user, role);
            //if (res == false)
            //{
            //    return false;
            //}

            var userDb = this.GetUser(user.Email);
            var roleDb = this.Context.Roles.First(r => string.CompareOrdinal(r.Name, role) == 0);

            var userRole = this.Context.UserRoles.FirstOrDefault(ur => ur.RoleId == roleDb.Id && ur.UserId == userDb.Id);

            if (userRole == null)
            {
                return false;
            }

            this.Context.UserRoles.Remove(userRole);

            var result = await this.Context.SaveChangesAsync();
            return result != 0;
        }

        public Task<bool> RemoveUserFromRoleAsync(TUser user, IEnumerable<string> roles)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> CheckUserInRoleAsync(TUser user, string role)
        {
            if (user == null)
            {
                return false;
            }

            var userDb = this.GetUser(user.Email);
            if (userDb == null)
            {
                return false;
            }

            if (string.IsNullOrEmpty(role))
            {
                return false;
            }

            var roleDb = this.Context.Roles.First(r => string.CompareOrdinal(r.Name, role) == 0);
            if (roleDb == null)
            {
                return false;
            }

            return await this.UserManager.IsInRoleAsync(user, role);
        }
    }
}