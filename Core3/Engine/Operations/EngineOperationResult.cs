using Core3.Engine;

namespace Core3.Engine.Operations;

/// <summary>
/// Carries an operation result together with the frame context that produced it.
/// The result element remains a normal graded element; the frame relation is
/// preserved separately as provenance.
/// </summary>
public sealed record EngineOperationResult
{
    public EngineOperationResult(
        string operationName,
        GradedElement sourceFrame,
        IReadOnlyList<GradedElement> sourceMembers,
        GradedElement result,
        GradedElement? resultFrame = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(operationName);
        ArgumentNullException.ThrowIfNull(sourceFrame);
        ArgumentNullException.ThrowIfNull(sourceMembers);
        ArgumentNullException.ThrowIfNull(result);

        OperationName = operationName;
        SourceFrame = sourceFrame;
        SourceMembers = sourceMembers;
        Result = result;
        ResultFrame = resultFrame ?? sourceFrame;
    }

    public string OperationName { get; }
    public GradedElement SourceFrame { get; }
    public IReadOnlyList<GradedElement> SourceMembers { get; }
    public GradedElement Result { get; }
    public GradedElement ResultFrame { get; }

    public bool TryReadResult(out GradedElement? read) =>
        Result.TryReferenceToFrame(ResultFrame, out read);

    public CompositeElement GetResultBoundaryAxis() =>
        ResultFrame.Grade == Result.Grade &&
        TryReadResult(out var read) &&
        read is not null
            ? EngineBoundary.GetAxis(ResultFrame, read)
            : EngineBoundary.CreateUnknownAxis(ResultFrame);
}
