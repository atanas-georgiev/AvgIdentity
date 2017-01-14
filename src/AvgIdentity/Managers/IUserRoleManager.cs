namespace AvgIdentity.Managers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using AvgIdentity.Models;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

    public interface IUserRoleManager<TUser, TContext>
        where TUser : AvgIdentityUser, new() where TContext : IdentityDbContext<TUser>
    {
        void AddRoles(IEnumerable<string> roles);

        Task<TUser> AddUserAsync(TUser user, string password, string role = null);

        Task<TUser> AddUserAsync(
            string email,
            string password,
            string question = null,
            string answer = null,
            string firstName = null,
            string lastName = null,
            string role = null);

        Task AddUserExternalLoginInfoAsync(TUser user, ExternalLoginInfo info);

        Task AddUserInRoleAsync(TUser user, string role);

        Task<bool> ChangePasswordAsync(TUser user, string oldPassword, string newPassword);

        Task<bool> CheckPasswordAsync(TUser user, string password);

        Task<bool> DeleteUserAsync(TUser user);

        Task<bool> DeleteUserAsync(string email);

        IQueryable<string> GetAllRoles();

        IQueryable<TUser> GetAllUsers();

        IQueryable<TUser> GetAllUsersinRole(string role);

        TUser GetUser(string email);

        bool RemoveRoles(IEnumerable<string> roles);

        Task<bool> ResetPasswordAsync(TUser user, string passwordAnswer, string newPassword);

        Task<bool> SignInAsync(TUser user, string password = null);

        Task<bool> SignInAsync(string email, string password = null);

        Task<bool> UpdateUserAsync(TUser user);
    }
}