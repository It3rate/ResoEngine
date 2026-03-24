namespace Core3.Elements;

/// <summary>
/// A valve-like pin located on the ambient line.
/// It does not generate carriers. Instead it reads one attached raw extent into
/// an inbound port value and an outbound port value relative to the pin origin.
/// </summary>
public readonly record struct Pin(long Position, RawExtent Attachment)
{
    /// <summary>
    /// The carrier value of the inbound port. This is the start point read relative
    /// to the pin origin, so left-of-pin starts read positive and right-of-pin starts
    /// read negative only after the sign is folded through the inbound role.
    /// </summary>
    public long InboundCarrier => Position - Attachment.Start;

    /// <summary>
    /// The carrier value of the outbound port. This is the end point read directly
    /// from the pin origin toward the stored end point.
    /// </summary>
    public long OutboundCarrier => Attachment.End - Position;

    public bool HasInbound => InboundCarrier != 0;
    public bool HasOutbound => OutboundCarrier != 0;
    public bool InboundIsPositive => InboundCarrier > 0;
    public bool OutboundIsPositive => OutboundCarrier > 0;
    public bool InboundIsNegative => InboundCarrier < 0;
    public bool OutboundIsNegative => OutboundCarrier < 0;

    public override string ToString() => $"@{Position} : in {InboundCarrier}, out {OutboundCarrier}";
}
