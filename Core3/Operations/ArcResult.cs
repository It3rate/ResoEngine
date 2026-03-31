using Core3.Engine;
using Core3.Runtime;

namespace Core3.Operations;

/// <summary>
/// Shared outbound-arc shell for operation results that preserve context,
/// tension, and one-or-more outbound survivors without introducing a separate
/// ontology for the survivors themselves.
/// </summary>
public abstract record ArcResult
{
    protected ArcResult(
        OperationContext context,
        GradedElement? tension = null,
        string? note = null)
    {
        Context = context;
        Tension = tension;
        Note = note;
    }

    public OperationContext Context { get; }
    public GradedElement? Tension { get; }
    public string? Note { get; }
    public bool IsExact => Tension is null;
    public abstract string OriginLawName { get; }
    public abstract IReadOnlyList<OperationPiece> OutboundPieces { get; }
    public bool HasAny => OutboundPieces.Count > 0;
    public bool HasMany => OutboundPieces.Count > 1;
}



