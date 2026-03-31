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
    public abstract EngineElementOutcome Align(GradedElement other, ResolutionPolicy policy);
    public abstract EngineElementOutcome Add(GradedElement other);
    public abstract EngineElementOutcome Subtract(GradedElement other);
    public abstract EngineElementOutcome Multiply(GradedElement other);
    public abstract EngineElementOutcome Scale(AtomicElement factor);
    public EngineElementOutcome Lift(GradedElement other) =>
        EngineEvaluation.Lift(this, other);
    public EngineElementOutcome ViewInFrame(GradedElement frame) =>
        CommitToCalibration(frame);
    public GradedElement CreateZeroLikePeer() =>
        EngineEvaluation.CreateZeroLikeElement(Grade);
    public abstract bool SharesUnitSpace(GradedElement other);

    public EngineElementOutcome Align(GradedElement other) =>
        Align(other, ResolutionPolicy.ExactCommonFrame);

    protected static void RequireSameGrade(GradedElement left, GradedElement right)
    {
        if (left.Grade != right.Grade)
        {
            throw new InvalidOperationException("Elements must have the same grade.");
        }
    }
}
