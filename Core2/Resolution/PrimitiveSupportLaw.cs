namespace Core2.Resolution;

/// <summary>
/// Primitive support-law vocabulary for Pass 1 resolution work.
/// These names classify how support/resolution is expected to behave
/// before the runtime paths are fully reworked to obey them.
/// </summary>
public enum PrimitiveSupportLaw
{
    Inherit,
    Aggregate,
    Compose,
    Refine,
    CommonFrame,
}
