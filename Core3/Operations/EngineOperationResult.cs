using Core3.Engine;
using Core3.Runtime;

namespace Core3.Operations;

/// <summary>
/// Carries an operation result together with the frame context that produced it.
/// The result element remains a normal graded element; the frame relation is
/// preserved separately as provenance.
/// </summary>
public sealed record EngineOperationResult
{
    public EngineOperationResult(
        string operationName,
        EngineOperationContext context,
        GradedElement result,
        GradedElement? resultFrame = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(operationName);
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(result);

        OperationName = operationName;
        Context = context;
        Result = result;
        ResultFrame = resultFrame ?? context.Frame;
    }

    public EngineOperationResult(
        string operationName,
        GradedElement sourceFrame,
        IReadOnlyList<GradedElement> sourceMembers,
        GradedElement result,
        GradedElement? resultFrame = null)
        : this(
            operationName,
            new EngineOperationContext(sourceFrame, sourceMembers, isOrdered: true),
            result,
            resultFrame)
    {
    }

    public string OperationName { get; }
    public EngineOperationContext Context { get; }
    public GradedElement SourceFrame => Context.Frame;
    public IReadOnlyList<GradedElement> SourceMembers => Context.Members;
    public bool IsOrdered => Context.IsOrdered;
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
