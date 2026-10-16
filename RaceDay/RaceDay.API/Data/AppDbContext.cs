
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Models;

namespace RaceDay.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // Database tables
        public DbSet<User> Users => Set<User>();
        public DbSet<Participant> Participants => Set<Participant>();
        public DbSet<Organiser> Organisers => Set<Organiser>();
        public DbSet<Event> Events => Set<Event>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Enrollment> Enrollments => Set<Enrollment>();
        public DbSet<Result> Results => Set<Result>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User: unique email and valid roles
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(u => u.Email).IsUnique();

                entity.ToTable(t => t.HasCheckConstraint(
                    "CK_User_Role",
                    "[Role] IN ('Participant', 'Organiser')"));

                entity.Property(u => u.CreatedAt)
                    .HasDefaultValueSql("GETDATE()");
            });

            // User -> Participant: one-to-zero-or-one
            modelBuilder.Entity<Participant>(entity =>
            {
                entity.HasIndex(p => p.UserID).IsUnique();

                entity.HasOne(p => p.User)
                    .WithOne()
                    .HasForeignKey<Participant>(p => p.UserID)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.Property(p => p.DateOfBirth)
                    .HasColumnType("date");
            });

            // User -> Organiser: one-to-zero-or-one
            modelBuilder.Entity<Organiser>(entity =>
            {
                entity.HasIndex(o => o.UserID).IsUnique();

                entity.HasOne(o => o.User)
                    .WithOne()
                    .HasForeignKey<Organiser>(o => o.UserID)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // Organiser -> Event: one-to-many
            modelBuilder.Entity<Event>(entity =>
            {
                entity.HasOne(e => e.Organiser)
                    .WithMany()
                    .HasForeignKey(e => e.OrganiserID)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.Property(e => e.EventDate)
                    .HasColumnType("date");

                entity.ToTable(t => t.HasCheckConstraint(
                    "CK_Event_Distance",
                    "[Distance] > 0"));
            });

            // Event -> Category: one-to-many
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasOne(c => c.Event)
                    .WithMany()
                    .HasForeignKey(c => c.EventID)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.ToTable(t =>
                {
                    t.HasCheckConstraint(
                        "CK_Category_Age",
                        "[MinimumAge] IS NULL OR [MaximumAge] IS NULL OR [MinimumAge] <= [MaximumAge]");

                    t.HasCheckConstraint(
                        "CK_Category_Distance",
                        "[Distance] IS NULL OR [Distance] > 0");
                });
            });

            // Participant, Event, Category -> Enrollment
            modelBuilder.Entity<Enrollment>(entity =>
            {
                entity.HasOne(e => e.Participant)
                    .WithMany()
                    .HasForeignKey(e => e.ParticipantID)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(e => e.Event)
                    .WithMany()
                    .HasForeignKey(e => e.EventID)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(e => e.Category)
                    .WithMany()
                    .HasForeignKey(e => e.CategoryID)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasIndex(e => new
                {
                    e.ParticipantID,
                    e.EventID
                }).IsUnique();

                entity.Property(e => e.EnrollmentDate)
                    .HasDefaultValueSql("GETDATE()");
            });

            // Enrollment -> Result: one-to-zero-or-one
            modelBuilder.Entity<Result>(entity =>
            {
                entity.HasOne(r => r.Enrollment)
                    .WithOne()
                    .HasForeignKey<Result>(r => r.EnrollmentID)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.Property(r => r.FinishTime)
                    .HasColumnType("time");

                entity.ToTable(t => t.HasCheckConstraint(
                    "CK_Result_Position",
                    "[FinishingPosition] > 0"));
            });
        }
    }
}
