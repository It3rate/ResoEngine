using Core2.Elements;

namespace Core2.Symbolics.Expressions;

public sealed record AnchorReferenceTerm : ValueTerm
{
    public AnchorReferenceTerm(string ownerName, string anchorName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(ownerName);
        ArgumentException.ThrowIfNullOrWhiteSpace(anchorName);

        OwnerName = ownerName;
        AnchorName = anchorName;
    }

    public string OwnerName { get; }
    public string AnchorName { get; }
    public string QualifiedName => $"{OwnerName}.{AnchorName}";

    public PinSideRole? SideRole => AnchorName switch
    {
        "i" => PinSideRole.Recessive,
        "u" => PinSideRole.Dominant,
        _ => null,
    };
}
