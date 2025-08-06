using Microsoft.EntityFrameworkCore;
using Upstart.Persistence.Entitities;

namespace Upstart.IntegrationTests.TestSetup;

public class TestDbContext : DbContext
{
    public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
    {
    }

    public DbSet<UserEntity> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}