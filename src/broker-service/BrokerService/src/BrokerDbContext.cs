// File: EasyTrade/BrokerService/BrokerDbContext.cs
using EasyTrade.BrokerService.Entities.Balances;
using EasyTrade.BrokerService.Entities.Instruments;
using EasyTrade.BrokerService.Entities.Packages;
using EasyTrade.BrokerService.Entities.Products;
using EasyTrade.BrokerService.Entities.Trades;
using Microsoft.EntityFrameworkCore;

namespace EasyTrade.BrokerService
{
    public class BrokerDbContext : DbContext
    {
        public BrokerDbContext(DbContextOptions<BrokerDbContext> options) : base(options) { }

        public DbSet<Balance> Balances { get; set; }
        public DbSet<BalanceHistory> BalanceHistories { get; set; }
        public DbSet<Instrument> Instruments { get; set; }
        public DbSet<OwnedInstrument> OwnedInstruments { get; set; }
        public DbSet<Package> Packages { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Trade> Trades { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Trade>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id)
                    .HasColumnType("int")
                    .UseMySqlIdentityColumn() 
                    .ValueGeneratedOnAdd();
            });

            // Global application of DateTime converter to all models (if needed)
            var decimalProperties = modelBuilder
                .Model.GetEntityTypes()
                .SelectMany(type => type.GetProperties())
                .Where(property => property.ClrType == typeof(decimal) || property.ClrType == typeof(decimal?));

            foreach (var property in decimalProperties)
            {
                property.SetPrecision(18);
                property.SetScale(8);
            }
        }
    }
}
