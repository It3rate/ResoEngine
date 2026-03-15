using Core2.Elements;
using Core2.Units;

namespace Core2.Resolution;

public sealed class LayeredQuantity
{
    private readonly IReadOnlyList<ResolutionComponent> _components;

    public LayeredQuantity(
        IEnumerable<ResolutionComponent> components,
        UnitSignature signature,
        UnitChoice? preferredUnit = null)
    {
        ArgumentNullException.ThrowIfNull(components);
        ArgumentNullException.ThrowIfNull(signature);

        var materialized = components.ToArray();
        foreach (var component in materialized)
        {
            if (!component.Frame.Signature.Equals(signature))
            {
                throw new ArgumentException(
                    $"Resolution component frame {component.Frame.Symbol} does not match signature {signature}.",
                    nameof(components));
            }
        }

        _components = materialized;
        Signature = signature;
        PreferredUnit = preferredUnit;
    }

    public IReadOnlyList<ResolutionComponent> Components => _components;
    public UnitSignature Signature { get; }
    public UnitChoice? PreferredUnit { get; }

    public Quantity<Scalar> Fold()
    {
        Scalar sum = Scalar.Zero;
        foreach (var component in _components)
        {
            sum += component.CanonicalAmount;
        }

        return new Quantity<Scalar>(sum, Signature, PreferredUnit);
    }

    public LayeredQuantity AddComponent(ResolutionComponent component)
    {
        if (!component.Frame.Signature.Equals(Signature))
        {
            throw new ArgumentException(
                $"Resolution component frame {component.Frame.Symbol} does not match signature {Signature}.",
                nameof(component));
        }

        return new LayeredQuantity([.. _components, component], Signature, PreferredUnit);
    }

    public LayeredQuantity Normalize(ResolutionLadder ladder, bool preserveZeroDigits = true) =>
        ladder.Decompose(Fold(), preserveZeroDigits);

    public ResolutionReading ReadAt(
        ResolutionFrame frame,
        ResolutionQuantizationRule rule = ResolutionQuantizationRule.Nearest)
    {
        if (!frame.CanRead(Signature))
        {
            throw new ArgumentException(
                $"Resolution frame {frame.Symbol} does not match signature {Signature}.",
                nameof(frame));
        }

        var exact = Fold();
        Scalar rawTickCount = exact.Value / frame.Grain;
        Scalar tickCount = ResolutionQuantizer.Quantize(rawTickCount, rule);
        Scalar representativeValue = tickCount * frame.Grain;
        Scalar residualValue = exact.Value - representativeValue;

        return new ResolutionReading(
            frame,
            rawTickCount,
            tickCount,
            rule,
            exact,
            new Quantity<Scalar>(representativeValue, Signature, PreferredUnit),
            new Quantity<Scalar>(residualValue, Signature, PreferredUnit));
    }

    public LayeredQuantity CollapseTo(
        ResolutionFrame frame,
        ResolutionQuantizationRule rule = ResolutionQuantizationRule.Nearest) =>
        ReadAt(frame, rule).Collapse();
}
