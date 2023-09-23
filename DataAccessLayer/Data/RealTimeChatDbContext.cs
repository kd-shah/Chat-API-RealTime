using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using RealTimeChatApi.DataAccessLayer.Models;
using File = RealTimeChatApi.DataAccessLayer.Models.File;

namespace RealTimeChatApi.DataAccessLayer.Data
{
    public class RealTimeChatDbContext : IdentityDbContext<AppUser, IdentityRole, string>
    {
        public RealTimeChatDbContext(DbContextOptions<RealTimeChatDbContext> options) : base(options)
        {
        }

        public DbSet<Message> Messages { get; set; }

        public DbSet<Log> Logs { get; set; }

        public DbSet<File> Files { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<IdentityUser>().ToTable("users");

            modelBuilder.Entity<AppUser>().ToTable("AspNetUsers").HasKey(u => u.Id);
            modelBuilder.Entity<IdentityRole>().ToTable("AspNetRoles").HasKey(r => r.Id);
            modelBuilder.Entity<IdentityUserRole<string>>().ToTable("AspNetUserRoles").HasKey(ur => new { ur.UserId, ur.RoleId });
            modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("AspNetUserClaims").HasKey(uc => uc.Id);
            modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("AspNetUserLogins").HasKey(ul => new { ul.LoginProvider, ul.ProviderKey });
            modelBuilder.Entity<IdentityUserToken<string>>().ToTable("AspNetUserTokens").HasKey(ut => new { ut.UserId, ut.LoginProvider, ut.Name });


            modelBuilder.Entity<Message>()
             .HasOne(m => m.sender)
             .WithMany(u => u.sentMessages)
             .HasForeignKey(m => m.senderId)
             .OnDelete(DeleteBehavior.ClientSetNull);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.receiver)
                .WithMany(u => u.receivedMessages)
                .HasForeignKey(m => m.receiverId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            modelBuilder.Entity<File>()
                .HasOne(f => f.receiver)
                .WithMany(u => u.receivedFiles)
                .HasForeignKey(f => f.receiverId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            modelBuilder.Entity<File>()
                .HasOne(f => f.sender)
                .WithMany(u => u.sentFiles)
                .HasForeignKey(f => f.senderId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            
            
            modelBuilder.Entity<Message>()
                .HasOne(m => m.AttachedFile)
                .WithOne(f => f.Message)
                .HasForeignKey<File>(f => f.messageId);



        }
    }
}
