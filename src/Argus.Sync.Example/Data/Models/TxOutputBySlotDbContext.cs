using Argus.Sync.Data;
using Microsoft.EntityFrameworkCore;

namespace Argus.Sync.Example.Data.Models;

public interface ITxOutputBySlotDbContext
{
    DbSet<TxOutputBySlot> TxOutputBySlot { get; }
}

public class TxOutPutBySlotDbContext(
    DbContextOptions options,
    IConfiguration configuration
) : CardanoDbContext(options, configuration), ITxOutputBySlotDbContext
{
    public DbSet<TxOutputBySlot> TxOutputBySlot => Set<TxOutputBySlot>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TxOutputBySlot>(entity =>
        {
            entity.HasKey(e => new { e.Id, e.Index });
        });
    }
}