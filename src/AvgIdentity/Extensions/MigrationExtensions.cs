namespace AvgIdentity.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    using AvgIdentity.Exceptions;
    using AvgIdentity.Managers;
    using AvgIdentity.Models;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static class MigrationExtensions
    {
        public static async Task AddAvgIdentityMigration<TContext, TUser>(
            this IApplicationBuilder app,
            IServiceScopeFactory scopeFactory,
            IConfiguration configuration) where TUser : AvgIdentityUser, new() where TContext : IdentityDbContext<TUser>
        {
            using (var serviceScope = scopeFactory.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<TContext>();
                var userRoleManager = serviceScope.ServiceProvider.GetService<IUserRoleManager<TUser, TContext>>();

                try
                {
                    if (!context.Roles.Any())
                    {
                        SeedRoles(userRoleManager, configuration);
                        await SeedUsers(userRoleManager, configuration);
                    }
                }
                catch
                {
                    context.Database.Migrate();

                    if (!context.Roles.Any())
                    {
                        SeedRoles(userRoleManager, configuration);
                        await SeedUsers(userRoleManager, configuration);
                    }
                }
            }
        }

        private static void SeedRoles<TUser, TContext>(
            IUserRoleManager<TUser, TContext> userRoleManager,
            IConfiguration configuration) where TUser : AvgIdentityUser, new() where TContext : IdentityDbContext<TUser>
        {
            var roles = new List<string>();

            try
            {
                roles =
                    configuration.GetSection("AvgIdentity")
                        .GetSection("InitialData")
                        .GetSection("Roles")
                        .GetChildren()
                        .Select(c => c.Value)
                        .ToList();
            }
            catch
            {
                throw new AvgIdentityConfigurationException("AvgIdentity InitialData error");
            }

            // TODO: asdada
            // userRoleManager.AddRoles(roles);
        }

        private static async Task SeedUsers<TUser, TContext>(
            IUserRoleManager<TUser, TContext> userRoleManager,
            IConfiguration configuration) where TUser : AvgIdentityUser, new() where TContext : IdentityDbContext<TUser>
        {
            try
            {
                // TODO: add reflection
                var userref = typeof(TUser).GetTypeInfo().GetProperties();

                var users =
                    configuration.GetSection("AvgIdentity")
                        .GetSection("InitialData")
                        .GetSection("Users")
                        .GetChildren()
                        .Select(
                            c =>
                                new
                                    {
                                        FirstName = c.GetSection("FirstName").Value,
                                        LastName = c.GetSection("LastName").Value,
                                        Email = c.GetSection("Email").Value,
                                        Password = c.GetSection("Password").Value,
                                        Role = c.GetSection("Role").Value
                                    });

                foreach (var user in users)
                {
                    await userRoleManager.AddUserAsync(
                        user.Email,
                        user.Password,
                        user.FirstName,
                        user.LastName,
                        user.Role);
                }
            }
            catch (Exception ex)
            {
                throw new AvgIdentityConfigurationException("AvgIdentity InitialData error");
            }
        }
    }
}