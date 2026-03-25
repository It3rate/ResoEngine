namespace Core3.Elements;

/// <summary>
/// Which side of a pin or element a carrier is being read from.
/// Inbound mirrors the raw reading; outbound reads it directly.
/// </summary>
public enum CarrierSide
{
    Inbound,
    Outbound,
}
