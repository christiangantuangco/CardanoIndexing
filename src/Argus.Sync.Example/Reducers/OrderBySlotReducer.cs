using System.Linq.Expressions;
using Argus.Sync.Example.Data.Enums;
using Argus.Sync.Example.Data.Extensions;
using Argus.Sync.Example.Data.Models;
using Argus.Sync.Example.Data.Models.Datums;
using Argus.Sync.Example.Data.Models.Redeemers;
using Argus.Sync.Extensions;
using Argus.Sync.Reducers;
using Argus.Sync.Utils;
using Chrysalis.Cardano.Core.Extensions;
using Chrysalis.Cardano.Core.Types.Block;
using Chrysalis.Cardano.Core.Types.Block.Transaction.Body;
using Chrysalis.Cardano.Core.Types.Block.Transaction.Input;
using Chrysalis.Cardano.Sundae.Types.Common;
using Chrysalis.Cbor.Converters;
using Microsoft.EntityFrameworkCore;

namespace Argus.Sync.Example.Reducers;

public class OrderBySlotReducer(
    IDbContextFactory<OrderBookDbContext> dbContextFactory,
    IConfiguration configuration
) : IReducer<OrderBySlot>
{
    private readonly string _orderBookScriptHash = configuration.GetValue("OrderBook", "0f45963b8e895bd46839bbcf34185993440f26e3f07c668bd2026f92");

    public async Task RollBackwardAsync(ulong slot)
    {
        await using OrderBookDbContext dbContext = await dbContextFactory.CreateDbContextAsync();

        await dbContext
            .OrderBySlots
            .Where(o => o.Slot >= slot)
            .ExecuteDeleteAsync();

        await dbContext.DisposeAsync();
    }

    public async Task RollForwardAsync(Block block)
    {
        if (!block.TransactionBodies().Any()) await Task.CompletedTask;

        await using OrderBookDbContext dbContext = await dbContextFactory.CreateDbContextAsync();
        IEnumerable<TransactionBody> transactions = block.TransactionBodies();

        transactions.ToList().ForEach(tx => ProcessOutputs(
            tx,
            block,
            dbContext
        ));

        IEnumerable<(string Id, ulong Index)> outRefs = transactions.SelectMany(
            tx => tx.Inputs()
                .Select(i => (i.TransactionId(), i.Index.Value))
        );

        Expression<Func<OrderBySlot, bool>> predicate = PredicateBuilder.False<OrderBySlot>();
        outRefs.ToList()
            .ForEach(outRef => predicate = predicate.Or(p => p.Id == outRef.Id && p.Index == outRef.Index));

        List<OrderBySlot> dbEntries = await dbContext.OrderBySlots
            .Where(predicate)
            .ToListAsync();

        List<OrderBySlot> localEntries = [.. dbContext.OrderBySlots.Local
            .Where(e => outRefs.Any(outRef => outRef.Id == e.Id && outRef.Index == e.Index))
        ];

        List<OrderBySlot> combinedEntries = [.. dbEntries, .. localEntries];

        ProcessInputs(transactions,block, combinedEntries, dbContext);

        await dbContext.SaveChangesAsync();
    }

    private void ProcessInputs(
        IEnumerable<TransactionBody> transactions,
        Block block,
        List<OrderBySlot> entries,
        OrderBookDbContext dbContext
    )
    {
        if (!entries.Any()) return;

        ulong slot = block.Slot() ?? 0;

        IEnumerable<(byte[]? RedeemerRaw, TransactionInput Input, TransactionBody Tx)> inputRedeemers = transactions.ToList().GetInputRedeemerTuple(block);

        entries.ForEach(entry =>
        {
            (byte[]? RedeemerRaw, TransactionInput Input, TransactionBody Tx) = inputRedeemers
                .FirstOrDefault(ir => ir.Input.TransactionId() == entry.Id && ir.Input.Index.Value == entry.Index);

            if (RedeemerRaw is null) return;

            bool isAcceptRedeemer = false;
            try
            {
                AcceptRedeemer redeemer = CborSerializer.Deserialize<AcceptRedeemer>(RedeemerRaw);
                isAcceptRedeemer = true;
            }
            catch {}

            OrderBySlot? localEntry = dbContext.OrderBySlots.Local
                .FirstOrDefault(e => e.Id == entry.Id && e.Index == entry.Index);

            OrderBySlot updatedEntry = entry with
            {
                OrderStatus = isAcceptRedeemer ? OrderStatus.Traded : OrderStatus.Cancelled,
                SpentSlot = isAcceptRedeemer ? slot : null
            };

            if (localEntry is not null)
                dbContext.Entry(localEntry).CurrentValues.SetValues(updatedEntry);
            else
                dbContext.Attach(updatedEntry).State = EntityState.Modified;
        });
    }

    private void ProcessOutputs(
        TransactionBody txBody,
        Block block,
        OrderBookDbContext dbContext
    )
    {
        string id = txBody.Id();
        ulong? slot = block.Slot();

        txBody.Outputs()
            .Select((output, index) => new { Output = output, Index = index })
            .ToList()
            .ForEach(e =>
            {
                string outputPkh = Convert.ToHexString(e.Output.Address()?.GetPublicKeyHash() ?? []).ToLowerInvariant();
                if (string.IsNullOrEmpty(outputPkh) | outputPkh != _orderBookScriptHash) return;

                OrderDatum orderDatum = CborSerializer.Deserialize<OrderDatum>(e.Output.Datum()!);

                string ownerPkh = Convert.ToHexString(orderDatum.Owner.Value).ToLowerInvariant();

                AssetClass asset = orderDatum.Asset;
                string policyId = Convert.ToHexStringLower(asset.Value()[0].Value);
                string assetName = Convert.ToHexStringLower(asset.Value()[1].Value);
                ulong quantity = orderDatum.Quantity.Value;

                OrderBySlot orderBySlot = new(
                    id,
                    (ulong)e.Index,
                    slot ?? 0,
                    ownerPkh,
                    e.Output.Raw ?? [],
                    orderDatum.Raw ?? [],
                    policyId,
                    assetName,
                    quantity,
                    OrderStatus.Listed
                );

                dbContext.OrderBySlots.Add(orderBySlot);
            });
    }
}