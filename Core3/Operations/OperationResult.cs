using Core3.Engine;
using Core3.Runtime;

namespace Core3.Operations;

/// <summary>
/// Carries the outbound side of a one-result operation arc together with the
/// inbound context that produced it. The result element remains a normal graded
/// element; the frame relation is preserved separately as provenance rather
/// than creating a second result ontology. Some laws may also choose to keep
/// an explicit richer structure beside the normal expected result. Conceptually
/// this is the one-survivor case of the broader outbound-family pattern also
/// used by piece-producing operations.
/// </summary>
public sealed record OperationResult : ArcResult
{
    public OperationResult(
        string operationName,
        OperationContext context,
        GradedElement result,
        GradedElement? resultFrame = null,
        GradedElement? preservedStructure = null,
        GradedElement? tension = null,
        string? note = null)
        : base(context, tension, note)
    {
        OperationName = operationName;
        Result = result;
        ResultFrame = resultFrame ?? context.Frame;
        PreservedStructure = preservedStructure;
    }

    public string OperationName { get; }
    public GradedElement Result { get; }
    public GradedElement ResultFrame { get; }
    public GradedElement? PreservedStructure { get; }
    public override string OriginLawName => OperationName;
    public OperationPiece OutboundPiece =>
        new(Result, ResultFrame, Enumerable.Range(0, Context.Count).ToArray());
    public override IReadOnlyList<OperationPiece> OutboundPieces => [OutboundPiece];

    public EngineElementOutcome ReadResult() =>
        Result.ViewInFrame(ResultFrame);

    public CompositeElement GetResultBoundaryAxis() =>
        ResultFrame.Grade == Result.Grade &&
        ReadResult() is var outcome &&
        outcome.IsExact
            ? EngineBoundary.GetAxis(ResultFrame, outcome.Result)
            : EngineBoundary.CreateUnknownAxis(ResultFrame);
}





