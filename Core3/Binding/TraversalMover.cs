using Core3.Engine;

namespace Core3.Binding;

/// <summary>
/// A small exact mover/cursor/trolley represented by one atomic iterator
/// value. The numerator is the current route position and the denominator is
/// the route end/resolution. The current pass uses a default +1 continuation
/// with clamp-like stop behavior at the denominator.
/// </summary>
public sealed record TraversalMover
{
    public TraversalMover(string name, AtomicElement position)
    {
        Name = !string.IsNullOrWhiteSpace(name)
            ? name
            : throw new ArgumentException("A traversal mover name cannot be empty.", nameof(name));

        ArgumentNullException.ThrowIfNull(position);

        if (!position.HasResolvedUnits || position.Unit <= 0)
        {
            throw new InvalidOperationException("Traversal movers currently require one positive resolved atomic iterator.");
        }

        if (position.Value < 0 || position.Value > position.Unit)
        {
            throw new InvalidOperationException("Traversal mover position must stay within 0 and the iterator denominator.");
        }

        Position = position;
    }

    public string Name { get; }
    public AtomicElement Position { get; }

    public long CurrentTick => Position.Value;
    public long EndTick => Position.Unit;
    public bool IsAtStop => CurrentTick == EndTick;

    public bool TryAdvance(out TraversalMover? advanced)
    {
        if (IsAtStop)
        {
            advanced = null;
            return false;
        }

        var nextValue = Math.Min(checked(CurrentTick + 1), EndTick);
        advanced = new TraversalMover(Name, new AtomicElement(nextValue, Position.Unit));
        return true;
    }
}
