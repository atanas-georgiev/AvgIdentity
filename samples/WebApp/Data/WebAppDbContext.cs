namespace WebApp.Data
{
    using AvgIdentity.Models;

    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;

    public class WebAppDbContext : IdentityDbContext<AvgIdentityUser>
    {
        public WebAppDbContext(DbContextOptions options)
            : base(options)
        {
        }
    }
}
