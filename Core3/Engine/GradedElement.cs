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
    public abstract EngineElementOutcome FoldWithTension();
    public abstract EngineElementOutcome CommitToCalibrationWithTension(GradedElement calibration);
    public abstract EngineElementPairOutcome AlignWithTension(GradedElement other, ResolutionPolicy policy);
    public abstract EngineElementOutcome AddWithTension(GradedElement other);
    public abstract EngineElementOutcome SubtractWithTension(GradedElement other);
    public abstract EngineElementOutcome MultiplyWithTension(GradedElement other);
    public abstract EngineElementOutcome ScaleWithTension(AtomicElement factor);
    public abstract bool TryFold(out GradedElement? folded);
    public abstract bool TryCommitToCalibration(GradedElement calibration, out GradedElement? committed);
    public abstract bool TryAlignExact(GradedElement other, ResolutionPolicy policy, out GradedElement? leftAligned, out GradedElement? rightAligned);
    public abstract bool SharesUnitSpace(GradedElement other);
    public abstract bool TryAdd(GradedElement other, out GradedElement? sum);
    public abstract bool TrySubtract(GradedElement other, out GradedElement? difference);
    public abstract bool TryMultiply(GradedElement other, out GradedElement? product);
    public abstract bool TryScale(AtomicElement factor, out GradedElement? scaled);

    public bool TryAlignExact(GradedElement other, out GradedElement? leftAligned, out GradedElement? rightAligned) =>
        TryAlignExact(other, ResolutionPolicy.ExactCommonFrame, out leftAligned, out rightAligned);

    public EngineElementPairOutcome AlignWithTension(GradedElement other) =>
        AlignWithTension(other, ResolutionPolicy.ExactCommonFrame);

    public bool TryReferenceToFrame(GradedElement frame, out GradedElement? read) =>
        TryCommitToCalibration(frame, out read);

    public bool CanAdd(GradedElement other) => TryAdd(other, out _);
    public bool CanSubtract(GradedElement other) => TrySubtract(other, out _);
    public bool CanMultiply(GradedElement other) => TryMultiply(other, out _);

    protected static void RequireSameGrade(GradedElement left, GradedElement right)
    {
        if (left.Grade != right.Grade)
        {
            throw new InvalidOperationException("Elements must have the same grade.");
        }
    }
}
