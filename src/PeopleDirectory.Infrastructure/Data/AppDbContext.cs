using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PeopleDirectory.Domain.Entities;

namespace PeopleDirectory.Infrastructure.Data;

public class AppDbContext : IdentityDbContext<IdentityUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Person> People => Set<Person>();
    public DbSet<Country> Countries => Set<Country>();
    public DbSet<City> Cities => Set<City>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Person>(e =>
        {
            e.HasIndex(p => p.FirstName);
            e.HasIndex(p => p.LastName);
            e.HasIndex(p => p.Email).IsUnique();
            e.Property(p => p.FirstName).HasMaxLength(100);
            e.Property(p => p.LastName).HasMaxLength(100);
            e.Property(p => p.Email).HasMaxLength(255);
            e.Property(p => p.MobileNumber).HasMaxLength(20);
            e.Property(p => p.Gender).HasMaxLength(10);
            e.Property(p => p.ProfilePictureUrl).HasMaxLength(500);
            e.Property(p => p.AddressLine).HasMaxLength(300);
            e.HasOne(p => p.City).WithMany(c => c.People).HasForeignKey(p => p.CityId);
            e.HasQueryFilter(p => p.IsActive);
        });

        builder.Entity<Country>(e =>
        {
            e.Property(c => c.Name).HasMaxLength(100);
            e.Property(c => c.Code).HasMaxLength(5);
        });

        builder.Entity<City>(e =>
        {
            e.Property(c => c.Name).HasMaxLength(100);
            e.HasOne(c => c.Country).WithMany(co => co.Cities).HasForeignKey(c => c.CountryId);
        });

        builder.Entity<AuditLog>(e =>
        {
            e.Property(a => a.Action).HasMaxLength(20);
            e.Property(a => a.PerformedBy).HasMaxLength(100);
            e.HasOne(a => a.Person).WithMany().HasForeignKey(a => a.PersonId);
        });

        builder.Entity<OutboxMessage>(e =>
        {
            e.HasKey(m => m.Id);
            e.Property(m => m.Type).HasMaxLength(100).IsRequired();
            e.Property(m => m.Payload).IsRequired();
            e.Property(m => m.LastError).HasMaxLength(2000);
            // Speeds up the background processor's "pending messages" query.
            e.HasIndex(m => new { m.ProcessedAt, m.CreatedAt });
        });

        SeedData.Seed(builder);
    }
}
