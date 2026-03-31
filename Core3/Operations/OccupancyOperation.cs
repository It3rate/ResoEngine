namespace Core3.Operations;

/// <summary>
/// Symmetric family-wide occupancy predicates over one local partition of a
/// frame. These do not assume a distinguished primary/secondary pair.
/// TODO: This fixed enum is a convenience shell. Longer term, occupancy,
/// retention, and survivor selection should be consolidated into a more
/// Core3-native operation/branch expression rather than remaining a closed
/// code-only category list.
/// </summary>
public enum OccupancyOperation
{
    None,
    Any,
    All,
    NotAll,
    ExactlyOne,
    Odd,
    Even
}

