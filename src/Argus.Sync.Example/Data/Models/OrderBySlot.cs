using Argus.Sync.Data.Models;
using Argus.Sync.Example.Data.Enums;

namespace Argus.Sync.Example.Data.Models;

public record OrderBySlot : IReducerModel
{
    public string Id { get; init; }
    public ulong Index { get; init; }
    public ulong Slot { get; init; }
    public ulong? SpentSlot { get; set; }
    public string OwnerPkh { get; init; }
    public byte[] RawCbor { get; init; }
    public byte[] RawDatum { get; init; }
    public string PolicyId { get; set; }
    public string AssetName { get; set; }
    public ulong Quantity { get; set; }
    public OrderStatus OrderStatus { get; set; }

    public OrderBySlot(
        string id,
        ulong index,
        ulong slot,
        string ownerPkh,
        byte[] rawCbor,
        byte[] rawDatum,
        string policyId,
        string assetName,
        ulong quantity,
        OrderStatus orderStatus
    )
    {
        Id = id;
        Index = index;
        Slot = slot;
        OwnerPkh = ownerPkh;
        RawCbor = rawCbor;
        RawDatum = rawDatum;
        PolicyId = policyId;
        AssetName = assetName;
        Quantity = quantity;
        OrderStatus = orderStatus;
    }
}