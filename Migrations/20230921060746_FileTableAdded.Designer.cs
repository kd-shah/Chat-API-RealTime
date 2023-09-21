﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RealTimeChatApi.DataAccessLayer.Data;

#nullable disable

namespace RealTimeChatApi.Migrations
{
    [DbContext(typeof(RealTimeChatDbContext))]
    [Migration("20230921060746_FileTableAdded")]
    partial class FileTableAdded
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ConcurrencyStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NormalizedName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("AspNetRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleId")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("RoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RoleId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId", "RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("RealTimeChatApi.DataAccessLayer.Models.AppUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NormalizedEmail")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NormalizedUserName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("AspNetUsers", (string)null);
                });

            modelBuilder.Entity("RealTimeChatApi.DataAccessLayer.Models.File", b =>
                {
                    b.Property<int>("fileId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("fileId"));

                    b.Property<string>("contentType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("fileName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("filePath")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("fileSize")
                        .HasColumnType("bigint");

                    b.Property<bool>("isRead")
                        .HasColumnType("bit");

                    b.Property<string>("receiverId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("senderId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("uploadDateTime")
                        .HasColumnType("datetime2");

                    b.HasKey("fileId");

                    b.HasIndex("receiverId");

                    b.HasIndex("senderId");

                    b.ToTable("Files");
                });

            modelBuilder.Entity("RealTimeChatApi.DataAccessLayer.Models.Log", b =>
                {
                    b.Property<int>("logId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("logId"));

                    b.Property<string>("ipAddress")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("requestBody")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("timeStamp")
                        .HasColumnType("datetime2");

                    b.HasKey("logId");

                    b.ToTable("Logs");
                });

            modelBuilder.Entity("RealTimeChatApi.DataAccessLayer.Models.Message", b =>
                {
                    b.Property<int>("messageId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("messageId"));

                    b.Property<string>("content")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("isRead")
                        .HasColumnType("bit");

                    b.Property<string>("receiverId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("senderId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("timestamp")
                        .HasColumnType("datetime2");

                    b.HasKey("messageId");

                    b.HasIndex("receiverId");

                    b.HasIndex("senderId");

                    b.ToTable("Messages");
                });

            modelBuilder.Entity("RealTimeChatApi.DataAccessLayer.Models.File", b =>
                {
                    b.HasOne("RealTimeChatApi.DataAccessLayer.Models.AppUser", "receiver")
                        .WithMany("receivedFiles")
                        .HasForeignKey("receiverId")
                        .IsRequired();

                    b.HasOne("RealTimeChatApi.DataAccessLayer.Models.AppUser", "sender")
                        .WithMany("sentFiles")
                        .HasForeignKey("senderId")
                        .IsRequired();

                    b.Navigation("receiver");

                    b.Navigation("sender");
                });

            modelBuilder.Entity("RealTimeChatApi.DataAccessLayer.Models.Message", b =>
                {
                    b.HasOne("RealTimeChatApi.DataAccessLayer.Models.AppUser", "receiver")
                        .WithMany("receivedMessages")
                        .HasForeignKey("receiverId")
                        .IsRequired();

                    b.HasOne("RealTimeChatApi.DataAccessLayer.Models.AppUser", "sender")
                        .WithMany("sentMessages")
                        .HasForeignKey("senderId")
                        .IsRequired();

                    b.Navigation("receiver");

                    b.Navigation("sender");
                });

            modelBuilder.Entity("RealTimeChatApi.DataAccessLayer.Models.AppUser", b =>
                {
                    b.Navigation("receivedFiles");

                    b.Navigation("receivedMessages");

                    b.Navigation("sentFiles");

                    b.Navigation("sentMessages");
                });
#pragma warning restore 612, 618
        }
    }
}
