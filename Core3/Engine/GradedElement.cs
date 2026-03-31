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
    public abstract EngineElementOutcome CommitToCalibration(GradedElement calibration);
    public abstract EngineElementPairOutcome Align(GradedElement other, ResolutionPolicy policy);
    public abstract EngineElementOutcome Add(GradedElement other);
    public abstract EngineElementOutcome Subtract(GradedElement other);
    public abstract EngineElementOutcome Multiply(GradedElement other);
    public abstract EngineElementOutcome Scale(AtomicElement factor);
    public EngineElementOutcome Lift(GradedElement other) =>
        EngineEvaluation.Lift(this, other);
    public GradedElement CreateZeroLikePeer() =>
        EngineEvaluation.CreateZeroLikeElement(Grade);

    public virtual EngineElementOutcome CommitToCalibrationWithTension(GradedElement calibration) =>
        CommitToCalibration(calibration);

    public virtual EngineElementPairOutcome AlignWithTension(GradedElement other, ResolutionPolicy policy) =>
        Align(other, policy);

    public virtual EngineElementOutcome AddWithTension(GradedElement other) =>
        Add(other);

    public virtual EngineElementOutcome SubtractWithTension(GradedElement other) =>
        Subtract(other);

    public virtual EngineElementOutcome MultiplyWithTension(GradedElement other) =>
        Multiply(other);

    public virtual EngineElementOutcome ScaleWithTension(AtomicElement factor) =>
        Scale(factor);

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
            CommitToCalibration(calibration),
            static outcome => outcome.Result,
            out committed);

    public abstract bool TryAlignExact(GradedElement other, ResolutionPolicy policy, out GradedElement? leftAligned, out GradedElement? rightAligned);
    public abstract bool SharesUnitSpace(GradedElement other);
    public virtual bool TryAdd(GradedElement other, out GradedElement? sum) =>
        EngineExactness.TryProjectExact(
            Add(other),
            static outcome => outcome.Result,
            out sum);

    public virtual bool TrySubtract(GradedElement other, out GradedElement? difference) =>
        EngineExactness.TryProjectExact(
            Subtract(other),
            static outcome => outcome.Result,
            out difference);

    public virtual bool TryMultiply(GradedElement other, out GradedElement? product) =>
        EngineExactness.TryProjectExact(
            Multiply(other),
            static outcome => outcome.Result,
            out product);

    public virtual bool TryScale(AtomicElement factor, out GradedElement? scaled) =>
        EngineExactness.TryProjectExact(
            Scale(factor),
            static outcome => outcome.Result,
            out scaled);

    public bool TryLift(GradedElement other, out GradedElement? lifted) =>
        EngineExactness.TryProjectExact(
            Lift(other),
            static outcome => outcome.Result,
            out lifted);

    public bool TryAlignExact(GradedElement other, out GradedElement? leftAligned, out GradedElement? rightAligned) =>
        TryAlignExact(other, ResolutionPolicy.ExactCommonFrame, out leftAligned, out rightAligned);

    public EngineElementPairOutcome Align(GradedElement other) =>
        Align(other, ResolutionPolicy.ExactCommonFrame);

    public EngineElementPairOutcome AlignWithTension(GradedElement other) =>
        Align(other, ResolutionPolicy.ExactCommonFrame);

    public bool TryViewInFrame(GradedElement frame, out GradedElement? read) =>
        TryCommitToCalibration(frame, out read);

    public bool TryReferenceToFrame(GradedElement frame, out GradedElement? read) =>
        TryViewInFrame(frame, out read);

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
