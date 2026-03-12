namespace ResoEngine.Core2;

/// <summary>
/// A concrete frame that gives an ordered interval measurable meaning.
/// Start is measured by the left unit, end by the right unit.
/// Each measurement is a Proportion, so reading a line yields an Axis.
/// </summary>
public readonly record struct MeasurementFrame
{
    private MeasurementFrame(
        Scalar origin,
        Scalar leftUnit,
        Scalar rightUnit,
        Perspective perspective,
        bool _)
    {
        if (leftUnit.IsZero)
            throw new ArgumentOutOfRangeException(nameof(leftUnit), "Left unit must be non-zero.");
        if (rightUnit.IsZero)
            throw new ArgumentOutOfRangeException(nameof(rightUnit), "Right unit must be non-zero.");

        Origin = origin;
        LeftUnit = leftUnit;
        RightUnit = rightUnit;
        Perspective = perspective;
    }

    public MeasurementFrame(long origin, long unit, Perspective perspective = Perspective.Dominant)
        : this(new Scalar(origin), new Scalar(unit), new Scalar(unit), perspective, true)
    {
    }

    public MeasurementFrame(
        long origin,
        long leftUnit,
        long rightUnit,
        Perspective perspective = Perspective.Dominant)
        : this(new Scalar(origin), new Scalar(leftUnit), new Scalar(rightUnit), perspective, true)
    {
    }

    public Scalar Origin { get; }
    public Scalar LeftUnit { get; }
    public Scalar RightUnit { get; }
    public Perspective Perspective { get; }

    public Proportion MeasureStart(long start) => Proportion.FromScalars(Origin - (Scalar)start, LeftUnit);
    public Proportion MeasureEnd(long end) => Proportion.FromScalars((Scalar)end - Origin, RightUnit);

    public MeasurementFrame WithPerspective(Perspective perspective) =>
        new(Origin, LeftUnit, RightUnit, perspective, true);

    public MeasurementFrame OpposePerspective() =>
        WithPerspective(Perspective.Oppose());

    public MeasurementFrame SwapUnitRoles() =>
        new(Origin, RightUnit, LeftUnit, Perspective, true);

    public Axis Read(DirectedInterval interval)
    {
        var dominant = new Axis(MeasureStart(interval.Start), MeasureEnd(interval.End));
        return Perspective == Perspective.Dominant ? dominant : -dominant;
    }

    public Axis Read(DirectedInterval interval, Perspective perspective) =>
        WithPerspective(perspective).Read(interval);
}
