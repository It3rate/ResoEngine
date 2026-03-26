namespace Core3.Engine;

/// <summary>
/// Runtime-graded engine element.
/// Grade 0 is atomic; higher grades are composed recursively from two
/// lower-grade children.
/// </summary>
public abstract record GradedElement
{
    public abstract int Grade { get; }
    public abstract bool HasResolvedUnits { get; }
    public abstract GradedElement Negate();
    public abstract GradedElement SwapOrder();
    public abstract GradedElement FlipPerspective();
    public abstract bool SharesUnitSpace(GradedElement other);
    public abstract bool TryAdd(GradedElement other, out GradedElement? sum);
    public abstract bool TrySubtract(GradedElement other, out GradedElement? difference);

    public bool CanAdd(GradedElement other) => TryAdd(other, out _);
    public bool CanSubtract(GradedElement other) => TrySubtract(other, out _);

    protected static void RequireSameGrade(GradedElement left, GradedElement right)
    {
        if (left.Grade != right.Grade)
        {
            throw new InvalidOperationException("Elements must have the same grade.");
        }
    }
}
