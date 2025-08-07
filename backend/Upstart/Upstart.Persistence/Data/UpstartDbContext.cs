using Microsoft.EntityFrameworkCore;
using Upstart.Persistence.Entitities;

namespace Upstart.Persistence.Data;

public class UpstartDbContext : DbContext
{
    public UpstartDbContext(DbContextOptions<UpstartDbContext> options) : base(options)
    {
    }

    public DbSet<UserEntity> Users { get; set; }
    public DbSet<PollEntity> Polls { get; set; }
    public DbSet<PollAnswerEntity> PollAnswers { get; set; }
    public DbSet<PollStatEntity> PollStats { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure UserEntity
        modelBuilder.Entity<UserEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(255);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
        });

        // Configure PollEntity
        modelBuilder.Entity<PollEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.PollGuid).IsRequired().HasMaxLength(36);
            entity.HasIndex(e => e.PollGuid).IsUnique();
            entity.Property(e => e.Question).IsRequired().HasMaxLength(500);
            
            // Foreign key relationship
            entity.HasOne(e => e.User)
                .WithMany(u => u.Polls)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure PollAnswerEntity
        modelBuilder.Entity<PollAnswerEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.AnswerText).IsRequired().HasMaxLength(500);
            
            // Foreign key relationship
            entity.HasOne(e => e.Poll)
                .WithMany(p => p.Answers)
                .HasForeignKey(e => e.PollId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure PollStatEntity
        modelBuilder.Entity<PollStatEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            // Foreign key relationships
            entity.HasOne(e => e.Poll)
                .WithMany(p => p.Stats)
                .HasForeignKey(e => e.PollId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(e => e.PollAnswer)
                .WithMany(pa => pa.Stats)
                .HasForeignKey(e => e.PollAnswerId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(e => e.User)
                .WithMany(u => u.PollStats)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Unique constraint to prevent duplicate votes
            entity.HasIndex(e => new { e.PollId, e.UserId })
                .IsUnique();
        });
    }
}