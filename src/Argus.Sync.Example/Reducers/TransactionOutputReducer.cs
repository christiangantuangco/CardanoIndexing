using Argus.Sync.Reducers;
using Argus.Sync.Example.Data.Models;
using Microsoft.EntityFrameworkCore;
using Chrysalis.Cardano.Core.Extensions;
using Chrysalis.Cardano.Core.Types.Block.Transaction.Body;
using System.Linq.Expressions;
using Argus.Sync.Extensions;
using Block = Chrysalis.Cardano.Core.Types.Block.Block;

namespace Argus.Sync.Example.Reducers;

public class TransactionOutputReducer(
    IDbContextFactory<TxOutPutBySlotDbContext> dbContextFactory
) : IReducer<TxOutputBySlot>
{
    public async Task RollBackwardAsync(ulong slot)
    {
        using TxOutPutBySlotDbContext dbContext = await dbContextFactory.CreateDbContextAsync();

        await dbContext
            .TxOutputBySlot
            .Where(e => e.Slot >= slot)
            .ExecuteDeleteAsync();
    }

    public async Task RollForwardAsync(Block block)
    {
        if (!block.TransactionBodies().Any()) return;

        await using TxOutPutBySlotDbContext dbContext = await dbContextFactory.CreateDbContextAsync();

        IEnumerable<TransactionBody> txBodies = block.TransactionBodies();
        await ProcessInputs(txBodies, block.Slot() ?? 0, dbContext);
        await ProcessOutputs(txBodies, block.Slot() ?? 0, dbContext);

        await dbContext.SaveChangesAsync();
        await dbContext.DisposeAsync();
    }

    public async Task ProcessInputs(IEnumerable<TransactionBody> txBodies, ulong slot, TxOutPutBySlotDbContext dbContext)
    {
        List<(string Id, ulong Index)> existingOutputs = [.. txBodies
            .SelectMany(tx => tx.Inputs()
                .Select(input => (input.TransactionId(), input.Index())))];

        Expression<Func<TxOutputBySlot, bool>> predicate = PredicateBuilder.False<TxOutputBySlot>();

        existingOutputs.ForEach(o => predicate = predicate.Or(p => p.Id == o.Id && p.Index == o.Index));

        List<TxOutputBySlot> existingTxOutputBySlots = [.. dbContext.TxOutputBySlot.Where(predicate)];

        if (existingTxOutputBySlots.Any())
        {
            existingTxOutputBySlots.ForEach(txOutputBySlot =>
            {
                txOutputBySlot.SpentSlot = slot;
            });

            dbContext.TxOutputBySlot.UpdateRange(existingTxOutputBySlots);
        }
    }

    public async Task ProcessOutputs(IEnumerable<TransactionBody> txBodies, ulong slot, TxOutPutBySlotDbContext dbContext)
    {
        IEnumerable<TxOutputBySlot> txOutputBySlots = txBodies.SelectMany(txBody => 
            txBody
                .Outputs()
                .Select((output, index) =>
                {
                    TxOutputBySlot txOutputBySlot = new(
                        txBody.Id(),
                        (ulong)index,
                        slot,
                        null,
                        Convert.ToHexString(output.Address()?.Raw ?? []),
                        output?.Raw ?? []
                    );
                    return txOutputBySlot;
                })
        );

        dbContext.TxOutputBySlot
            .AddRange(txOutputBySlots);
    }
}
