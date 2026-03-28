using Core3.Engine;
using Core3.Runtime;

namespace Core3.Operations;

/// <summary>
/// Binary boolean result metadata. The actual result pieces remain ordinary
/// composite elements; this record only preserves the frame, operand roles,
/// and surviving piece family for this boolean read.
/// </summary>
public sealed record EngineBooleanResult
{
    public EngineBooleanResult(
        EngineOperationContext context,
        EngineBooleanOperation operation,
        IReadOnlyList<EngineOperationPiece> pieces)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(pieces);

        if (context.Count != 2 ||
            context.Frame is not CompositeElement ||
            context.Members[0] is not CompositeElement ||
            context.Members[1] is not CompositeElement)
        {
            throw new InvalidOperationException("Binary boolean results require a composite frame and two composite members.");
        }

        Context = context;
        Operation = operation;
        Pieces = pieces;
    }

    public EngineOperationContext Context { get; }
    public CompositeElement Frame => (CompositeElement)Context.Frame;
    public CompositeElement Primary => (CompositeElement)Context.Members[0];
    public CompositeElement Secondary => (CompositeElement)Context.Members[1];
    public EngineBooleanOperation Operation { get; }
    public IReadOnlyList<EngineOperationPiece> Pieces { get; }
    public bool HasAny => Pieces.Count > 0;
    public IReadOnlyList<CompositeElement> Segments => Pieces.Select(piece => (CompositeElement)piece.Result).ToArray();
}
