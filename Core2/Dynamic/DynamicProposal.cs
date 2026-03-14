using Core2.Branching;

namespace Core2.Dynamic;

public sealed record DynamicProposal<TEffect>(
    string Strand,
    BranchId SourceId,
    TEffect Effect,
    IReadOnlyList<DynamicTension> Tensions,
    IReadOnlyList<IBranchAnnotation> Annotations,
    decimal Weight = 1m,
    string? Note = null)
{
    public DynamicProposal(
        string strand,
        BranchId sourceId,
        TEffect effect,
        decimal weight = 1m,
        string? note = null)
        : this(strand, sourceId, effect, [], [], weight, note)
    {
    }
}
