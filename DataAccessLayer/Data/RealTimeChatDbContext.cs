﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using RealTimeChatApi.DataAccessLayer.Models;

namespace RealTimeChatApi.DataAccessLayer.Data
{
    public class RealTimeChatDbContext : IdentityDbContext<AppUser, IdentityRole, string>
    {
        public RealTimeChatDbContext(DbContextOptions<RealTimeChatDbContext> options) : base(options)
        {
        }

        //public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<IdentityUser>().ToTable("users");

            modelBuilder.Entity<AppUser>().ToTable("AspNetUsers").HasKey(u => u.Id);
            modelBuilder.Entity<IdentityRole>().ToTable("AspNetRoles").HasKey(r => r.Id);
            modelBuilder.Entity<IdentityUserRole<string>>().ToTable("AspNetUserRoles").HasKey(ur => new { ur.UserId, ur.RoleId });
            modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("AspNetUserClaims").HasKey(uc => uc.Id);
            modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("AspNetUserLogins").HasKey(ul => new { ul.LoginProvider, ul.ProviderKey });
            modelBuilder.Entity<IdentityUserToken<string>>().ToTable("AspNetUserTokens").HasKey(ut => new { ut.UserId, ut.LoginProvider, ut.Name });



        }
    }
}
