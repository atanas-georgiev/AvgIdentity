namespace AvgIdentity.Extensions
{
    using AvgIdentity.Exceptions;
    using AvgIdentity.Models;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static class ServicesExtensions
    {
        public static void AddAvgIdentityServices<TContext, TUser>(
            this IServiceCollection services,
            IConfiguration configuration) where TUser : AvgIdentityUser where TContext : IdentityDbContext<TUser>
        {
            PasswordOptions passwordOptions;

            try
            {
                passwordOptions = new PasswordOptions
                                      {
                                          RequireDigit =
                                              bool.Parse(
                                                  configuration[
                                                      "AvgIdentity:PasswordConfig:RequireDigit"]),
                                          RequireLowercase =
                                              bool.Parse(
                                                  configuration[
                                                      "AvgIdentity:PasswordConfig:RequireLowercase"]),
                                          RequireNonAlphanumeric =
                                              bool.Parse(
                                                  configuration[
                                                      "AvgIdentity:PasswordConfig:RequireNonAlphanumeric"
                                                  ]),
                                          RequireUppercase =
                                              bool.Parse(
                                                  configuration[
                                                      "AvgIdentity:PasswordConfig:RequireUppercase"]),
                                          RequiredLength =
                                              int.Parse(
                                                  configuration[
                                                      "AvgIdentity:PasswordConfig:RequiredLength"])
                                      };
            }
            catch
            {
                throw new AvgIdentityConfigurationException("AvgIdentity PasswordConfig error");
            }

            services.AddIdentity<TUser, IdentityRole>(o => o.Password = passwordOptions)
                .AddEntityFrameworkStores<TContext>()
                .AddDefaultTokenProviders();
        }
    }
}