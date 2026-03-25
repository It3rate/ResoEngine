namespace Core3.Engine;

/// <summary>
/// Runtime-graded engine element.
/// Grade 0 is atomic; higher grades are composed recursively from two
/// lower-grade children.
/// </summary>
public abstract record GradedElement
{
    public abstract int Grade { get; }
    public abstract EngineSignature Signature { get; }
    public bool HasResolvedSignature => Signature.IsResolved;
    public abstract GradedElement Mirror();
}
