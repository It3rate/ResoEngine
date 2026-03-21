namespace Core2.Resolution;

/// <summary>
/// Policy vocabulary for choosing a committed result support after
/// exact refinement or exact common-frame alignment has already happened.
/// This is intentionally separate from the primitive support laws.
/// </summary>
public enum ResultSupportPolicy
{
    PreserveCoarser,
    PreserveFiner,
    PreserveHost,
    PreserveExactAlignment,
    NegotiateFromUncertainty,
    PreserveExactStructure,
}
