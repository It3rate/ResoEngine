namespace Core3.Engine;

public sealed record EngineBooleanResult
{
    public EngineBooleanResult(
        CompositeElement frame,
        CompositeElement primary,
        CompositeElement secondary,
        EngineBooleanOperation operation,
        IReadOnlyList<EngineBooleanPiece> pieces)
    {
        ArgumentNullException.ThrowIfNull(frame);
        ArgumentNullException.ThrowIfNull(primary);
        ArgumentNullException.ThrowIfNull(secondary);
        ArgumentNullException.ThrowIfNull(pieces);

        Frame = frame;
        Primary = primary;
        Secondary = secondary;
        Operation = operation;
        Pieces = pieces;
    }

    public CompositeElement Frame { get; }
    public CompositeElement Primary { get; }
    public CompositeElement Secondary { get; }
    public EngineBooleanOperation Operation { get; }
    public IReadOnlyList<EngineBooleanPiece> Pieces { get; }
    public bool HasAny => Pieces.Count > 0;
    public IReadOnlyList<CompositeElement> Segments => Pieces.Select(piece => piece.Segment).ToArray();
}
