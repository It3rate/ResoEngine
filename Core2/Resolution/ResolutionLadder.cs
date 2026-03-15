using Core2.Elements;
using Core2.Units;

namespace Core2.Resolution;

public sealed class ResolutionLadder
{
    private readonly IReadOnlyList<ResolutionFrame> _frames;

    public ResolutionLadder(IEnumerable<ResolutionFrame> frames)
    {
        ArgumentNullException.ThrowIfNull(frames);

        var ordered = frames.ToArray();
        if (ordered.Length == 0)
        {
            throw new ArgumentException("A resolution ladder requires at least one frame.", nameof(frames));
        }

        Signature = ordered[0].Signature;
        for (int index = 0; index < ordered.Length; index++)
        {
            var frame = ordered[index];
            if (!frame.Signature.Equals(Signature))
            {
                throw new ArgumentException("All ladder frames must share the same unit signature.", nameof(frames));
            }

            if (index > 0 && ordered[index - 1].Grain.Value <= frame.Grain.Value)
            {
                throw new ArgumentException("Resolution ladder frames must descend from coarser grain to finer grain.", nameof(frames));
            }
        }

        _frames = ordered;
    }

    public UnitSignature Signature { get; }
    public IReadOnlyList<ResolutionFrame> Frames => _frames;

    public LayeredQuantity Decompose(Quantity<Scalar> quantity, bool preserveZeroDigits = true)
    {
        if (!quantity.Signature.Equals(Signature))
        {
            throw new ArgumentException(
                $"Quantity signature {quantity.Signature} does not match ladder signature {Signature}.",
                nameof(quantity));
        }

        decimal remaining = quantity.Value.Value;
        var components = new List<ResolutionComponent>(_frames.Count);

        for (int index = 0; index < _frames.Count; index++)
        {
            var frame = _frames[index];
            Scalar count = index == _frames.Count - 1
                ? new Scalar(remaining / frame.Grain.Value)
                : new Scalar(decimal.Truncate(remaining / frame.Grain.Value));

            remaining -= count.Value * frame.Grain.Value;
            if (preserveZeroDigits || !count.IsZero)
            {
                components.Add(new ResolutionComponent(frame, count));
            }
        }

        return new LayeredQuantity(components, Signature, quantity.PreferredUnit);
    }
}
