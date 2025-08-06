using Microsoft.EntityFrameworkCore;
using Upstart.Persistence.Entitities;

namespace Upstart.Persistence.Data;

public class UpstartDbContext : DbContext
{
    public UpstartDbContext(DbContextOptions<UpstartDbContext> options) : base(options)
    {
    }

    public DbSet<UserEntity> Users { get; set; }
    public DbSet<LoanEntity> Loans { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure UserEntity
        modelBuilder.Entity<UserEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.SocialSecurityNumber).HasMaxLength(11);
            entity.Property(e => e.AddressLine1).HasMaxLength(100);
            entity.Property(e => e.AddressLine2).HasMaxLength(100);
            entity.Property(e => e.City).HasMaxLength(50);
            entity.Property(e => e.State).HasMaxLength(2);
            entity.Property(e => e.ZipCode).HasMaxLength(10);
            entity.Property(e => e.AnnualIncome).HasColumnType("decimal(18,2)");
            entity.Property(e => e.EmploymentStatus).HasMaxLength(50);
        });

        // Configure LoanEntity
        modelBuilder.Entity<LoanEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.LoanAmount).HasColumnType("decimal(18,2)").IsRequired();
            entity.Property(e => e.InterestRate).HasColumnType("decimal(5,2)").IsRequired();
            entity.Property(e => e.MonthlyPayment).HasColumnType("decimal(18,2)").IsRequired();
            entity.Property(e => e.LoanPurpose).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LoanStatus).IsRequired().HasMaxLength(20);
            entity.Property(e => e.OutstandingBalance).HasColumnType("decimal(18,2)");
            entity.Property(e => e.TotalPaymentsMade).HasColumnType("decimal(18,2)");
            entity.Property(e => e.PaymentFrequency).IsRequired().HasMaxLength(20);
            entity.Property(e => e.LateFees).HasColumnType("decimal(18,2)");
            entity.Property(e => e.OriginationFee).HasColumnType("decimal(18,2)");
            entity.Property(e => e.APR).HasColumnType("decimal(5,2)");
            entity.Property(e => e.LoanOfficerNotes).HasMaxLength(1000);

            // Foreign key relationship
            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}