namespace ResoEngine.Core2;

/// <summary>
/// Lightweight wrapper that lets any encoded value act as a frame.
/// Perspective and child membership live here so the algebraic value can stay a clean state description.
/// </summary>
public sealed class ValueFrame<TValue>
{
    private readonly List<ValueFrame<TValue>> _children = [];

    public ValueFrame(TValue value, Perspective perspective = Perspective.Dominant)
        : this(value, perspective, null)
    {
    }

    private ValueFrame(TValue value, Perspective perspective, ValueFrame<TValue>? parent)
    {
        Value = value;
        Perspective = perspective;
        Parent = parent;
    }

    public TValue Value { get; }
    public Perspective Perspective { get; private set; }
    public ValueFrame<TValue>? Parent { get; }
    public IReadOnlyList<ValueFrame<TValue>> Children => _children;

    public ValueFrame<TValue> AddChild(TValue childValue, Perspective? perspective = null)
    {
        var child = new ValueFrame<TValue>(childValue, perspective ?? Perspective, this);
        _children.Add(child);
        return child;
    }

    public ValueFrame<TValue> OpposePerspective()
    {
        Perspective = Perspective.Oppose();
        return this;
    }

    public ValueFrame<TValue> SetPerspective(Perspective perspective)
    {
        Perspective = perspective;
        return this;
    }

    public TValue Encode(Func<TValue, TValue> oppositeEncoder) =>
        Perspective == Perspective.Dominant ? Value : oppositeEncoder(Value);
}

public static class ValueFrameExtensions
{
    public static ValueFrame<TValue> AsFrame<TValue>(
        this TValue value,
        Perspective perspective = Perspective.Dominant) =>
        new(value, perspective);
}
