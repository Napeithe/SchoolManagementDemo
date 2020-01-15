﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Model;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace SchoolManagement.Migrations
{
    [DbContext(typeof(SchoolManagementContext))]
    [Migration("20191110213222_AddNumberOfClass")]
    partial class AddNumberOfClass
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.2.4-servicing-10062")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("Model.Domain.AnchorGroupClass", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<int>("GroupClassId");

                    b.Property<DateTime>("CreatedDate");

                    b.Property<DateTime>("ModificationDate");

                    b.HasKey("UserId", "GroupClassId");

                    b.HasIndex("GroupClassId");

                    b.ToTable("AnchorGroupClass");
                });

            modelBuilder.Entity("Model.Domain.ClassDayOfWeek", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("DayOfWeek")
                        .IsRequired();

                    b.Property<int>("GroupClassId");

                    b.Property<TimeSpan>("Hour");

                    b.Property<DateTime>("ModificationDate");

                    b.HasKey("Id");

                    b.HasIndex("GroupClassId");

                    b.ToTable("ClassDayOfWeek");
                });

            modelBuilder.Entity("Model.Domain.ClassTime", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedDate");

                    b.Property<DateTime>("EndDate");

                    b.Property<int>("GroupClassId");

                    b.Property<DateTime>("ModificationDate");

                    b.Property<int>("NumberOfClass");

                    b.Property<int>("NumberOfClasses");

                    b.Property<int>("RoomId");

                    b.Property<DateTime>("StartDate");

                    b.HasKey("Id");

                    b.HasIndex("GroupClassId");

                    b.HasIndex("RoomId");

                    b.ToTable("ClassTimes");
                });

            modelBuilder.Entity("Model.Domain.GroupClass", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedDate");

                    b.Property<int>("DurationTimeInMinutes");

                    b.Property<int?>("GroupLevelId");

                    b.Property<bool>("IsSolo");

                    b.Property<DateTime>("ModificationDate");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<int>("NumberOfClasses");

                    b.Property<int>("ParticipantLimits");

                    b.Property<int>("PassPrice");

                    b.Property<int?>("RoomId");

                    b.Property<DateTime>("StartClasses");

                    b.HasKey("Id");

                    b.HasIndex("GroupLevelId");

                    b.HasIndex("RoomId");

                    b.ToTable("GroupClass");
                });

            modelBuilder.Entity("Model.Domain.GroupLevel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedDate");

                    b.Property<int>("Level");

                    b.Property<DateTime>("ModificationDate");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("GroupLevel");
                });

            modelBuilder.Entity("Model.Domain.ParticipantClassTime", b =>
                {
                    b.Property<int>("ClassTimeId");

                    b.Property<string>("ParticipantId");

                    b.Property<DateTime>("CreatedDate");

                    b.Property<DateTime>("ModificationDate");

                    b.Property<int?>("PassId");

                    b.Property<string>("PresenceType")
                        .IsRequired();

                    b.Property<bool>("WasPresence");

                    b.HasKey("ClassTimeId", "ParticipantId");

                    b.HasIndex("ParticipantId");

                    b.HasIndex("PassId");

                    b.ToTable("ParticipantPresences");
                });

            modelBuilder.Entity("Model.Domain.ParticipantGroupClass", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedDate");

                    b.Property<int>("GroupClassId");

                    b.Property<DateTime>("ModificationDate");

                    b.Property<string>("Role")
                        .IsRequired();

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("GroupClassId");

                    b.HasIndex("UserId");

                    b.ToTable("GroupClassMembers");
                });

            modelBuilder.Entity("Model.Domain.Pass", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedDate");

                    b.Property<bool>("IsStudent");

                    b.Property<DateTime>("ModificationDate");

                    b.Property<int>("NumberOfEntry");

                    b.Property<bool>("Paid");

                    b.Property<int>("ParticipantGroupClassId");

                    b.Property<string>("ParticipantId");

                    b.Property<int>("PassNumber");

                    b.Property<int>("Price");

                    b.Property<DateTime>("Start");

                    b.Property<string>("Status")
                        .IsRequired();

                    b.Property<int>("Used");

                    b.Property<bool>("WasGenerateAutomatically");

                    b.HasKey("Id");

                    b.HasIndex("ParticipantGroupClassId");

                    b.HasIndex("ParticipantId");

                    b.ToTable("Passes");
                });

            modelBuilder.Entity("Model.Domain.Role", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Description");

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Model.Domain.Room", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("HexColor");

                    b.Property<DateTime>("ModificationDate");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("NormalizeName");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.HasIndex("NormalizeName")
                        .IsUnique();

                    b.ToTable("Rooms");
                });

            modelBuilder.Entity("Model.Domain.User", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<string>("FacebookLink");

                    b.Property<string>("FirstName");

                    b.Property<string>("LastName");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Model.Domain.Role")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Model.Domain.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Model.Domain.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Model.Domain.Role")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Model.Domain.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("Model.Domain.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Model.Domain.AnchorGroupClass", b =>
                {
                    b.HasOne("Model.Domain.GroupClass", "GroupClass")
                        .WithMany("Anchors")
                        .HasForeignKey("GroupClassId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Model.Domain.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Model.Domain.ClassDayOfWeek", b =>
                {
                    b.HasOne("Model.Domain.GroupClass", "GroupClass")
                        .WithMany("ClassDaysOfWeek")
                        .HasForeignKey("GroupClassId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Model.Domain.ClassTime", b =>
                {
                    b.HasOne("Model.Domain.GroupClass", "GroupClass")
                        .WithMany("Schedule")
                        .HasForeignKey("GroupClassId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Model.Domain.Room", "Room")
                        .WithMany("Schedule")
                        .HasForeignKey("RoomId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Model.Domain.GroupClass", b =>
                {
                    b.HasOne("Model.Domain.GroupLevel", "GroupLevel")
                        .WithMany()
                        .HasForeignKey("GroupLevelId");

                    b.HasOne("Model.Domain.Room", "Room")
                        .WithMany()
                        .HasForeignKey("RoomId");
                });

            modelBuilder.Entity("Model.Domain.ParticipantClassTime", b =>
                {
                    b.HasOne("Model.Domain.ClassTime", "ClassTime")
                        .WithMany("PresenceParticipants")
                        .HasForeignKey("ClassTimeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Model.Domain.User", "Participant")
                        .WithMany()
                        .HasForeignKey("ParticipantId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Model.Domain.Pass", "Pass")
                        .WithMany("ParticipantClassTimes")
                        .HasForeignKey("PassId")
                        .OnDelete(DeleteBehavior.SetNull);
                });

            modelBuilder.Entity("Model.Domain.ParticipantGroupClass", b =>
                {
                    b.HasOne("Model.Domain.GroupClass", "GroupClass")
                        .WithMany("Participants")
                        .HasForeignKey("GroupClassId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Model.Domain.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Model.Domain.Pass", b =>
                {
                    b.HasOne("Model.Domain.ParticipantGroupClass", "ParticipantGroupClass")
                        .WithMany("Passes")
                        .HasForeignKey("ParticipantGroupClassId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Model.Domain.User", "Participant")
                        .WithMany("Passes")
                        .HasForeignKey("ParticipantId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
