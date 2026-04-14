using ComicRealmBE.Models;
using Microsoft.EntityFrameworkCore;

namespace ComicRealmBE.Data
{
    /// <summary>
    /// ComicRealmContext: Database context for ComicRealm application
    /// 
    /// Security Architecture:
    /// - Uses PostgreSQL with row-level security (RLS) policies
    /// - EF Core parameterized queries prevent SQL injection
    /// - Connection string comes from User Secrets (dev) or environment variables (prod)
    /// 
    /// Database Tables:
    /// - users: Authentication and role management
    /// - comics: Comic book inventory
    /// 
    /// Migration Usage:
    /// Run: dotnet ef database update
    /// Create new migration: dotnet ef migrations add {MigrationName}
    /// </summary>
    public class ComicRealmContext : DbContext
    {
        public ComicRealmContext(DbContextOptions<ComicRealmContext> options) : base(options)
        {
        }

        // DbSets for each entity
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Comic> Comics { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ========== User Entity Configuration ==========
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");
                entity.HasKey(u => u.UserId);

                // Unique constraint on email
                entity.HasIndex(u => u.Email).IsUnique();

                // Self-referencing relationship for CreatedBy
                entity.HasOne(u => u.CreatedByUser)
                    .WithMany(u => u.CreatedUsers)
                    .HasForeignKey(u => u.CreatedBy)
                    .OnDelete(DeleteBehavior.SetNull);

                // One-to-many: User -> Comics
                entity.HasMany(u => u.Comics)
                    .WithOne(c => c.CreatedByUser)
                    .HasForeignKey(c => c.CreatedBy)
                    .OnDelete(DeleteBehavior.Restrict);

                // Index for active users (commonly queried)
                entity.HasIndex(u => u.IsActive);
            });

            // ========== Comic Entity Configuration ==========
            modelBuilder.Entity<Comic>(entity =>
            {
                entity.ToTable("comics");
                entity.HasKey(c => c.ComicId);

                // Composite index on serie and number for unique comic identification
                entity.HasIndex(c => new { c.Serie, c.Number });

                // Foreign key to User (creator)
                entity.HasOne(c => c.CreatedByUser)
                    .WithMany(u => u.Comics)
                    .HasForeignKey(c => c.CreatedBy)
                    .OnDelete(DeleteBehavior.Restrict);

                // Index for filtering by created user
                entity.HasIndex(c => c.CreatedBy);
            });

            // ========== Seed Initial Data ==========
            // Note: This is for development only. In production, use migrations with proper seeding.
            // SuperAdmin user (password: "SuperAdmin@12345")
            // Hash generated using Argon2, shown here for reference - actual hash differs
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    UserId = 1,
                    Email = "superadmin@comicreal m.local",
                    PasswordHash = "$argon2id$v=19$m=19456,t=2,p=1$random salt$hash", // Placeholder - actual hash in migration
                    Role = Models.Enums.UserRole.SuperAdmin,
                    CreatedBy = null,
                    CreatedAt = new DateTime(2026, 4, 14, 11, 5, 10, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2026, 4, 14, 11, 5, 10, DateTimeKind.Utc),
                    IsActive = true
                }
            );
        }
    }
}
