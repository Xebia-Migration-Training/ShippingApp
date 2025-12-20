using Microsoft.EntityFrameworkCore;
using ShippingRules.Domain.Entities;

namespace ShippingRules.Infrastructure.Data;

public class ShippingRulesDbContext : DbContext
{
    public ShippingRulesDbContext(DbContextOptions<ShippingRulesDbContext> options) : base(options)
    {
    }

    public DbSet<Country> Countries => Set<Country>();
    public DbSet<Port> Ports => Set<Port>();
    public DbSet<Vessel> Vessels => Set<Vessel>();
    public DbSet<Principal> Principals => Set<Principal>();
    public DbSet<ShippingRule> ShippingRules => Set<ShippingRule>();
    public DbSet<ExchangeRate> ExchangeRates => Set<ExchangeRate>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Country configuration
        modelBuilder.Entity<Country>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Code).IsRequired().HasMaxLength(10);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Region).HasMaxLength(100);
            entity.HasIndex(e => e.Code).IsUnique();
        });

        // Port configuration
        modelBuilder.Entity<Port>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Code).IsRequired().HasMaxLength(10);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Type).HasMaxLength(50);
            entity.HasIndex(e => e.Code).IsUnique();
            
            entity.HasOne(e => e.Country)
                .WithMany(c => c.Ports)
                .HasForeignKey(e => e.CountryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Principal configuration
        modelBuilder.Entity<Principal>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Code).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.ContactEmail).HasMaxLength(100);
            entity.Property(e => e.ContactPhone).HasMaxLength(20);
            entity.HasIndex(e => e.Code).IsUnique();
        });

        // Vessel configuration
        modelBuilder.Entity<Vessel>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.IMONumber).IsRequired().HasMaxLength(10);
            entity.Property(e => e.VesselType).HasMaxLength(50);
            entity.HasIndex(e => e.IMONumber).IsUnique();
            
            entity.HasOne(e => e.Principal)
                .WithMany(p => p.Vessels)
                .HasForeignKey(e => e.PrincipalId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ShippingRule configuration
        modelBuilder.Entity<ShippingRule>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.RuleName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.PrecedenceLevel).IsRequired();
            entity.Property(e => e.BaseRate).HasColumnType("decimal(18,2)");
            entity.Property(e => e.SurchargePercentage).HasColumnType("decimal(5,2)");
            entity.Property(e => e.RuleType).IsRequired().HasMaxLength(50);
            entity.Property(e => e.RuleCategory).HasMaxLength(100);
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.ApprovedBy).HasMaxLength(100);

            entity.HasIndex(e => new { e.PrecedenceLevel, e.EffectiveFrom, e.IsActive });
            
            entity.HasOne(e => e.Country)
                .WithMany(c => c.ShippingRules)
                .HasForeignKey(e => e.CountryId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Port)
                .WithMany(p => p.ShippingRules)
                .HasForeignKey(e => e.PortId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Vessel)
                .WithMany(v => v.ShippingRules)
                .HasForeignKey(e => e.VesselId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Principal)
                .WithMany(p => p.ShippingRules)
                .HasForeignKey(e => e.PrincipalId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ExchangeRate configuration
        modelBuilder.Entity<ExchangeRate>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FromCurrency).IsRequired().HasMaxLength(3);
            entity.Property(e => e.ToCurrency).IsRequired().HasMaxLength(3);
            entity.Property(e => e.Rate).HasColumnType("decimal(18,6)");
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.HasIndex(e => new { e.FromCurrency, e.ToCurrency, e.EffectiveFrom, e.IsActive });
        });

        // Seed data
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // Seed Countries
        var usa = new Country { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), Code = "US", Name = "United States", Region = "North America", IsActive = true, CreatedAt = DateTime.UtcNow, CreatedBy = "System" };
        var china = new Country { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), Code = "CN", Name = "China", Region = "Asia", IsActive = true, CreatedAt = DateTime.UtcNow, CreatedBy = "System" };
        var india = new Country { Id = Guid.Parse("33333333-3333-3333-3333-333333333333"), Code = "IN", Name = "India", Region = "Asia", IsActive = true, CreatedAt = DateTime.UtcNow, CreatedBy = "System" };

        modelBuilder.Entity<Country>().HasData(usa, china, india);

        // Seed Ports
        var losAngeles = new Port { Id = Guid.Parse("44444444-4444-4444-4444-444444444444"), Code = "USLAX", Name = "Port of Los Angeles", Type = "Seaport", CountryId = usa.Id, IsActive = true, CreatedAt = DateTime.UtcNow, CreatedBy = "System" };
        var shanghai = new Port { Id = Guid.Parse("55555555-5555-5555-5555-555555555555"), Code = "CNSHA", Name = "Port of Shanghai", Type = "Seaport", CountryId = china.Id, IsActive = true, CreatedAt = DateTime.UtcNow, CreatedBy = "System" };
        var mumbai = new Port { Id = Guid.Parse("66666666-6666-6666-6666-666666666666"), Code = "INMUM", Name = "Port of Mumbai", Type = "Seaport", CountryId = india.Id, IsActive = true, CreatedAt = DateTime.UtcNow, CreatedBy = "System" };

        modelBuilder.Entity<Port>().HasData(losAngeles, shanghai, mumbai);

        // Seed Principals
        var maersk = new Principal { Id = Guid.Parse("77777777-7777-7777-7777-777777777777"), Code = "MAER", Name = "Maersk Line", ContactEmail = "contact@maersk.com", ContactPhone = "+1-555-0100", IsActive = true, CreatedAt = DateTime.UtcNow, CreatedBy = "System" };
        var msc = new Principal { Id = Guid.Parse("88888888-8888-8888-8888-888888888888"), Code = "MSC", Name = "MSC Mediterranean Shipping", ContactEmail = "info@msc.com", ContactPhone = "+1-555-0200", IsActive = true, CreatedAt = DateTime.UtcNow, CreatedBy = "System" };

        modelBuilder.Entity<Principal>().HasData(maersk, msc);

        // Seed Vessels
        var vessel1 = new Vessel { Id = Guid.Parse("99999999-9999-9999-9999-999999999999"), Name = "Maersk Explorer", IMONumber = "IMO1234567", VesselType = "Container Ship", PrincipalId = maersk.Id, IsActive = true, CreatedAt = DateTime.UtcNow, CreatedBy = "System" };
        var vessel2 = new Vessel { Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), Name = "MSC Oscar", IMONumber = "IMO7654321", VesselType = "Container Ship", PrincipalId = msc.Id, IsActive = true, CreatedAt = DateTime.UtcNow, CreatedBy = "System" };

        modelBuilder.Entity<Vessel>().HasData(vessel1, vessel2);

        // Seed Exchange Rates (minimal demo)
        var now = DateTime.UtcNow;
        var usdToInr = new ExchangeRate
        {
            Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
            FromCurrency = "USD",
            ToCurrency = "INR",
            Rate = 83.000000m,
            EffectiveFrom = now.AddYears(-5),
            EffectiveTo = null,
            IsActive = true,
            CreatedAt = now,
            CreatedBy = "System"
        };
        var inrToUsd = new ExchangeRate
        {
            Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
            FromCurrency = "INR",
            ToCurrency = "USD",
            Rate = 0.012048m,
            EffectiveFrom = now.AddYears(-5),
            EffectiveTo = null,
            IsActive = true,
            CreatedAt = now,
            CreatedBy = "System"
        };

        modelBuilder.Entity<ExchangeRate>().HasData(usdToInr, inrToUsd);
    }
}
