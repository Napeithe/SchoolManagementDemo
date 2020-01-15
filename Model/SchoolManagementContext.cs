using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Model.Domain;

namespace Model
{
    public class SchoolManagementContext : IdentityDbContext<User, Role, string>
    {
        public DbSet<ClassTime> ClassTimes { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<GroupClass> GroupClass { get; set; }
        public DbSet<GroupLevel> GroupLevel { get; set; }
        public DbSet<Pass> Passes { get; set; }
        public DbSet<ParticipantClassTime> ParticipantPresences { get; set; }
        public DbSet<ParticipantGroupClass> GroupClassMembers { get; set; }

        public SchoolManagementContext(DbContextOptions<SchoolManagementContext> options) :
            base(options)
        {
           
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            AddModificationDate();

            return base.SaveChangesAsync(cancellationToken);
        }

        private void AddModificationDate()
        {
            foreach (EntityEntry entry in this.ChangeTracker.Entries())
            {
                switch (entry.State)
                {
                    case EntityState.Modified:
                        if (entry.Entity is IEntity modifiedEntity)
                        {
                            modifiedEntity.ModificationDate = DateTime.UtcNow;
                        }

                        break;
                    case EntityState.Added:
                        if (entry.Entity is IEntity createdEntity)
                        {
                            createdEntity.CreatedDate = DateTime.UtcNow;
                        }

                        break;
                }
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Room>(x =>
            {
                x.Property(p => p.Name)
                    .IsRequired();
                x.HasIndex(p => p.Name).IsUnique();
                x.HasIndex(p => p.NormalizeName).IsUnique();
            });

            builder.Entity<Pass>(b =>
            {
                b.HasOne(x => x.Participant)
                    .WithMany(x => x.Passes)
                    .HasForeignKey(x => x.ParticipantId)
                    .OnDelete(DeleteBehavior.Cascade);

                b.Property(x => x.Status)
                    .HasConversion(x => x.ToString(),
                        x => (Pass.PassStatus)Enum.Parse(typeof(Pass.PassStatus), x));
            });

            builder.Entity<ParticipantClassTime>(b =>
            {
                b.Property(x => x.PresenceType)
                    .HasConversion(x => x.ToString(), 
                        x => (PresenceType) Enum.Parse(typeof(PresenceType), x));
                b.HasOne(x => x.Participant);
                b.HasOne(x => x.Pass)
                    .WithMany(x => x.ParticipantClassTimes)
                    .HasForeignKey(x => x.PassId)
                    .OnDelete(DeleteBehavior.SetNull);
                b.HasOne(x => x.MakeUpParticipant)
                    .WithOne()
                    .HasForeignKey<ParticipantClassTime>(x => x.MakeUpParticipantId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            builder.Entity<ClassDayOfWeek>(b =>
            {
                b.HasOne(x => x.GroupClass)
                    .WithMany(x => x.ClassDaysOfWeek)
                    .HasForeignKey(x => x.GroupClassId);
                b.Property(x => x.DayOfWeek)
                    .HasConversion(x => x.ToString(),
                        x => (DayOfWeek)Enum.Parse(typeof(DayOfWeek), x));
            });

            builder.Entity<ClassTime>(b =>
            {
                b.HasOne(x => x.GroupClass)
                    .WithMany(x => x.Schedule)
                    .HasForeignKey(x => x.GroupClassId);
                b.HasOne(x => x.Room)
                    .WithMany(x => x.Schedule)
                    .HasForeignKey(x => x.RoomId);
                b.HasMany(x => x.PresenceParticipants)
                    .WithOne(x => x.ClassTime)
                    .HasForeignKey(x => x.ClassTimeId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<GroupClass>(b =>
            {
                b.Property(x => x.Name).IsRequired();
                b.HasOne(x => x.GroupLevel);
                b.HasOne(x => x.Room);
                b.HasMany(x => x.Anchors)
                    .WithOne(x => x.GroupClass)
                    .HasForeignKey(x => x.GroupClassId)
                    .OnDelete(DeleteBehavior.Cascade);
                b.HasMany(x => x.Participants)
                    .WithOne(x => x.GroupClass)
                    .HasForeignKey(x => x.GroupClassId)
                    .OnDelete(DeleteBehavior.Cascade);
                b.HasMany(x => x.Schedule)
                    .WithOne(x => x.GroupClass)
                    .HasForeignKey(x => x.GroupClassId)
                    .OnDelete(DeleteBehavior.Cascade);
                b.HasMany(x => x.ClassDaysOfWeek)
                    .WithOne(x => x.GroupClass)
                    .HasForeignKey(x => x.GroupClassId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<AnchorGroupClass>(b =>
            {
                b.HasKey(x => new { x.UserId, x.GroupClassId });
                b.HasOne(x => x.User);
            });

            builder.Entity<ParticipantGroupClass>(b =>
            {
                b.HasKey(x => x.Id);
                b.Property(x => x.Role)
                    .HasConversion(x => x.ToString(),
                        x => (ParticipantRole)Enum.Parse(typeof(ParticipantRole), x));
                b.HasOne(x => x.User);

                b.HasMany(x => x.Passes)
                    .WithOne(x => x.ParticipantGroupClass)
                    .HasForeignKey(x=>x.ParticipantGroupClassId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

        }
    }
}
