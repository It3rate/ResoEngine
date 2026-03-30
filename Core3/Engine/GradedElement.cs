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
    public abstract EngineElementOutcome Fold();
    public EngineElementOutcome Lower() => EngineEvaluation.Lower(this);
    public abstract EngineElementOutcome CommitToCalibrationWithTension(GradedElement calibration);
    public abstract EngineElementPairOutcome AlignWithTension(GradedElement other, ResolutionPolicy policy);
    public abstract EngineElementOutcome AddWithTension(GradedElement other);
    public abstract EngineElementOutcome SubtractWithTension(GradedElement other);
    public abstract EngineElementOutcome MultiplyWithTension(GradedElement other);
    public abstract EngineElementOutcome ScaleWithTension(AtomicElement factor);
    public EngineElementOutcome Lift(GradedElement other) =>
        EngineEvaluation.Lift(this, other);
    public GradedElement CreateZeroLikePeer() =>
        EngineEvaluation.CreateZeroLikeElement(Grade);

    public virtual bool TryFold(out GradedElement? folded)
    {
        folded = Fold().Result;
        return true;
    }
    public virtual bool TryLower(out GradedElement? lowered) =>
        EngineExactness.TryProjectExact(
            Lower(),
            static outcome => outcome.Result,
            out lowered);
    public virtual bool TryCommitToCalibration(GradedElement calibration, out GradedElement? committed) =>
        EngineExactness.TryProjectExact(
            CommitToCalibrationWithTension(calibration),
            static outcome => outcome.Result,
            out committed);

    public abstract bool TryAlignExact(GradedElement other, ResolutionPolicy policy, out GradedElement? leftAligned, out GradedElement? rightAligned);
    public abstract bool SharesUnitSpace(GradedElement other);
    public virtual bool TryAdd(GradedElement other, out GradedElement? sum) =>
        EngineExactness.TryProjectExact(
            AddWithTension(other),
            static outcome => outcome.Result,
            out sum);

    public virtual bool TrySubtract(GradedElement other, out GradedElement? difference) =>
        EngineExactness.TryProjectExact(
            SubtractWithTension(other),
            static outcome => outcome.Result,
            out difference);

    public virtual bool TryMultiply(GradedElement other, out GradedElement? product) =>
        EngineExactness.TryProjectExact(
            MultiplyWithTension(other),
            static outcome => outcome.Result,
            out product);

    public virtual bool TryScale(AtomicElement factor, out GradedElement? scaled) =>
        EngineExactness.TryProjectExact(
            ScaleWithTension(factor),
            static outcome => outcome.Result,
            out scaled);

    public bool TryLift(GradedElement other, out GradedElement? lifted) =>
        EngineExactness.TryProjectExact(
            Lift(other),
            static outcome => outcome.Result,
            out lifted);

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
