namespace Core3.Operations;

/// <summary>
/// Symmetric family-wide occupancy predicates over one local partition of a
/// frame. These do not assume a distinguished primary/secondary pair.
/// </summary>
public enum EngineOccupancyOperation
{
    None,
    Any,
    All,
    NotAll,
    ExactlyOne,
    Odd,
    Even
}
