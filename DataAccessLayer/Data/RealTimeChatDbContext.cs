using Microsoft.EntityFrameworkCore;
using RealTimeChatApi.Models;

namespace RealTimeChatApi.DataAccessLayer.Data
{
    public class RealTimeChatDbContext : DbContext
    {
        public RealTimeChatDbContext(DbContextOptions<RealTimeChatDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("users");

            
        }
    }
}
