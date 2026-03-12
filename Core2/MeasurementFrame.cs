namespace ResoEngine.Core2;

/// <summary>
/// A concrete frame that gives an ordered interval measurable meaning.
/// Start is measured by the left unit, end by the right unit.
/// </summary>
public readonly record struct MeasurementFrame
{
    public MeasurementFrame(long origin, long leftUnitTicks, long rightUnitTicks)
    {
        if (leftUnitTicks <= 0)
            throw new ArgumentOutOfRangeException(nameof(leftUnitTicks), "Left unit must be positive.");
        if (rightUnitTicks <= 0)
            throw new ArgumentOutOfRangeException(nameof(rightUnitTicks), "Right unit must be positive.");

        Origin = origin;
        LeftUnitTicks = leftUnitTicks;
        RightUnitTicks = rightUnitTicks;
    }

    public MeasurementFrame(long origin, long unitTicks) : this(origin, unitTicks, unitTicks)
    {
    }

    public long Origin { get; }
    public long LeftUnitTicks { get; }
    public long RightUnitTicks { get; }

    public Scalar MeasureStart(long start) => new(Origin - start, LeftUnitTicks);
    public Scalar MeasureEnd(long end) => new(end - Origin, RightUnitTicks);

    public Proportion Read(DirectedInterval interval, Perspective perspective = Perspective.Dominant)
    {
        var dominant = new Proportion(MeasureStart(interval.Start), MeasureEnd(interval.End));
        return perspective == Perspective.Dominant ? dominant : -dominant;
    }
}
