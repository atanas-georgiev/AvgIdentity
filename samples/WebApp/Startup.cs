namespace WebApp
{
    using AutoMapper;

    using AvgIdentity.Extensions;
    using AvgIdentity.Managers;
    using AvgIdentity.Models;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    using WebApp.Data;

    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder =
                new ConfigurationBuilder().SetBasePath(env.ContentRootPath)
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                    .AddEnvironmentVariables();
            this.Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IServiceScopeFactory scopeFactory)
        {
            loggerFactory.AddConsole(this.Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.AddAvgIdentityMigration<WebAppDbContext, AvgIdentityUser>(scopeFactory, Configuration);

            app.UseMvc(
                routes => { routes.MapRoute(name: "default", template: "{controller=Home}/{action=Index}/{id?}"); });
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<WebAppDbContext>(o => o.UseSqlServer(this.Configuration.GetConnectionString("WebAppDb")));
            services.AddScoped<DbContext, WebAppDbContext>();
            services.Add(ServiceDescriptor.Scoped(typeof(IUserRoleManager<,>), typeof(UserRoleManager<,>)));

            services.AddAvgIdentityServices<WebAppDbContext, AvgIdentityUser>(this.Configuration);

            services.AddAutoMapper();

            // Add framework services.
            services.AddMvc();
        }
    }
}