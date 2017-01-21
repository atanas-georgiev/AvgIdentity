namespace AvgIdentity.Test.Mocks
{
    using AvgIdentity.Models;

    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;

    internal class TestInMemoryIdentityDbContext : IdentityDbContext<AvgIdentityUser>
    {
        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            builder.UseInMemoryDatabase();
            base.OnConfiguring(builder);
        }
    }
}