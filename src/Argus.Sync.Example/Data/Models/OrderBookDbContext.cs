using Argus.Sync.Data;
using Microsoft.EntityFrameworkCore;

namespace Argus.Sync.Example.Data.Models;

public interface IOrderBookDbContext
{
    // DbSet<TxOutputBySlot> TxOutputBySlot { get; }
    DbSet<OrderBySlot> OrderBySlots { get; }
}

public class OrderBookDbContext(
    DbContextOptions options,
    IConfiguration configuration
) : CardanoDbContext(options, configuration), IOrderBookDbContext
{
    public DbSet<OrderBySlot> OrderBySlots => Set<OrderBySlot>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<OrderBySlot>(entity =>
        {
            entity.HasKey(e => new { e.Id, e.Index });
        });
    }
}