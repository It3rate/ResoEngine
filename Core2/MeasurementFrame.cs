namespace ResoEngine.Core2;

/// <summary>
/// A concrete frame that gives an ordered interval measurable meaning.
/// Start is measured by the left unit, end by the right unit.
/// Each measurement is a Proportion, so reading a line yields an Axis.
/// </summary>
public readonly record struct MeasurementFrame
{
    private MeasurementFrame(Scalar origin, Scalar leftUnit, Scalar rightUnit, bool _)
    {
        if (leftUnit.IsZero)
            throw new ArgumentOutOfRangeException(nameof(leftUnit), "Left unit must be non-zero.");
        if (rightUnit.IsZero)
            throw new ArgumentOutOfRangeException(nameof(rightUnit), "Right unit must be non-zero.");

        Origin = origin;
        LeftUnit = leftUnit;
        RightUnit = rightUnit;
    }

    public MeasurementFrame(long origin, long unit)
        : this(new Scalar(origin), new Scalar(unit), new Scalar(unit), true)
    {
    }

    public MeasurementFrame(long origin, long leftUnit, long rightUnit)
        : this(new Scalar(origin), new Scalar(leftUnit), new Scalar(rightUnit), true)
    {
    }

    public Scalar Origin { get; }
    public Scalar LeftUnit { get; }
    public Scalar RightUnit { get; }

    public Proportion MeasureStart(long start) => Proportion.FromScalars(Origin - (Scalar)start, LeftUnit);
    public Proportion MeasureEnd(long end) => Proportion.FromScalars((Scalar)end - Origin, RightUnit);

    public Axis Read(DirectedInterval interval, Perspective perspective = Perspective.Dominant)
    {
        var dominant = new Axis(MeasureStart(interval.Start), MeasureEnd(interval.End));
        return perspective == Perspective.Dominant ? dominant : -dominant;
    }
}
