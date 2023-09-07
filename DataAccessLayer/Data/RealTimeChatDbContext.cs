using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;


namespace RealTimeChatApi.DataAccessLayer.Data
{
    public class RealTimeChatDbContext : IdentityDbContext<IdentityUser>
    {
        public RealTimeChatDbContext(DbContextOptions<RealTimeChatDbContext> options) : base(options)
        {
        }

        //public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<IdentityUser>().ToTable("users");

            
        }
    }
}
