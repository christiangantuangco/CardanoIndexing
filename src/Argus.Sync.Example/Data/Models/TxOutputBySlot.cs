using Argus.Sync.Data.Models;
using Chrysalis.Cardano.Core.Extensions;
using Chrysalis.Cardano.Core.Types.Block.Transaction.Output;
using Chrysalis.Cbor.Converters;

namespace Argus.Sync.Example.Data.Models;

public record TxOutputBySlot : IReducerModel
{
    public string Id { get; init; }
    public ulong Index { get; init; }
    public ulong Slot { get; init; }
    public ulong? SpentSlot { get; set; }
    public string Address { get; init; }
    public byte[] RawCbor { get; init; }
    
    public TxOutputBySlot(string id, ulong index, ulong slot, ulong? spentSlot, string address, byte[] rawCbor)
    {
        Id = id;
        Index = index;
        Slot = slot;
        SpentSlot = spentSlot;
        Address = address;
        RawCbor = rawCbor;
    }

    public Value Amount
    {
        get => CborSerializer.Deserialize<TransactionOutput>(RawCbor)?.Amount()
            ?? throw new InvalidOperationException("Failed to deserialize Value from RawCbor");
    }
};