using Core2.Elements;

namespace Core2.Resolution;

public readonly record struct ResolutionComponent(ResolutionFrame Frame, Scalar Count)
{
    public Scalar CanonicalAmount => Count * Frame.Grain;

    public override string ToString() => $"{Count} x {Frame.Symbol}";
}
