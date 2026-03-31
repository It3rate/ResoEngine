using Core3.Engine;
using Core3.Runtime;

namespace Core3.Operations;

/// <summary>
/// Carries the outbound side of a one-result operation arc together with the
/// inbound context that produced it. The result element remains a normal graded
/// element; the frame relation is preserved separately as provenance rather
/// than creating a second result ontology. Conceptually this is the
/// one-survivor case of the broader outbound-family pattern also used by
/// piece-producing operations.
/// </summary>
public sealed record EngineOperationResult : IExactResult
{
    public EngineOperationResult(
        string operationName,
        EngineOperationContext context,
        GradedElement result,
        GradedElement? resultFrame = null,
        GradedElement? preservedStructure = null,
        GradedElement? tension = null,
        string? note = null)
    {
        OperationName = operationName;
        Context = context;
        Result = result;
        ResultFrame = resultFrame ?? context.Frame;
        PreservedStructure = preservedStructure;
        Tension = tension;
        Note = note;
    }

    public EngineOperationResult(
        string operationName,
        GradedElement sourceFrame,
        IReadOnlyList<GradedElement> sourceMembers,
        GradedElement result,
        GradedElement? resultFrame = null,
        GradedElement? preservedStructure = null,
        GradedElement? tension = null,
        string? note = null)
        : this(
            operationName,
            new EngineOperationContext(sourceFrame, sourceMembers, true),
            result,
            resultFrame,
            preservedStructure,
            tension,
            note)
    {
    }

    public string OperationName { get; }
    public EngineOperationContext Context { get; }
    public EngineOperationContext Inbound => Context;
    public GradedElement SourceFrame => Context.Frame;
    public IReadOnlyList<GradedElement> SourceMembers => Context.Members;
    public bool IsOrdered => Context.IsOrdered;
    public string OriginLaw => OperationName;
    public GradedElement Result { get; }
    public GradedElement ResultFrame { get; }
    public GradedElement? PreservedStructure { get; }
    public GradedElement Outbound => Result;
    public string OriginLawName => OperationName;
    public EngineOperationPiece OutboundPiece =>
        new(Result, ResultFrame, Enumerable.Range(0, Context.Count).ToArray());
    public IReadOnlyList<EngineOperationPiece> OutboundPieces => [OutboundPiece];
    public GradedElement? Tension { get; }
    public string? Note { get; }
    public bool IsExact => Tension is null;
    public bool HasAny => true;
    public bool HasMany => false;

    public bool TryReadResult(out GradedElement? read) =>
        Result.TryViewInFrame(ResultFrame, out read);

    public bool TryGetRawMultiplyKernel(out CompositeElement? kernel)
    {
        // Temporary inspection shell. Long term this wants to become an
        // ordinary frame/family read over preserved kernel structure rather
        // than a bespoke multiply-only helper.
        if (PreservedStructure is CompositeElement preservedKernel &&
            string.Equals(OperationName, "Multiply", StringComparison.Ordinal))
        {
            kernel = preservedKernel;
            return true;
        }

        if (!string.Equals(OperationName, "Multiply", StringComparison.Ordinal) ||
            Context.Count != 2)
        {
            kernel = null;
            return false;
        }

        var family = new EngineFamily(Context);

        if (!family.TryReadAllWithTension(out var readResult) ||
            readResult is null ||
            readResult.Reads[0] is not CompositeElement left ||
            readResult.Reads[1] is not CompositeElement right)
        {
            kernel = null;
            return false;
        }

        return left.TryMultiplyKernel(right, out kernel);
    }

    public CompositeElement GetResultBoundaryAxis() =>
        ResultFrame.Grade == Result.Grade &&
        TryReadResult(out var read) &&
        read is not null
            ? EngineBoundary.GetAxis(ResultFrame, read)
            : EngineBoundary.CreateUnknownAxis(ResultFrame);
}
